using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Prospecteurs44Back.Model
{
    public class Topic
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le titre est obligatoire.")]
        [StringLength(100, ErrorMessage = "Le titre ne peut pas dépasser 100 caractères.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Le contenu est requis.")]
        public string? Content { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [DataType(DataType.DateTime)]
        public DateTime? UpdatedAt { get; set; }

        public bool IsClosed { get; set; }
    }
}
