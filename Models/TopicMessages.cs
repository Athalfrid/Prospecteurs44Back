using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prospecteurs44Back.Model
{
    public class TopicMessages
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le contenu du message est obligatoire.")]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Relation vers le Topic
        public int TopicId { get; set; }

        [ForeignKey("TopicId")]
        public Topic Topic { get; set; }

        // Optionnel : relation vers l’auteur
        public int AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public User Author { get; set; }
    }
}
