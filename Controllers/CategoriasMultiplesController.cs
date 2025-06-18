using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ProyectoProcesosExperienciaV1.Data.Dao;
using ProyectoProcesosExperienciaV1.Data.Modelos;
using System.Collections.Generic;

namespace ProyectoProcesosExperienciaV1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasMultiplesController : ControllerBase
    {
        private readonly CategoriaDAO _categoriaDAO;

        public CategoriasMultiplesController(IConfiguration config)
        {
            _categoriaDAO = new CategoriaDAO(config.GetConnectionString("InventarioConnection"));
        }

        [HttpPost]
        public IActionResult Post([FromBody] List<string> categorias)
        {
            if (categorias == null || categorias.Count == 0)
                return BadRequest("❌ La lista de categorías está vacía.");

            var categoriasValidas = new List<Categoria>();

            foreach (var nombre in categorias)
            {
                var nombreLimpio = nombre?.Trim();
                if (!string.IsNullOrWhiteSpace(nombreLimpio))
                {
                    categoriasValidas.Add(new Categoria { Nombre = nombreLimpio });
                }
            }

            if (categoriasValidas.Count == 0)
                return BadRequest("❌ Todas las categorías estaban vacías o inválidas.");

            foreach (var categoria in categoriasValidas)
            {
                _categoriaDAO.InsertarCategoria(categoria);
            }

            return Ok(new { mensaje = "✅ Categorías agregadas correctamente." });
        }
    }
}
