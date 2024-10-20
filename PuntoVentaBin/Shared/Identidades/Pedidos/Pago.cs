namespace PuntoVentaBin.Shared.Identidades.Pedidos
{
    public class Pago
    {
        public long Id { get; set; }
        public long PedidoExtensionId { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaPago { get; set; }
    }
}
