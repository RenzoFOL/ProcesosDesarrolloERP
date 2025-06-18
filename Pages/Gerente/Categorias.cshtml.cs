using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProyectoProcesosExperienciaV1.Data.Dao;
using ProyectoProcesosExperienciaV1.Data.Modelos;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoProcesosExperienciaV1.Pages.Gerente
{
    [Authorize(Roles = "Gerente")]
    public class CategoriasModel : PageModel
    {
        private readonly CategoriaDAO categoriaDAO;
        private readonly string connectionString;

        public List<Categoria> ListaCategorias { get; set; } = new();

        [BindProperty]
        public Categoria NuevaCategoria { get; set; }

        public CategoriasModel(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("InventarioConnection");
            categoriaDAO = new CategoriaDAO(connectionString);
        }

        public void OnGet()
        {
            CargarCategorias();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "❌ Error al registrar la categoría. Revisa los campos.";
                return RedirectToPage();
            }

            categoriaDAO.InsertarCategoria(NuevaCategoria);
            TempData["Mensaje"] = "✅ Categoría agregada correctamente.";
            return RedirectToPage();
        }public IActionResult OnPostAgregarMultiples(List<string> categorias)
{
    if (categorias == null || categorias.Any(c => string.IsNullOrWhiteSpace(c)))
    {
        TempData["Error"] = "❌ Todos los campos deben estar llenos.";
        return RedirectToPage();
    }

var existentes = categoriaDAO.ObtenerCategorias()
    .Select(c => c.Nombre.Trim().ToLower())
    .ToHashSet();

var nuevas = categorias
    .Select(c => c.Trim())
    .Where(c => !existentes.Contains(c.ToLower()))
    .Distinct();

foreach (var nombre in nuevas)
{
    categoriaDAO.InsertarCategoria(new Categoria { Nombre = nombre });
}

    TempData["Mensaje"] = "✅ Categorías agregadas exitosamente.";
    return RedirectToPage();
}



        public IActionResult OnPostEliminar(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "❌ No se pudo eliminar. ID inválido.";
                return RedirectToPage();
            }

            try
            {
                categoriaDAO.EliminarCategoria(id);
                TempData["Mensaje"] = "✅ Categoría eliminada exitosamente.";
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = $"❌ Error al eliminar la categoría: {ex.Message}. Es posible que tenga productos asociados.";
            }

            return RedirectToPage();
        }

        public IActionResult OnPostActualizar()
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "❌ Error al actualizar la categoría. Revisa los campos.";
                return RedirectToPage();
            }

            categoriaDAO.Actualizar(NuevaCategoria);
            TempData["Mensaje"] = "✅ Categoría actualizada correctamente.";
            return RedirectToPage();
        }

        private void CargarCategorias()
        {
            ListaCategorias = categoriaDAO.ObtenerCategorias();
        }
        

    }
}