namespace PuntoVentaBin.Shared.Identidades.Productos
{
    public class ProductoPaquete
    {
        public long Id { get; set; }
        public long ProductoId { get; set; }
        public string NombreProducto { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Cantidad { get; set; }
        public Producto Producto { get; set; }

    }
}
