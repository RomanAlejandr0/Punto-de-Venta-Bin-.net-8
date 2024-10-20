using System.ComponentModel.DataAnnotations;

namespace PuntoVentaBin.Shared.Identidades
{
    public class VentaDetalle
    {
        public long EmpresaId { get; set; }
        public long VentaId { get; set; }
        [Key]
        public long Id { get; set; }
        public long ProductoId { get; set; }
        public string DescripcionProducto { get; set; }

        private decimal _cantidad;
        public decimal Cantidad
        {
            get
            {
                return _cantidad;
            }
            set
            {
                this.Total = value * Precio;
                _cantidad = value;
            }
        }
        public decimal Precio { get; set; }
        public decimal Total { get; set; }
        public Venta Venta { get; set; }

    }
}
