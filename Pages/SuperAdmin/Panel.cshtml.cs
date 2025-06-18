using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProyectoProcesosExperienciaV1.Data.ApplicationDbContext;

namespace ProyectoProcesosExperienciaV1.Pages.SuperAdmin
{
    [Authorize(Policy = "SuperAdminOnly")]
    public class PanelModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public PanelModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public List<ApplicationUser> Users { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            Users = _userManager.Users
                .Where(u => u.Email != "renol1099@gmail.com")
                .OrderBy(u => u.Email)
                .ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var allUsers = _userManager.Users
                .Where(u => u.Email != "renol1099@gmail.com")
                .ToList();

            foreach (var inputUser in Users)
            {
                var dbUser = await _userManager.FindByIdAsync(inputUser.Id);
                if (dbUser != null)
                {
                    dbUser.IsEnabled = inputUser.IsEnabled;
                    dbUser.RoleAssigned = inputUser.RoleAssigned;
                    dbUser.EmailConfirmed = inputUser.EmailConfirmed;

                    await _userManager.UpdateAsync(dbUser);
                }
            }

            TempData["SuccessMessage"] = "✅ Cambios guardados correctamente.";
            return RedirectToPage();
        }
    }
}
