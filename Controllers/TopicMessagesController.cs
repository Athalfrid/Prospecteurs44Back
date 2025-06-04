using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prospecteurs44Back;
using Prospecteurs44Back.Data;
using Prospecteurs44Back.Model;

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
            return await _context.TopicMessages.Where(m => m.TopicId == topicId).ToListAsync();
        }

        [HttpPost]
        public async Task<IActionResult> PostMessage(int topicId, [FromBody] TopicMessages message)
        {
            var topic = await _context.Topics.FindAsync(topicId);
            if (topic == null)
            {
                return NotFound("Topic introuvable !");
            }

            if (topic.IsClosed)
            {
                return BadRequest("Ce topic est clôturé, impossible de publier de nouveaux messages !");
            }

            message.TopicId = topicId;
            message.CreatedAt = DateTime.Now;

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


            return Ok("");
        }

        private bool TopicMessagesExists(int id)
        {
            return _context.TopicMessages.Any(e => e.Id == id);
        }
    }
}
