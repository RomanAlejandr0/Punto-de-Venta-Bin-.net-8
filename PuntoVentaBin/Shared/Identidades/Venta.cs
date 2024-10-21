using System.ComponentModel.DataAnnotations;
using PuntoVentaBin.Shared.Identidades.Pedidos;

namespace PuntoVentaBin.Shared.Identidades
{
    public class Venta
    {
        public Venta()
        {
            VentaDetalles = new List<VentaDetalle>();
            PedidoExtension = new PedidoExtension();
        }

        public long NegocioId { get; set; }
        public long UsuarioId { get; set; }
        public long ClienteId { get; set; }
        [Key]
        public long Id { get; set; }
        //public string Folio { get; set; }
        public DateTime FechaHoraVenta { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public bool EsPedido { get; set; } = false;


        //Propiedades que son ignoradas en base de datos:
        public string NombreCliente { get; set; }
        public List<VentaDetalle> VentaDetalles { get; set; } = new List<VentaDetalle>();
        public PedidoExtension PedidoExtension { get; set; }
    }
}
