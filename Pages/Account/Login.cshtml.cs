using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProyectoProcesosExperienciaV1.Data.ApplicationDbContext;
using ProyectoProcesosExperienciaV1.Services;
using System.ComponentModel.DataAnnotations;

namespace ProyectoProcesosExperienciaV1.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;

        public LoginModel(SignInManager<ApplicationUser> signInManager,
                          UserManager<ApplicationUser> userManager,
                          IJwtService jwtService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtService = jwtService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ErrorMessage { get; set; }

        public class InputModel
        {
    [Required(ErrorMessage = "El campo correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "Debes ingresar un correo electrónico válido.")]
            public string Email { get; set; }

    [Required(ErrorMessage = "El campo contraseña es obligatorio.")]
    [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ErrorMessage = "El usuario no existe.";
                return Page();
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ErrorMessage = "Debes confirmar tu correo antes de iniciar sesión.";
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(user, Input.Password, false, false);
            if (result.Succeeded)
            {
                if (user.Email == "renol1099@gmail.com")
                    return RedirectToPage("/SuperAdmin/Panel");

                if (!user.IsEnabled)
                {
                    await _signInManager.SignOutAsync();
                    return RedirectToPage("/Account/UsuarioNoHabilitado");
                }
                if (!await _userManager.IsInRoleAsync(user, user.RoleAssigned))
                {
                    await _userManager.AddToRoleAsync(user, user.RoleAssigned);
                }

                return RedirectToPage(user.RoleAssigned switch
                {
                    "Administrador" => "/Admin/Dashboard",
                    "Gerente" => "/Gerente/Dashboard",
                    "Empleado" => "/Empleado/Dashboard",
                    _ => "/Account/Perfil"
                });
            }

            ErrorMessage = "Contraseña incorrecta.";
            return Page();
        }
    }
}
