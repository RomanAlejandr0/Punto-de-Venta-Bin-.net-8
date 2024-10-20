namespace PuntoVentaBin.Shared.Identidades.Pedidos
{
    public class PedidoExtension
    {
        public long Id { get; set; }
        public long EmpresaId { get; set; }
        public long VentaId { get; set; }
        public DateTime FechaEntrega { get; set; } = DateTime.Now;
        public string LugarEntrega { get; set; }
        public string Anotaciones { get; set; }
        public bool Entregado { get; set; }
        public bool Pagado { get; set; }
        public bool Activo { get; set; }

        //public decimal MontoCubierto { get; set; }
        //public decimal MontoRestante { get; set; }
        //public List<Pago> Pagos { get; set; }
    }
}
