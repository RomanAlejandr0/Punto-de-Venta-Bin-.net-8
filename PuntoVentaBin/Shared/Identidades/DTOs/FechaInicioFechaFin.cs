using PuntoVentaBin.Shared.Identidades;

namespace PuntoVentaBin.Shared.Identidades.DTOs
{
    public class FechaInicioFechaFin
    {
        public long NegocioId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public List<Venta>? Ventas { get; set; }
    }
}
