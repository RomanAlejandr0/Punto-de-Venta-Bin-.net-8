using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntoVentaBin.Shared.LogServices
{
    public class ErrorLog
    {
        [Key]
        public long Id { get; set; }
        public string Proceso { get; set; } = string.Empty;
        public string? Mensaje { get; set; } = string.Empty;
        public string? ExceptionMessage { get; set; }
        public string? StackTrace { get; set; }
        public DateTime Fecha { get; set; }
    }
}
