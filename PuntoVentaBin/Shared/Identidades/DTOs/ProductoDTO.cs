namespace PuntoVentaBin.Shared.Identidades.DTOs
{
    public class ProductoDTO
    {
        public long Id { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal CantidadInventario { get; set; }
        public decimal CantidadSumadaInventario { get; set; }
        public string CodigoBarras { get; set; }
    }
}
