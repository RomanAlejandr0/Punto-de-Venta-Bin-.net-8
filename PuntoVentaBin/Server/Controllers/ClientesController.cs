using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVentaBin.Shared.Identidades;
using PuntoVentaBin.Shared;
using PuntoVentaBin.Shared.AccesoDatos;
using PuntoVentaBin.Shared.Identidades.DTOs;
using PuntoVentaBin.Shared.LogServices;
using System.Text.Json;

namespace PuntoVentaBin.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly ApplicationDbContext context;

        public ClientesController(ApplicationDbContext context, ILogService logService)
        {
            _logService = logService;
            this.context = context;
        }

        [HttpGet("{id}")]
        public async Task<Respuesta<Cliente>> Get(long id)
        {
            var respuesta = new Respuesta<Cliente> { Estado = EstadosDeRespuesta.Correcto };

            try
            {
                respuesta.Datos = await context.Clientes.
                    FirstOrDefaultAsync(x => x.Id == id);
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al consultar el cliente";
            }

            return respuesta;
        }
        
        [HttpGet("GetAll/{negocioId}")]
        public async Task<Respuesta<List<Cliente>>> GetAll(long negocioId)
        {
            var respuesta = new Respuesta<List<Cliente>> { Estado = EstadosDeRespuesta.Correcto };

            try
            {
                respuesta.Datos = await context.Clientes.
                    Where(x => x.NegocioId == negocioId).
                    OrderBy(x => x.Nombre).
                    ToListAsync();
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al consultar los clientes";
            }

            return respuesta;
        }

        [HttpGet("GetClientesDTOs/{negocioId}")]
        public async Task<Respuesta<List<ClienteDTO>>> GetClientesDTOs(long negocioId)
        {
            var respuesta = new Respuesta<List<ClienteDTO>> { Estado = EstadosDeRespuesta.Correcto };

            try
            {
                respuesta.Datos = await context.Clientes.
                    Where(x => x.NegocioId == negocioId).
                    Select(a => new ClienteDTO
                    {
                        Id = a.Id,
                        Nombre = a.Nombre
                    }).
                    AsNoTracking().
                    ToListAsync();
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al consultar los clientes";
            }

            return respuesta;
        }

        [HttpPost]
        [Route("{action}")]
        public async Task<Respuesta<long>> CrearCliente([FromBody] Cliente cliente)
        {
            var respuesta = new Respuesta<long> { Estado = EstadosDeRespuesta.Correcto };

            try
            {
                context.Add(cliente);
                await context.SaveChangesAsync();
                respuesta.Datos = cliente.Id;

                var objetoSerializado = JsonSerializer.Serialize(cliente);
                await _logService.LogAsync("Guardar Cliente", $"Nuevo cliente: {objetoSerializado}",cliente.NegocioId);
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al guardar el cliente {cliente.Nombre}";
            }

            return respuesta;
        }

        [HttpPost]
        [Route("{action}")]
        public async Task<Respuesta<long>> EditarCliente([FromBody] Cliente cliente)
        {
            var respuesta = new Respuesta<long> { Estado = EstadosDeRespuesta.Correcto };

            try
            {
                var registroAnterior = await context.Clientes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == cliente.Id);
                var objetoSerializadoAnterior = JsonSerializer.Serialize(registroAnterior);

                context.Attach(cliente).State = EntityState.Modified;
                await context.SaveChangesAsync();
                respuesta.Datos = cliente.Id;

                var objetoSerializado = JsonSerializer.Serialize(cliente);
                await _logService.LogAsync("Editar Cliente", $"Provedor cliente de {objetoSerializadoAnterior} a {objetoSerializado}.", cliente.NegocioId);
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al guardar al actualizar el cliente {cliente.Nombre}";
            }

            return respuesta;
        }

        [HttpPost]
        [Route("{action}")]
        public async Task<Respuesta<long>> EliminarCliente([FromBody] Cliente cliente)
        {
            var respuesta = new Respuesta<long> { Estado = EstadosDeRespuesta.Correcto };

            try
            {
                var clienteBorrado = await context.Clientes.
                    FirstOrDefaultAsync(x => x.Id == cliente.Id);
                context.Attach(clienteBorrado).State = EntityState.Deleted;
                await context.SaveChangesAsync();

                var objetoSerializado = JsonSerializer.Serialize(cliente);
                await _logService.LogAsync("Eliminar Cliente", $"Cliente eliminado {objetoSerializado}.", cliente.NegocioId);
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al guardar al eliminar el usuario el cliente {cliente.Nombre}";
            }

            return respuesta;
        }
    }
}
