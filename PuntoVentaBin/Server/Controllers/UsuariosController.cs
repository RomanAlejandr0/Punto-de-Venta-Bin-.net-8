using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVentaBin.Shared;
using PuntoVentaBin.Shared.AccesoDatos;
using PuntoVentaBin.Shared.Identidades;

namespace PuntoVentaBIN.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public UsuariosController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet("{id}")]
        public async Task<Respuesta<Usuario>> Get(long id)
        {
            var respuesta = new Respuesta<Usuario> { Estado = EstadosDeRespuesta.Correcto };

            try
            {
                respuesta.Datos = await context.UsuariosBin.
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
        public async Task<Respuesta<List<Usuario>>> GetAll(long empresaId)
        {
            var respuesta = new Respuesta<List<Usuario>> { Estado = EstadosDeRespuesta.Correcto };

            try
            {
                //respuesta.Datos = await context.UsuariosBin.
                //    Where(x => x.NegocioId == empresaId).
                //    OrderBy(x => x.Id).
                //    AsNoTracking().
                //    ToListAsync();
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al consultar los usuarios";
            }

            return respuesta;
        }


        [HttpPost]
        public async Task<Respuesta<long>> CrearUsuario(Usuario usuario)
        {
            var respuesta = new Respuesta<long> { Estado = EstadosDeRespuesta.Correcto };
            try
            {
                context.Add(usuario);
                await context.SaveChangesAsync();
                respuesta.Datos = usuario.Id;
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al guardar el usuario {usuario.Nombre}";
            }
            return respuesta;
        }

        [HttpPost]
        public async Task<Respuesta<long>> EditarUsuario(Usuario usuario)
        {
            var respuesta = new Respuesta<long> { Estado = EstadosDeRespuesta.Correcto };

            try
            {
                context.Attach(usuario).State = EntityState.Modified;
                await context.SaveChangesAsync();
                respuesta.Datos = usuario.Id;
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al guardar al actualizar el usuario {usuario.Nombre}";
            }

            return respuesta;

        }

        [HttpPost]
        public async Task<Respuesta<long>> EliminarUsuario(Usuario usuario)
        {
            var respuesta = new Respuesta<long> { Estado = EstadosDeRespuesta.Correcto };
            var _usuario = usuario;

            try
            {
                var usuarioBorrado = await context.UsuariosBin.FirstOrDefaultAsync(x => x.Id == _usuario.Id);
                context.Attach(usuarioBorrado).State = EntityState.Deleted;
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
