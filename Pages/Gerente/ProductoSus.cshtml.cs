using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProyectoProcesosExperienciaV1.Data.Dao;
using Microsoft.AspNetCore.Authorization;
using ProyectoProcesosExperienciaV1.Data.Modelos;
using Microsoft.Extensions.Configuration;

namespace ProyectoProcesosExperienciaV1.Pages.Gerente 
{
    [Authorize(Roles = "Gerente")]
    public class ProductoSusModel : PageModel
    {
        private readonly CategoriaDAO categoriaDAO;
        private readonly ProductoDAO productoDAO;
        private readonly string connectionString;

        public List<Categoria> ListaCategorias { get; set; } = new();
        public List<ProductoExtendido> ListaProductos { get; set; } = new();

        [BindProperty]
        public Producto NuevoProducto { get; set; } = new();

        // ✅ AGREGADO: Propiedad para model binding de múltiples productos
        [BindProperty]
        public List<Producto> Productos { get; set; } = new();

        public ProductoSusModel(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("InventarioConnection");
            categoriaDAO = new CategoriaDAO(connectionString);
            productoDAO = new ProductoDAO(connectionString);
        }

        public void OnGet()
        {
            CargarDatos();
        }

        public IActionResult OnPost()
        {
            try
            {
                if (!ModelState.IsValid || NuevoProducto == null)
                {
                    TempData["Error"] = "❌ Error al registrar el producto. Revisa los campos.";
                    CargarDatos(); // ✅ Recargar datos en caso de error
                    return Page();
                }

                // ✅ Validar que no exista el producto
                if (productoDAO.ExisteProducto(NuevoProducto.ID_Producto))
                {
                    TempData["Error"] = $"❌ El producto con ID '{NuevoProducto.ID_Producto}' ya existe.";
                    CargarDatos();
                    return Page();
                }

                productoDAO.InsertarProducto(NuevoProducto);
                TempData["Mensaje"] = "✅ Producto agregado correctamente.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Error al registrar producto: {ex.Message}";
                CargarDatos();
                return Page();
            }
        }

        public IActionResult OnPostEliminar(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "❌ No se pudo eliminar. ID inválido.";
                    return RedirectToPage();
                }

                productoDAO.EliminarProducto(id);
                TempData["Mensaje"] = "✅ Producto eliminado exitosamente junto con sus registros asociados.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Error al eliminar producto: {ex.Message}";
                return RedirectToPage();
            }
        }

        private void CargarDatos()
        {
            try
            {
                ListaCategorias = categoriaDAO.ObtenerCategorias() ?? new List<Categoria>();
                ListaProductos = productoDAO.ObtenerProductosExtendidos() ?? new List<ProductoExtendido>();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Error al cargar datos: {ex.Message}";
                ListaCategorias = new List<Categoria>();
                ListaProductos = new List<ProductoExtendido>();
            }
        }

        // ✅ MÉTODO CORREGIDO: Usar model binding automático en lugar de IFormCollection
        [ValidateAntiForgeryToken]
        public IActionResult OnPostMultiples()
        {
            try
            {
                CargarDatos();

                // ✅ Verificar que se recibieron productos
                if (Productos == null || !Productos.Any())
                {
                    TempData["Error"] = "❌ No se recibieron productos para agregar.";
                    return RedirectToPage();
                }

                // ✅ Filtrar productos válidos (con datos completos)
                var productosValidos = Productos.Where(p =>
                    !string.IsNullOrWhiteSpace(p.ID_Producto) &&
                    !string.IsNullOrWhiteSpace(p.Nombre) &&
                    !string.IsNullOrWhiteSpace(p.Descripcion) &&
                    p.fkID_Categoria.HasValue && p.fkID_Categoria.Value > 0
                ).ToList();

                if (!productosValidos.Any())
                {
                    TempData["Error"] = "❌ No se encontraron productos válidos para agregar.";
                    return RedirectToPage();
                }

                // ✅ Validar productos
                var errores = ValidarProductos(productosValidos);
                if (errores.Any())
                {
                    TempData["Error"] = "❌ Errores encontrados:<br>" + string.Join("<br>", errores);
                    return RedirectToPage();
                }

                // ✅ Verificar duplicados en la base de datos
                var productosExistentes = new List<string>();
                foreach (var producto in productosValidos)
                {
                    if (productoDAO.ExisteProducto(producto.ID_Producto))
                    {
                        productosExistentes.Add(producto.ID_Producto);
                    }
                }

                if (productosExistentes.Any())
                {
                    TempData["Error"] = $"❌ Los siguientes productos ya existen: {string.Join(", ", productosExistentes)}";
                    return RedirectToPage();
                }

                // ✅ Insertar productos
                int productosAgregados = 0;
                foreach (var producto in productosValidos)
                {
                    // ✅ Limpiar espacios en blanco
                    producto.ID_Producto = producto.ID_Producto?.Trim();
                    producto.Nombre = producto.Nombre?.Trim();
                    producto.Descripcion = producto.Descripcion?.Trim();

                    productoDAO.InsertarProducto(producto);
                    productosAgregados++;
                }

                TempData["Mensaje"] = $"✅ {productosAgregados} producto(s) agregados correctamente.";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Error al procesar productos: {ex.Message}";
                return RedirectToPage();
            }
        }

        private List<string> ValidarProductos(List<Producto> productos)
        {
            var errores = new List<string>();
            
            if (productos == null || !productos.Any())
            {
                errores.Add("No se recibieron productos para validar.");
                return errores;
            }

            // ✅ Verificar duplicados en la lista enviada
            var idsEncontrados = new HashSet<string>();
            
            for (int i = 0; i < productos.Count; i++)
            {
                var producto = productos[i];
                
                // Validaciones básicas
                if (string.IsNullOrWhiteSpace(producto.ID_Producto))
                    errores.Add($"El ID del producto #{i + 1} es obligatorio.");
                else if (idsEncontrados.Contains(producto.ID_Producto.Trim().ToUpper()))
                    errores.Add($"El ID '{producto.ID_Producto}' está duplicado en la lista.");
                else
                    idsEncontrados.Add(producto.ID_Producto.Trim().ToUpper());
                    
                if (string.IsNullOrWhiteSpace(producto.Nombre))
                    errores.Add($"El nombre del producto #{i + 1} es obligatorio.");
                    
                if (string.IsNullOrWhiteSpace(producto.Descripcion))
                    errores.Add($"La descripción del producto #{i + 1} es obligatoria.");
                    
                if (!producto.fkID_Categoria.HasValue || producto.fkID_Categoria.Value <= 0)
                    errores.Add($"La categoría del producto #{i + 1} es obligatoria.");

                // ✅ Validar longitud de campos
                if (!string.IsNullOrWhiteSpace(producto.ID_Producto) && producto.ID_Producto.Length > 50)
                    errores.Add($"El ID del producto #{i + 1} no puede exceder 50 caracteres.");
                    
                if (!string.IsNullOrWhiteSpace(producto.Nombre) && producto.Nombre.Length > 100)
                    errores.Add($"El nombre del producto #{i + 1} no puede exceder 100 caracteres.");
            }
            
            return errores;
        }
    }
}