using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prospecteurs44Back.Model
{
    public class TopicMessages
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le contenu du message est obligatoire.")]
        public string? Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relation vers le Topic
        public int TopicId { get; set; }

        [ForeignKey("TopicId")]
        public required Topic Topic { get; set; }

        [ForeignKey("AuthorId")]
        public required User Author { get; set; }

        public bool IsReported { get; set; } = false;
    }
}
