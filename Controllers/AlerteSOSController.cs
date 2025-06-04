using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Prospecteurs44Back.Data;
using Prospecteurs44Back.Model;
using Prospecteurs44Back.Services;

namespace Prospecteurs44Back.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AlerteSOSController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public AlerteSOSController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AlerteSOS>>> GetAlerteSOS()
        {
            return await _context.AlerteSOS.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AlerteSOS>> GetAlerteSOS(int id)
        {
            var alerteSOS = await _context.AlerteSOS.FindAsync(id);

            if (alerteSOS == null)
            {
                return NotFound(new { message = "Alerte non trouvée" });
            }

            return alerteSOS;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<AlerteSOS>> PostAlerteSOS(AlerteSOS alerteSOS)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Nombre maximum d'alertes autorisées par email dans les dernières 24h
            int maxAlertesParEmail = 5;
            var dateLimite = DateTime.UtcNow.AddHours(-24);

            var nbAlertesRecente = await _context.AlerteSOS
                .CountAsync(a => a.Email == alerteSOS.Email && a.DateAlerte >= dateLimite);

            if (nbAlertesRecente >= maxAlertesParEmail)
            {
                return BadRequest($"Vous avez déjà posté {maxAlertesParEmail} alertes dans les dernières 24h. Merci de patienter.");
            }

            var dateModified = DateTime.SpecifyKind(alerteSOS.DateObjetPerdu, DateTimeKind.Utc);
            alerteSOS.DateObjetPerdu = dateModified;

            _context.AlerteSOS.Add(alerteSOS);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAlerteSOS", new { id = alerteSOS.IdAlerte }, alerteSOS);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutAlerteSOS(int id, AlerteSOS alerteSOS)
        {
            var existingAlerte = await _context.AlerteSOS.FindAsync(id);
            if (existingAlerte == null)
            {
                return NotFound("Alerte non trouvée");
            }

            // Mise à jour des propriétés (sans toucher à l'ID)
            existingAlerte.TitreAlerte = alerteSOS.TitreAlerte;
            existingAlerte.DateAlerte = alerteSOS.DateAlerte;
            existingAlerte.DescriptionAlerte = alerteSOS.DescriptionAlerte;
            existingAlerte.DateObjetPerdu = alerteSOS.DateObjetPerdu;
            existingAlerte.LieuObjetPerdu = alerteSOS.LieuObjetPerdu;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlerteSOSExists(id))
                {
                    return NotFound("Alerte non trouvée");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlerteSOS(int id)
        {
            var alerteSOS = await _context.AlerteSOS.FindAsync(id);
            if (alerteSOS == null)
            {
                return NotFound();
            }

            _context.AlerteSOS.Remove(alerteSOS);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/contactParticipation")]
        public async Task<IActionResult> ContacterProprietaire(int id)
        {
            var alerte = await _context.AlerteSOS.FindAsync(id);

            if (alerte == null || String.IsNullOrEmpty(alerte.Email))
            {
                return NotFound("Alerte introuvable ou email manquant");
            }

            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            Console.WriteLine(userEmail);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
            {
                return Unauthorized();
            }

            var sujetPourProprietaire = $"Quelqu'un veut vous aider à retrouver votre objet perdu !";
            var messagePourProprietaire = $@"
                Bonjour,

                {user.InformationsPersonnelles.Prenom} {user.InformationsPersonnelles.Nom} ({user.Email}) souhaite vous aider à retrouver votre objet perdu.
                Il a cliqué sur 'Je veux aider' depuis l'alerte suivante : '{alerte.TitreAlerte}'.

                N'hésitez pas à le contacter rapidement !

                Cordialement,
                L'équipe Prospecteurs44";


            var sujetPourUtilisateur = $"Vous avez accepter de participer à une alerte !";
            var messagePourUtilisateur = $@"
                Bonjour,

                Vous avez cliqué sur 'Je veux aider' depuis l'alerte suivante : '{alerte.TitreAlerte}'.

                Pour rappel des informations :

                    Email du propriétaire : {alerte.Email}
                    Lieu : {alerte.LieuObjetPerdu}
                    Description : {alerte.DescriptionAlerte}

                N'hésitez pas à le contacter rapidement afin de l'aider au maximum !

                Cordialement,
                L'équipe Prospecteurs44";

            //await _emailService.SendEmailAsync(alerte.Email, sujetPourProprietaire, messagePourProprietaire);

            return Ok(new { message = "Email envoyé au propriétaire !" });
        }

        private bool AlerteSOSExists(int id)
        {
            return _context.AlerteSOS.Any(e => e.IdAlerte == id);
        }
    }
}
