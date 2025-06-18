using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using ProyectoProcesosExperienciaV1.Services;
using ProyectoProcesosExperienciaV1.Data.ApplicationDbContext;

namespace ProyectoProcesosExperienciaV1.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public RegisterModel(UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public bool Success { get; set; }

public class InputModel
{
    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Debes confirmar tu contraseña.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmPassword { get; set; }
}


        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = new ApplicationUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                IsEnabled = false,
                RoleAssigned = "Pendiente"
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Page("/Account/ConfirmEmail", null,
                    new { userId = user.Id, token }, Request.Scheme);

                var body = $"<p>Gracias por registrarte. Confirma tu cuenta haciendo clic aquí:</p><p><a href='{confirmationLink}'>Confirmar cuenta</a></p>";
                await _emailService.SendEmailAsync(Input.Email, "Confirma tu cuenta - ManaSys", body);

                Success = true;
                return Page();
            }

            // ⛔ Si falló, mostramos todos los errores en pantalla
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
