﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVentaBin.Server;
using PuntoVentaBin.Shared;
using PuntoVentaBin.Shared.AccesoDatos;
using PuntoVentaBin.Shared.Identidades;
using PuntoVentaBin.Shared.Identidades.Productos;
using PuntoVentaBin.Shared.LogServices;
using System.Text.Json;

namespace PuntoVentaBIN.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ProvedoresController : ControllerBase
	{
		private readonly ApplicationDbContext context;
        private readonly ILogService _logService;

        public ProvedoresController(ApplicationDbContext context, ILogService logService)
		{
			this.context = context;
            _logService = logService;
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

		[HttpGet("GetAll/{negocioId}")]
		public async Task<Respuesta<List<Provedor>>> GetAll(long negocioId)
		{
			var respuesta = new Respuesta<List<Provedor>> { Estado = EstadosDeRespuesta.Correcto };

			try
			{
				respuesta.Datos = await context.Provedores.
					Where(x => x.NegocioId == negocioId).
					OrderBy(x => x.Id).
					AsNoTracking().
					ToListAsync();
			}
			catch (Exception ex)
			{
				respuesta.Estado = EstadosDeRespuesta.Error;
				respuesta.Mensaje = $"Error al consultar los provedores";
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

                var objetoSerializado = JsonSerializer.Serialize(provedor);
                await _logService.LogAsync("Guardar Provedor", $"Nuevo provedor: {objetoSerializado}", provedor.NegocioId);
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
                var registroAnterior = await context.Provedores.AsNoTracking().FirstOrDefaultAsync(x => x.Id == provedor.Id);
                var objetoSerializadoAnterior = JsonSerializer.Serialize(registroAnterior);

                context.Attach(provedor).State = EntityState.Modified;
				await context.SaveChangesAsync();
				respuesta.Datos = provedor.Id;

                var objetoSerializado = JsonSerializer.Serialize(provedor);
                await _logService.LogAsync("Editar Provedor", $"Provedor editado de {objetoSerializadoAnterior} a {objetoSerializado}.", provedor.NegocioId);
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

                var objetoSerializado = JsonSerializer.Serialize(provedor);
                await _logService.LogAsync("Eliminar Provedor", $"Provedor eliminado {objetoSerializado}.", provedor.NegocioId);
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
