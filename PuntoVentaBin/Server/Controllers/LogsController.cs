using Microsoft.AspNetCore.Mvc;
using PuntoVentaBin.Shared;
using PuntoVentaBin.Shared.AccesoDatos;
using PuntoVentaBin.Shared.Identidades.DTOs;
using PuntoVentaBin.Shared.Identidades;
using PuntoVentaBin.Shared.LogServices;
using Microsoft.EntityFrameworkCore;

namespace PuntoVentaBin.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController: ControllerBase
    {
        private readonly ApplicationDbContext context;

        public LogsController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpPost("GetLogs")]
        public async Task<Respuesta<List<Log>>> GetLogs(FechaInicioFechaFin periodoFecha)
        {
            var respuesta = new Respuesta<List<Log>> { Estado = EstadosDeRespuesta.Correcto };

            List<Log> Logs = new List<Log>();

            var tsInicio = new TimeSpan(00, 00, 01);
            periodoFecha.FechaInicio = periodoFecha.FechaInicio.Date + tsInicio;

            var tsFin = new TimeSpan(23, 59, 59);
            periodoFecha.FechaFin = periodoFecha.FechaFin.Date + tsFin;

            try
            {
                Logs = await context.TablaLogs.
                    Where(x => x.NegocioId == periodoFecha.NegocioId &&
                        x.Fecha >= periodoFecha.FechaInicio &&
                        x.Fecha <= periodoFecha.FechaFin).
                    OrderByDescending(x => x.Fecha).
                    AsNoTracking().
                    ToListAsync();

                respuesta.Datos = Logs;

            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al consultar las ventas";
            }

            return respuesta;
        }
    }
}
