using Microsoft.JSInterop;

namespace PuntoVentaBin.Client.Helpers
{
    public class MostrarMensajes : IMostrarMensajes
    {
        private readonly IJSRuntime js;

        public MostrarMensajes(IJSRuntime js)
        {
            this.js = js;
        }

        public async Task MostrarMensajeError(string mensaje)
        {
            await MostrarMensaje("Error", mensaje, "error");
        }

        public async Task MostrarMensajeInformacion(string mensaje)
        {
            await MostrarMensaje("Informacion", mensaje, "info");
        }

        public async Task MostrarMensajeAdvertencia(string mensaje)
        {
            await MostrarMensaje("Advertencia", mensaje, "warning");

        }

        public async Task MostrarMensajeExitoso(string mensaje)
        {
            await MostrarMensaje("Exitoso", mensaje, "success");
        }

        private async ValueTask MostrarMensaje(string titulo, string mensaje, string tipoMensaje)
        {
            await js.InvokeVoidAsync("Swal.fire", titulo, mensaje, tipoMensaje);
        }

    }
}
