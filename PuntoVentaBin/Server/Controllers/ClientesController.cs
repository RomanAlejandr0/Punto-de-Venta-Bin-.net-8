using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVentaBin.Shared.Identidades;
using PuntoVentaBin.Shared;
using PuntoVentaBin.Shared.AccesoDatos;
using PuntoVentaBin.Shared.Identidades.DTOs;

namespace PuntoVentaBin.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public ClientesController(ApplicationDbContext context)
        {
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
        
        [HttpGet("GetAll/{empresaId}")]
        public async Task<Respuesta<List<Cliente>>> GetAll(long empresaId)
        {
            var respuesta = new Respuesta<List<Cliente>> { Estado = EstadosDeRespuesta.Correcto };

            try
            {
                respuesta.Datos = await context.Clientes.
                    Where(x => x.NegocioId == empresaId).
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

        [HttpGet("GetClientesDTOs/{empresaId}")]
        public async Task<Respuesta<List<ClienteDTO>>> GetClientesDTOs(long empresaId)
        {
            var respuesta = new Respuesta<List<ClienteDTO>> { Estado = EstadosDeRespuesta.Correcto };

            try
            {
                respuesta.Datos = await context.Clientes.
                    Where(x => x.NegocioId == empresaId).
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
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al guardar el c;iente {cliente.Nombre}";
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
                context.Attach(cliente).State = EntityState.Modified;
                await context.SaveChangesAsync();
                respuesta.Datos = cliente.Id;
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al guardar al actualizar el c;iente {cliente.Nombre}";
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
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al guardar al eliminar el usuario el usuario";
            }

            return respuesta;
        }
    }
}
