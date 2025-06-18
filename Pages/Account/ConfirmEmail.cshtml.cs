using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProyectoProcesosExperienciaV1.Data.ApplicationDbContext;

namespace ProyectoProcesosExperienciaV1.Pages.Account
{
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ConfirmEmailModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string userId, string token)
        {
            if (userId == null || token == null)
                return RedirectToPage("/Index");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return RedirectToPage("/Index");

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
                return Page();

            return RedirectToPage("/Error");
        }
    }
}