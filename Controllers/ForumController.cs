using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Prospecteurs44Back.Data;
using Prospecteurs44Back.Model;
using Prospecteurs44Back.DTO;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ForumController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public ForumController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Topic>>> GetTopics()
        {
            return await _context.Topics.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Topic>> GetTopic(int id)
        {
            var topic = await _context.Topics.FindAsync(id);

            if (topic == null)
            {
                return NotFound(new { message = "Topic non trouvé !" });
            }

            return topic;
        }


        [HttpPost]
        public async Task<ActionResult<Topic>> CreateTopic(TopicDTO topicDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newTopic = new Topic
            {
                Title = topicDTO.Title,
                Content = topicDTO.Content,
            };
            _context.Topics.Add(newTopic);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTopic), new { id = newTopic.Id }, newTopic);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTopic(int id, Topic updatedTopic)
        {
            if (id != updatedTopic.Id)
            {
                return BadRequest("Erreur identifiant !");
            }

            _context.Entry(updatedTopic).State = EntityState.Modified;
            updatedTopic.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TopicExists(id))
                {
                    return NotFound("Topic introuvable en bdd !");
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return NotFound("Topic introuvable");
            }
            _context.Topics.Remove(topic);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/close")]
        public async Task<IActionResult> CloseTopic(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return NotFound();
            }

            if (topic.IsClosed)
            {
                return BadRequest("Le topic est déjà clôturé !");
            }

            topic.IsClosed = true;
            topic.UpdatedAt = DateTime.UtcNow;
            topic.ClosedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Topic clôturé avec succès !", topic });
        }

        private bool TopicExists(int id)
        {
            return _context.Topics.Any(e => e.Id == id);
        }
    }
}
