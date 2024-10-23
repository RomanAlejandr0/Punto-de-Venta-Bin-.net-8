namespace PuntoVentaBin.Shared.LogServices
{
    public interface ILogService
    {
        Task LogAsync(string proceso, string mensaje, long negocioId);
    }
}
