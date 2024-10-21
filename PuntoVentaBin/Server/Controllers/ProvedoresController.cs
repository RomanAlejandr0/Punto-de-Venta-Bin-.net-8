using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVentaBin.Server;
using PuntoVentaBin.Shared;
using PuntoVentaBin.Shared.AccesoDatos;
using PuntoVentaBin.Shared.Identidades;

namespace PuntoVentaBIN.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProvedoresController : ControllerBase
	{
		private readonly ApplicationDbContext context;

		public ProvedoresController(ApplicationDbContext context)
		{
			this.context = context;
		}

		[HttpGet("{id}")]
		public async Task<Respuesta<Provedor>> Get(long id)
		{
			var respuesta = new Respuesta<Provedor> { Estado = EstadosDeRespuesta.Correcto };

			try
			{
				respuesta.Datos = await context.Provedores.
					AsNoTracking().
					FirstOrDefaultAsync(x => x.Id == id);
			}
			catch (Exception ex)
			{
				respuesta.Estado = EstadosDeRespuesta.Error;
				respuesta.Mensaje = $"Error al consultar el usuario";
			}

			return respuesta;
		}

		[HttpGet("GetAll/{empresaId}")]
		public async Task<Respuesta<List<Provedor>>> GetAll(long empresaId)
		{
			var respuesta = new Respuesta<List<Provedor>> { Estado = EstadosDeRespuesta.Correcto };

			try
			{
				respuesta.Datos = await context.Provedores.
					Where(x => x.NegocioId == empresaId).
					OrderBy(x => x.Id).
					AsNoTracking().
					ToListAsync();
			}
			catch (Exception ex)
			{
				respuesta.Estado = EstadosDeRespuesta.Error;
				respuesta.Mensaje = $"Error al consultar los usuarios";
			}

			return respuesta;
		}

		[HttpPost]
		[Route("{action}")]
		public async Task<Respuesta<long>> CrearProvedor([FromBody] Provedor provedor)
		{
			var respuesta = new Respuesta<long> { Estado = EstadosDeRespuesta.Correcto };
			try
			{
				context.Add(provedor);
				await context.SaveChangesAsync();
				respuesta.Datos = provedor.Id;
			}
			catch (Exception ex)
			{
				respuesta.Estado = EstadosDeRespuesta.Error;
				respuesta.Mensaje = $"Error al guardar el usuario {provedor.Nombre}";
			}
			return respuesta;
		}

		[HttpPost]
		[Route("{action}")]
		public async Task<Respuesta<long>> EditarProvedor([FromBody] Provedor provedor)
		{
			var respuesta = new Respuesta<long> { Estado = EstadosDeRespuesta.Correcto };

			try
			{
				context.Attach(provedor).State = EntityState.Modified;
				await context.SaveChangesAsync();
				respuesta.Datos = provedor.Id;
			}
			catch (Exception ex)
			{
				respuesta.Estado = EstadosDeRespuesta.Error;
				respuesta.Mensaje = $"Error al guardar al actualizar el usuario {provedor.Nombre}";
			}

			return respuesta;
		}

		[HttpPost]
		[Route("{action}")]
		public async Task<Respuesta<long>> EliminarProvedor([FromBody] Provedor provedor)
		{
			var respuesta = new Respuesta<long> { Estado = EstadosDeRespuesta.Correcto };

			try
			{
				context.Remove(provedor);
				//var provedorBorrado = await context.Provedores.FirstOrDefaultAsync(x => x.Id == provedor.Id);
				//context.Attach(provedorBorrado).State = EntityState.Deleted;
				await context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				respuesta.Estado = EstadosDeRespuesta.Error;
				respuesta.Mensaje = $"Error al guardar al eliminar el usuario";
			}

			return respuesta;
		}
	}
}
