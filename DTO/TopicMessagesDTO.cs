using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Prospecteurs44Back.DTO
{
    public class TopicMessagesDTO
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsReported { get; set; }

        public AuthorDTO? Author { get; set; }

        public TopicMiniDTO? Topic { get; set; }

    }
}
