
using PuntoVentaBin.Shared.AccesoDatos;
using PuntoVentaBin.Shared.Identidades;
using System.Diagnostics;
using System.Reflection.PortableExecutable;

namespace PuntoVentaBin.Shared.LogServices
{
    public class LogService : ILogService
    {
        private readonly ApplicationDbContext _context;

        public LogService(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task LogAsync(string proceso, string mensaje, long negocioId)
        {
            var Log = new Log
            {
                Usuario = "Anonimo",
                NegocioId = negocioId,
                Proceso = proceso,
                Mensaje = mensaje,
                Fecha = DateTime.Now
            };

            _context.TablaLogs.Add(Log);
            await _context.SaveChangesAsync();
        }


        public async Task ErrorLogAsync(string proceso, string mensaje, string? exceptionMessage, string? stackTrace)
        {
            var ErrorLog = new ErrorLog
            {
                Proceso = proceso,
                Mensaje = mensaje,
                ExceptionMessage = exceptionMessage,
                StackTrace = stackTrace,
                Fecha = DateTime.Now
            };

            try
            {
                _context.TablaErroresLogs.Add(ErrorLog);
            }
            catch (Exception ex)
            {

                Debug.WriteLine("hubo un error al guadar el error log");
            }
                await _context.SaveChangesAsync();

        }

    }
}
