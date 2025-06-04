using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Prospecteurs44Back.Data;
using Prospecteurs44Back.DTO;
using Prospecteurs44Back.Model;

namespace Prospecteurs44Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Inscription d’un nouvel utilisateur avec vérification du pseudo, email et (optionnellement) du parrain.
        /// </summary>
        /// <param name="registerDTO">Les informations d’inscription envoyées par l’utilisateur</param>
        /// <returns>Un message de succès, l’ID de l’utilisateur et un token JWT</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Vérifie si le pseudo est déjà utilisé
            if (await _context.Users.AnyAsync(u => u.UserPseudo == registerDTO.Pseudo))
            {
                return Conflict("Nom d'utilisateur déjà existant !");
            }

            // Vérifie si l’email est déjà utilisé
            if (await _context.Users.AnyAsync(u => u.Email == registerDTO.Email))
            {
                return Conflict("Email déjà existant !");
            }

            // Hachage sécurisé du mot de passe
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password);

            User? parrain = null;

            // Si l’utilisateur est parrainé, on vérifie l’existence du parrain
            if (registerDTO.IsParrained)
            {
                if (string.IsNullOrWhiteSpace(registerDTO.EmailParrain))
                {
                    return BadRequest("L'adresse email du parrain est requise.");
                }

                parrain = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerDTO.EmailParrain);

                if (parrain == null)
                {
                    return BadRequest("Parrain non trouvé.");
                }
            }

            // Création du nouvel utilisateur
            var newUser = new User
            {
                UserPseudo = registerDTO.Pseudo,
                Email = registerDTO.Email,
                Password = hashedPassword,
                InformationsPersonnelles = new InformationsPersonnelles
                {
                    Nom = registerDTO.InformationsPersonnelles.Nom,
                    Prenom = registerDTO.InformationsPersonnelles.Prenom,
                    DateNaissance = DateTime.SpecifyKind(registerDTO.InformationsPersonnelles.DateNaissance, DateTimeKind.Utc),
                    Ville = registerDTO.InformationsPersonnelles.Ville,
                    CodePostal = registerDTO.InformationsPersonnelles.CodePostal,
                    Telephone = registerDTO.InformationsPersonnelles.Telephone
                },
                UserParrain = parrain
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Inscription réussie", userId = newUser.UserId, token = WriteMyToken(newUser) });
        }

        /// <summary>
        /// Connexion de l’utilisateur et génération d’un token JWT.
        /// </summary>
        /// <param name="loginDTO">Email et mot de passe de l’utilisateur</param>
        /// <returns>Token JWT si les identifiants sont valides</returns>
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDTO loginDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDTO.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password))
            {
                return Unauthorized("Email ou mot de passe incorrect !");
            }

            return Ok(new { token = WriteMyToken(user) });
        }

        /// <summary>
        /// Supprime un utilisateur à partir de son ID.
        /// </summary>
        /// <param name="id">Identifiant de l’utilisateur à supprimer</param>
        /// <returns>Message de confirmation ou erreur</returns>
        [HttpDelete("user/{id}")]
        public async Task<ActionResult<string>> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound("Utilisateur non trouvé");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Utilisateur supprimé avec succès!" });
        }

        /// <summary>
        /// Génère un token JWT avec les claims de l’utilisateur.
        /// </summary>
        /// <param name="user">L’utilisateur pour lequel générer le token</param>
        /// <returns>Chaîne JWT encodée</returns>
        private string WriteMyToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserPseudo),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(2),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
