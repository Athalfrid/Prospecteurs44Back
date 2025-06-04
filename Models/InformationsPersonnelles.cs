namespace Prospecteurs44Back.Model
{
    public class InformationsPersonnelles
    {
        public int InformationsPersonnellesID { get; set; }
        public string Nom { get; set; } = null!;
        public string Prenom { get; set; } = null!;
        public DateTime DateNaissance { get; set; }
        public string Ville { get; set; } = null!;
        public string CodePostal { get; set; } = null!;
        public string Telephone { get; set; } = null!;
    }
}