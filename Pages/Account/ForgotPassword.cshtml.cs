using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using ProyectoProcesosExperienciaV1.Services;

namespace ProyectoProcesosExperienciaV1.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly IEmailService _emailService;

        public ForgotPasswordModel(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public bool EmailSent { get; set; } = false;

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Aquí iría la lógica para generar un token real y guardarlo
            var tokenFake = Guid.NewGuid().ToString();

            var link = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { token = tokenFake, email = Email },
                protocol: Request.Scheme);

            var body = $"<p>Haz clic en el siguiente enlace para restablecer tu contraseña:</p><p><a href='{link}'>Recuperar contraseña</a></p>";

            await _emailService.SendEmailAsync(Email, "Recuperación de contraseña - ManaSys", body);
            EmailSent = true;

            return Page();
        }
    }
}
