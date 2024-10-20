namespace PuntoVentaBin.Client.Helpers
{
    public interface IMostrarMensajes
    {
        Task MostrarMensajeError(string mensaje);
        Task MostrarMensajeInformacion(string mensaje);
        Task MostrarMensajeAdvertencia(string mensaje);
        Task MostrarMensajeExitoso(string mensaje);
    }
}