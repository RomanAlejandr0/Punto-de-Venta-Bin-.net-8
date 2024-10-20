using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVentaBin.Shared;
using PuntoVentaBin.Shared.AccesoDatos;
using PuntoVentaBin.Shared.Identidades;
using PuntoVentaBin.Shared.Identidades.Productos;

namespace PuntoVentaBin.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NegociosController : ControllerBase
    {
		private readonly ApplicationDbContext context;

		public NegociosController(ApplicationDbContext context)
		{
			this.context = context;
		}

        [HttpPost]
        [Route("{action}")]
        public async Task<Respuesta<long>> GuardarNegocio([FromBody] Negocio negocio)
        {
            var respuesta = new Respuesta<long> { Estado = EstadosDeRespuesta.Correcto, Mensaje = "Guardado Correctamente" };
            var transaction = context.Database.BeginTransaction();

            try
            {
                negocio.FechaRegistro = DateTime.Now;

                //negocio.Usuarios.Last().RolId = 1;
                //negocio.Usuarios.Last().FechaRegistro = DateTime.Now;
                
                negocio.Clientes.Last().Nombre = "Cliente Generico";
                negocio.Clientes.Last().Email = "";

                context.Negocios.Add(negocio);
                
                await context.SaveChangesAsync(true);

                respuesta.Datos = negocio.Id;

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Ocurrio un error al guardar la empresa";
            }
            return respuesta;
        }

	}
}