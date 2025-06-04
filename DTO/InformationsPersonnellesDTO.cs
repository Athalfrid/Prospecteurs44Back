namespace Prospecteurs44Back.DTO
{
    public class InformationsPersonnellesDTO
    {
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public DateTime DateNaissance { get; set; }
        public string Ville { get; set; } = string.Empty;
        public string CodePostal { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
    }
}