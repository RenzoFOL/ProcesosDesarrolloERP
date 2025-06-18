using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProyectoProcesosExperienciaV1.Pages.Account
{
    [Authorize]
    public class PerfilModel : PageModel
    {
        public string Email { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            Email = User.Identity?.Name ?? "usuario@ejemplo.com"; // Solo simula el correo
        }
    }
}
