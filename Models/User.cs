using System.ComponentModel.DataAnnotations;

namespace Prospecteurs44Back.Model
{
    public class User
    {
        public int UserId { get; set; }
        public string UserPseudo { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = "ROLE_USER"; //ROLE_ADMIN - ROLE_MODO - ROLE_USER
        public User? UserParrain { get; set; }
        public List<User>? Filleuls { get; set; }
        public DateTime DateInscription { get; set; } = DateTime.UtcNow;
        public InformationsPersonnelles InformationsPersonnelles { get; set; } = new();
    }
}