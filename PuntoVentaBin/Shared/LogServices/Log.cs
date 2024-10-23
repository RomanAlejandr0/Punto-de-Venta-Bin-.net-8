using System.ComponentModel.DataAnnotations;

namespace PuntoVentaBin.Shared.LogServices
{
    public class Log
    {
            [Key]
            public long Id { get; set; }
            public long NegocioId { get; set; }
            public string Usuario { get; set; } = string.Empty;
            public string Proceso { get; set; } = string.Empty;
            public string? Mensaje { get; set; }
            public DateTime Fecha { get; set; }
    }
}
