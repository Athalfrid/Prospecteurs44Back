namespace Prospecteurs44Back.DTO
{
    public class RegisterDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Pseudo { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string EmailParrain { get; set; } = string.Empty;
        public bool IsParrained { get; set; }

        public InformationsPersonnellesDTO InformationsPersonnelles { get; set; } = new();

    }
}