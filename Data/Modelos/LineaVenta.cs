using System.Collections.Generic;
using System.Linq;

namespace ProyectoProcesosExperienciaV1.Data.Modelos
{
    public class ListaVenta
    {
        public List<ProductoVenta> Productos { get; set; } = new();

        public decimal Total => Productos.Sum(p => p.Subtotal);

        public void AgregarProducto(ProductoVenta nuevoProducto)
        {
            var existente = Productos.FirstOrDefault(p => p.ID_Producto == nuevoProducto.ID_Producto);
            if (existente != null)
            {
                existente.Cantidad += nuevoProducto.Cantidad;
            }
            else
            {
                Productos.Add(nuevoProducto);
            }
        }

        public void Limpiar() => Productos.Clear();
    }
}
