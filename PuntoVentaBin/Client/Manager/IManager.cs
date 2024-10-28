using PuntoVentaBin.Shared;

namespace PuntoVentaBin.Client.Repositorios
{
    public interface IManager
    {
        Task<Respuesta<R>> Post<T, R>(string url, T enviar);

        Task<Respuesta<T>> Get<T>(string url);

        Task<Respuesta<string>> PostString<T>(string url, T enviar);
    }
}
