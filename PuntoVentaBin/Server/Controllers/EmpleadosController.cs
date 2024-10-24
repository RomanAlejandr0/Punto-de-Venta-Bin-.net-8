using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVentaBin.Shared;
using PuntoVentaBin.Shared.AccesoDatos;
using PuntoVentaBin.Shared.Identidades;
using PuntoVentaBin.Shared.Identidades.Adm_PerfilTareas;
using System.Runtime.InteropServices;

namespace PuntoVentaBin.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpleadosController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public EmpleadosController(ApplicationDbContext context)
        {
            this.context = context;
        }


        [HttpGet("{negocioId}")]
        public async Task<Respuesta<List<UsuarioRolNegocio>>> Get(long negocioId)
        {
            var respuesta = new Respuesta<List<UsuarioRolNegocio>> { Estado = EstadosDeRespuesta.Correcto };

            try
            {
                var usuariosRolesNegocios = new List<UsuarioRolNegocio>();


                // Paso 1: Obtener los usuarios asociados al negocio junto con sus roles
                var usuariosConRoles = await context.UsuariosRolesNegocios
                    .Where(un => un.NegocioId == negocioId)
                    .Select(un => new
                    {
                        Usuario = un.Usuario,
                        RolId = un.RolId
                    })
                    .AsNoTracking()
                    .ToListAsync();

                foreach (var usuarioConRol in usuariosConRoles)
                {
                    usuarioConRol.Usuario.Permisos = await context.RolesPermisos.
                        Where(x => x.RolID == usuarioConRol.RolId).
                        Select(x => x.Permiso).
                        ToListAsync();

                    usuariosRolesNegocios.Add(new UsuarioRolNegocio { Usuario = usuarioConRol.Usuario, RolId = usuarioConRol.RolId });
                }


                respuesta.Datos = usuariosRolesNegocios;

            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al consultar el usuario";
            }

            return respuesta;
        }


        //[HttpPost]
        //public async Task<Respuesta<long>> CrearUsuario(UsuarioRolNegocio usuar)
        //{
        //    var respuesta = new Respuesta<long> { Estado = EstadosDeRespuesta.Correcto };
        //    try
        //    {
        //        context.Add(usuario);
        //        await context.SaveChangesAsync();
        //        respuesta.Datos = usuario.Id;
        //    }
        //    catch (Exception ex)
        //    {
        //        respuesta.Estado = EstadosDeRespuesta.Error;
        //        respuesta.Mensaje = $"Error al guardar el usuario {usuario.Nombre}";
        //    }
        //    return respuesta;
        //}


    }
}
