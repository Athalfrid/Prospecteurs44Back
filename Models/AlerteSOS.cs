using System.ComponentModel.DataAnnotations;

namespace Prospecteurs44Back.Model
{
    public class AlerteSOS
    {
        [Key]
        public int IdAlerte { get; set; }
        public string? TitreAlerte { get; set; }
        public DateTime DateAlerte { get; set; } = DateTime.Now;
        public string? DescriptionAlerte { get; set; }
        public DateTime DateObjetPerdu { get; set; }
        public string? LieuObjetPerdu { get; set; }
        public string? Email { get; set; }
    }
}