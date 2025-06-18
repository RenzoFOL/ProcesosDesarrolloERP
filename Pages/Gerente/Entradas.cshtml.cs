using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProyectoProcesosExperienciaV1.Data.Dao;
using ProyectoProcesosExperienciaV1.Data.Modelos;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoProcesosExperienciaV1.Pages.Gerente
{
    [Authorize(Roles = "Gerente")]
    public class EntradasModel : PageModel
    {
        private readonly EntradaDAO entradaDAO;
        private readonly ProductoDAO productoDAO;

        [BindProperty]
        public Entrada? NuevaEntrada { get; set; }

        public List<EntradaExtendida> ListaEntradas { get; set; } = new();

        private readonly string connectionString;

        public EntradasModel(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("InventarioConnection");
            entradaDAO = new EntradaDAO(connectionString);
            productoDAO = new ProductoDAO(connectionString);
        }

        public void OnGet()
        {
            CargarEntradas();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid || NuevaEntrada == null)
            {
                TempData["Error"] = "❌ Error en los datos. Revisa los campos.";
                return RedirectToPage();
            }

            // Validar si el producto existe
            var producto = productoDAO.BuscarProductoPorID(NuevaEntrada.fkID_Producto);
            if (producto == null)
            {
                TempData["Error"] = "❌ El producto no existe. Verifica el ID.";
                return RedirectToPage();
            }

            entradaDAO.InsertarEntrada(NuevaEntrada);
            TempData["Mensaje"] = "✅ Entrada registrada correctamente.";
            return RedirectToPage();
        }

        public IActionResult OnPostEliminar(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "❌ ID inválido para eliminar.";
                return RedirectToPage();
            }

            entradaDAO.EliminarEntrada(id);
            TempData["Mensaje"] = "✅ Entrada eliminada exitosamente.";
            return RedirectToPage();
        }

        private void CargarEntradas()
        {
            ListaEntradas = entradaDAO.ObtenerEntradasExtendidas();
        }
    }
}
