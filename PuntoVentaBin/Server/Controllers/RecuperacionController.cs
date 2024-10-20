using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVentaBin.Shared;
using PuntoVentaBin.Shared.AccesoDatos;
using PuntoVentaBin.Shared.Identidades;
using System.Net;
using System.Net.Mail;

[ApiController]
[Route("api/[controller]")]
public class RecuperacionController : ControllerBase
{
    private readonly ApplicationDbContext context;
    private readonly IConfiguration configuration;

    public RecuperacionController(ApplicationDbContext context, IConfiguration configuration)
    {
        this.context = context;
        this.configuration = configuration;
    }

    [HttpPost("EnviarTokenRecuperacion")]
    public async Task<Respuesta<bool>> EnviarTokenRecuperacion([FromBody] EmailModel model)
    {
        var respuesta = new Respuesta<bool> { Estado = EstadosDeRespuesta.Correcto };

        try
        {
            var usuario = await context.UsuariosBin.FirstOrDefaultAsync(x => x.Email == model.Email);

            if (usuario != null)
            {
                // Generar un token de recuperación
                usuario.TokenRecuperacion = Guid.NewGuid().ToString();
                usuario.FechaExpiracionTokenRecuperacion = DateTime.Now.AddHours(1); // Token válido por 1 hora

                await context.SaveChangesAsync();

                // Enviar el correo electrónico con el token
                await EnviarCorreoRecuperacion(usuario.Email, usuario.Nombre, usuario.TokenRecuperacion);
            }

            // Por seguridad, siempre responder que se envió el correo, aunque el usuario no exista
            respuesta.Datos = true;
            respuesta.Mensaje = "Si el correo electrónico existe, se ha enviado un enlace para restablecer la contraseña.";
        }
        catch (Exception ex)
        {
            respuesta.Estado = EstadosDeRespuesta.Error;
            respuesta.Mensaje = $"Ocurrió un error: {ex.Message}";
        }

        return respuesta;
    }

    private async Task EnviarCorreoRecuperacion(string emailDestino, string nombreUsuario, string tokenRecuperacion)
    {
        var smtpConfig = configuration.GetSection("Smtp");
        var fromAddress = new MailAddress(smtpConfig["User"], "Punto Venta Bin");
        var toAddress = new MailAddress(emailDestino, nombreUsuario);
        string fromPassword = smtpConfig["Password"];
        string subject = "Recuperación de contraseña";

        //string urlRecuperacion = $"{Request.Scheme}://{Request.Host}/restablecer-contraseña?token={tokenRecuperacion}";
        string urlRecuperacion = $"http://arturhc-001-site13.atempurl.com/restablecer-contraseña?token={tokenRecuperacion}";

        string body = $"Hola {nombreUsuario},\n\n" +
                      $"Has solicitado restablecer tu contraseña. Por favor, haz clic en el siguiente enlace para continuar:\n\n" +
                      $"{urlRecuperacion}\n\n" +
                      $"Este enlace es válido por 1 hora.\n\n" +
                      $"Si no solicitaste este cambio, puedes ignorar este correo.";

        var smtp = new SmtpClient
        {
            Host = smtpConfig["Host"],
            Port = int.Parse(smtpConfig["Port"]),
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
            Timeout = 20000
        };

        using var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body
        };
        await smtp.SendMailAsync(message);
    }

    [HttpPost("RestablecerContraseña")]
    public async Task<Respuesta<bool>> RestablecerContraseña([FromBody] ResetPasswordModel model)
    {
        var respuesta = new Respuesta<bool> { Estado = EstadosDeRespuesta.Correcto };

        try
        {
            var usuario = await context.UsuariosBin.FirstOrDefaultAsync(u => u.TokenRecuperacion == model.Token);

            if (usuario == null || usuario.FechaExpiracionTokenRecuperacion < DateTime.Now)
            {
                respuesta.Datos = false;
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = "El token es inválido o ha expirado.";
                return respuesta;
            }

            // Actualizar la contraseña del usuario
            usuario.Password = model.NuevaContraseña; // Asegúrate de hashear la contraseña
            usuario.TokenRecuperacion = null;
            usuario.FechaExpiracionTokenRecuperacion = null;

            await context.SaveChangesAsync();

            respuesta.Datos = true;
            respuesta.Mensaje = "Contraseña restablecida exitosamente.";
        }
        catch (Exception ex)
        {
            respuesta.Estado = EstadosDeRespuesta.Error;
            respuesta.Mensaje = $"Ocurrió un error: {ex.Message}";
        }

        return respuesta;
    }
}
