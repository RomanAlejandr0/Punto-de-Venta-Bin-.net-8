
using PuntoVentaBin.Shared.AccesoDatos;

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

   
    }
}
