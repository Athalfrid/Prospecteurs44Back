using System.ComponentModel.DataAnnotations;

namespace Prospecteurs44Back.DTO
{
    public class TopicDTO
    {

        [Required(ErrorMessage = "Le titre est obligatoire.")]
        [StringLength(100, ErrorMessage = "Le titre ne peut pas dépasser 100 caractères.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Le contenu est requis.")]
        public string? Content { get; set; }
    }
}