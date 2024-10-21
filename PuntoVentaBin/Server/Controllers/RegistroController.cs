using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVentaBin.Shared;
using PuntoVentaBin.Shared.AccesoDatos;
using PuntoVentaBin.Shared.Identidades;
using System.Net.Mail;
using System.Net;

namespace PuntoVentaBin.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistroController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IConfiguration configuration;

        public RegistroController(ApplicationDbContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }


        [HttpPost]
        [Route("{action}")]
        public async Task<Respuesta<long>> RealizarRegistroUsuario([FromBody] UsuarioBin value)
        {
            var respuesta = new Respuesta<long> { Estado = EstadosDeRespuesta.Correcto };
            var transaction = context.Database.BeginTransaction();

            try
            {
                var usuarioExistente = await context.UsuariosBin.FirstOrDefaultAsync(x => x.Email == value.Email);

                if (usuarioExistente != null)
                {
                    if (usuarioExistente.CuentaActivada == true)
                    {
                        // La cuenta esta registrada y está activada (Pedirle al usuario que inicie sesion)
                        respuesta.Datos = 0;
                        respuesta.Mensaje = "Este correo electrónico ya está registrado y la cuenta esta activada. " +
                            "Por favor, inicia sesión.";
                    }
                    else
                    {
                        // La cuenta esta regitrada pero no está activada
                        // (Informar al usuario que ya tiene un correo de confirmación para verificar su cuenta)
                        respuesta.Datos = 0;
                        respuesta.Mensaje = "Tu correo ya esta registrado pero tu cuenta aún no ha sido activada. " +
                            "Por favor revisa tu correo electrónico para activarla.";
                    }
                }
                else
                {
                    // La cuenta no esta registrada y por lo tanto no activada
                    // (Registrar Cuenta y enviar correo de confirmacion)
                    
                    value.FechaRegistro = DateTime.Now;
                    value.TokenConfirmacion = Guid.NewGuid().ToString();
                    value.CuentaActivada = false; // La cuenta aún no está activada
                    value.NegocioId = 1014;
                    value.RolId = 1;

                    if (!TryValidateModel(value))
                    {
                        respuesta.Estado = EstadosDeRespuesta.Error;
                        respuesta.Mensaje = $"Error de validación de datos";
                        return respuesta;
                    }

                    context.UsuariosBin.Add(value);
                    await context.SaveChangesAsync(true);

                    var empleado = new Empleado { usuarioId = value.Id ,RolId = 1, Nombre= value.Nombre };
                    context.Empleados.Add(empleado);
                    await context.SaveChangesAsync(true);


                    // Enviar correo de confirmación
                    try
                    {
                        await EnviarCorreoConfirmacion(value.Email, value.Nombre, value.TokenConfirmacion);
                        respuesta.Datos = value.Id;
                        respuesta.Mensaje = "Usuario guardado en la base de datos. Se ha enviado un correo de confirmación.";
                    }
                    catch (Exception ex)
                    {
                        respuesta.Estado = EstadosDeRespuesta.Error;
                        respuesta.Mensaje = $"Error al enviar el correo de confirmación: {ex.Message}";
                        transaction.Rollback();
                        return respuesta;
                    }
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Ocurrio el siguiente error {ex.InnerException}";
            }

            return respuesta;
        }



        private async Task EnviarCorreoConfirmacion(string emailDestino, string nombreUsuario, string tokenConfirmacion)
        {
            // Obtener la configuración SMTP desde appsettings.json
            var smtpConfig = configuration.GetSection("Smtp");
            var fromAddress = new MailAddress(smtpConfig["User"], "Punto de Venta Bin");
            var toAddress = new MailAddress(emailDestino, nombreUsuario);
            string fromPassword = smtpConfig["Password"];
            string subject = "Confirmación de registro";

            // Construir la URL de confirmación con el dominio público
            //string urlConfirmacion = $"{Request.Scheme}://{Request.Host}/api/Registro/ConfirmarCorreo?token={tokenConfirmacion}";
            string urlConfirmacion = $"http://arturhc-001-site13.atempurl.com/api/Registro/ConfirmarCorreo?token={tokenConfirmacion}";

            // Cuerpo del correo electrónico
            string body = $"Hola {nombreUsuario},\n\nGracias por registrarte. " +
                $"Por favor, confirma tu correo electrónico haciendo clic en el " +
                $"siguiente enlace:\n\n{urlConfirmacion}\n\nSi no solicitaste esta acción, puedes ignorar este correo.";

            // Configurar el cliente SMTP
            var smtp = new SmtpClient
            {
                Host = smtpConfig["Host"],
                Port = int.Parse(smtpConfig["Port"]),
                //EnableSsl = bool.Parse(smtpConfig["EnableSsl"]),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false, 
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                Timeout = 20000
            };

            // Crear y enviar el mensaje de correo electrónico
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                await smtp.SendMailAsync(message);
                //try
                //{
                //    await smtp.SendMailAsync(message);
                //}
                //catch (SmtpException ex)
                //{
                //    // Manejo de errores SMTP
                //    Console.WriteLine($"Error al enviar el correo: {ex.Message}");
                //    // Puedes implementar lógica adicional o reintentos si es necesario
                //}
                //catch (Exception ex)
                //{
                //    // Manejo de errores generales
                //    Console.WriteLine($"Ocurrió un error inesperado: {ex.Message}");
                //}
            }
        }


        [HttpGet("ConfirmarCorreo")]
        public async Task<IActionResult> ConfirmarCorreo(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return Redirect("/token-invalido");
            }

            var usuario = await context.UsuariosBin.FirstOrDefaultAsync(x => x.TokenConfirmacion == token);

            if (usuario == null)
            {
                return Redirect("/token-invalido");
            }

            if (usuario.CuentaActivada)
            {
                usuario.TokenConfirmacion = null;
                await context.SaveChangesAsync();
                return Redirect("/cuenta-ya-activada");
            }

            usuario.CuentaActivada = true;
            usuario.TokenConfirmacion = null;
            await context.SaveChangesAsync();

            return Redirect("/confirmacion-exitosa");
        }


        
    }
}
