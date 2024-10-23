using System.ComponentModel.DataAnnotations;

namespace PuntoVentaBin.Shared.Identidades.Productos
{
    public class Producto
    {
        [Key]
        public long Id { get; set; }
        public long NegocioId { get; set; }
        [Required(ErrorMessage = "El nombre del producto es requerido.")]
        public string Descripcion { get; set; }
        public decimal CantidadInventario { get; set; }
        public decimal PrecioCompra { get; set; }
        [Required(ErrorMessage = "El precio venta es requerido.")]
        public decimal PrecioVenta { get; set; }
        public string? Categoria { get; set; }
        public string? CodigoBarras { get; set; }
        [Required(ErrorMessage = "El presentacion es requerida.")]
        public string Presentacion { get; set; } //Unidad, a granel o en paquete
        //public bool? EsPaquete { get; set; } = false; //Unidad, a granel o en paquete


        //Propiedades que son ignoradas en base de datos:
        //public List<ProductoPaquete> ProductosPaquete { get; set; } = new List<ProductoPaquete>();

    }
}
