using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prospecteurs44Back;
using Prospecteurs44Back.Data;
using Prospecteurs44Back.Model;
using Prospecteurs44Back.DTO;

namespace MyApp.Namespace
{
    [Route("api/topics/{topicId}/messages")]
    [ApiController]
    public class TopicMessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TopicMessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TopicMessages>>> GetMessagesByTopic(int topicId)
        {
            var messages = await _context.TopicMessages
            .Include(m => m.Topic)
            .Include(m => m.Author)
            .Where(m => m.Topic.Id == topicId)
            .Select(m => new TopicMessagesDTO
            {
                Id = m.Id,
                Content = m.Content,
                CreatedAt = m.CreatedAt,
                IsReported = m.IsReported,
                Author = new AuthorDTO
                {
                    UserId = m.Author.UserId,
                    UserPseudo = m.Author.UserPseudo
                },
                Topic = new TopicMiniDTO
                {
                    Id = m.Topic.Id,
                    Title = m.Topic.Title
                }
            })
            .ToListAsync();

            return Ok(messages);
        }

        [HttpPost]
        public async Task<IActionResult> PostMessage(int topicId, [FromBody] TopicMessagesDTO messageDTO)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("Utilisateur non authentifié");
            }

            var userId = int.Parse(userIdClaim.Value);


            var topic = await _context.Topics.FindAsync(topicId);
            if (topic == null)
            {
                return NotFound("Topic introuvable !");
            }

            if (topic.IsClosed)
            {
                return BadRequest("Ce topic est clôturé, impossible de publier de nouveaux messages !");
            }

            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound("Utilisateur non trouvé.");
            }

            var message = new TopicMessages
            {
                Content = messageDTO.Content,
                Author = user,
                Topic = topic,
                CreatedAt = DateTime.UtcNow
            };

            _context.TopicMessages.Add(message);
            await _context.SaveChangesAsync();

            return Ok(message);
        }

        [HttpPost("{messageId}/report")]
        public async Task<IActionResult> ReportMessage(int topicId, int messageId)
        {
            var topic = await _context.Topics.FindAsync(topicId);
            if (topic == null)
            {
                return NotFound("Topic introuvable !");
            }

            if (topic.IsClosed)
            {
                return BadRequest("Ce topic est clôturé, impossible de signaler de nouveaux messages !");
            }

            var message = await _context.TopicMessages
                .Include(m => m.Author)
                .FirstOrDefaultAsync(m => m.Id == messageId && m.Topic.Id == topicId);

            if (message == null)
            {
                return NotFound("Message introuvable dans ce topic !");
            }

            message.IsReported = true;

            await _context.SaveChangesAsync();

            var userName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            var sujetPourProprietaire = $"Nouveau message signalé !";
            var messagePourProprietaire = $@"
                Bonjour,

                {userName}  porte à votre connaissance qu'il juge offensant :

                Message du {message.CreatedAt} de {message.Author.UserPseudo} :
                {message.Content}

                Prenez le temps de vous renseigner ;-)

                Cordialement,
                L'équipe Prospecteurs44";



            //await _emailService.SendEmailAsync(alerte.Email, sujetPourProprietaire, messagePourProprietaire);



            return Ok(new { message = "Message signalé avec succès !" });
        }

        public async Task<IActionResult> DeleteMessage(int idMessage)
        {
            var message = await _context.TopicMessages.FindAsync(idMessage);

            if (message == null)
            {
                return NotFound("Message introuvable");
            }

            _context.TopicMessages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
