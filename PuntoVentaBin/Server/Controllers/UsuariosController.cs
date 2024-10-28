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

        [HttpGet("GetAll/{negocioId}")]
        public async Task<Respuesta<List<Usuario>>> GetAll(long negocioId)
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
        [Route("{action}")]
        public async Task<Respuesta<bool>> EliminarUsuario([FromBody] long usuarioId)
        {
            var respuesta = new Respuesta<bool> { Estado = EstadosDeRespuesta.Correcto, Mensaje = "Tu cuenta se elimino correctamente" };

            try
            {



                var usuariosRolesNegocios = await context.UsuariosRolesNegocios.Where(x => x.UsuarioId == usuarioId).ToListAsync();
                context.UsuariosRolesNegocios.RemoveRange(usuariosRolesNegocios);
                await context.SaveChangesAsync();


                var usuarioBorrado = await context.UsuariosBin.FirstOrDefaultAsync(x => x.Id == usuarioId);
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
