using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuntoVentaBin.Shared;
using PuntoVentaBin.Shared.AccesoDatos;
using PuntoVentaBin.Shared.Identidades;
using PuntoVentaBin.Shared.Identidades.Adm_PerfilTareas;
using PuntoVentaBin.Shared.Identidades.DTOs;
using PuntoVentaBin.Shared.LogServices;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PuntoVentaBin.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpleadosController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly ApplicationDbContext context;

        public EmpleadosController(ApplicationDbContext context, ILogService logService)
        {
            _logService = logService;
            this.context = context;
        }


        [HttpGet("{negocioId}")]
        public async Task<Respuesta<List<UsuarioRolNegocio>>> Get(long negocioId)
        {
            var respuesta = new Respuesta<List<UsuarioRolNegocio>> { Estado = EstadosDeRespuesta.Correcto };

            try
            {

                if (negocioId == 1014)
                {
                    respuesta.Datos = new List<UsuarioRolNegocio>();
                }
                else
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
                        usuariosRolesNegocios.Add(new UsuarioRolNegocio { Usuario = usuarioConRol.Usuario, RolId = usuarioConRol.RolId });
                    }


                    respuesta.Datos = usuariosRolesNegocios;
                }


            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al consultar el usuario";
            }

            return respuesta;
        }

        [HttpGet("GetPermisos/{rolId}")]
        public async Task<Respuesta<List<Permiso>>> GetPermisos(long rolId)
        {
            var respuesta = new Respuesta<List<Permiso>> { Estado = EstadosDeRespuesta.Correcto };

            try
            {
                respuesta.Datos = await (from rp in context.RolesPermisos
                                         join p in context.Permisos on rp.PermisoID equals p.Id
                                         where rp.RolID == rolId
                                         select p)
                                    .AsNoTracking()
                                    .ToListAsync()
                                    .ConfigureAwait(false);

            }

            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al consultar los permisos";
            }

            return respuesta;
        }


        [HttpPost]
        [Route("{action}")]
        public async Task<Respuesta<long>> VincularUsuarioConNegocio(UsuarioConNegocioPorAsignar value)
        {
            var respuesta = new Respuesta<long> { Estado = EstadosDeRespuesta.Correcto };
            try
            {

                var usuarioEstaRegistrado = await context.UsuariosBin.
                    AsNoTracking().
                    AnyAsync(x => x.Email == value.Email);

                //Caso: El nuevo empleado esta registado como Usuario
                if (usuarioEstaRegistrado)
                {
                    var usuarioId = await context.UsuariosBin.
                        Where(x => x.Email == value.Email).
                        Select(x => x.Id).
                        FirstOrDefaultAsync();

                    var usuarioYaEstaRelacionadoConElNegocio = await context.UsuariosRolesNegocios.
                        AsNoTracking().
                        AnyAsync(x => x.UsuarioId == usuarioId && x.NegocioId == value.NegocioId);
                    
                    //Caso: Ya esta vinculado con el negocio
                    if (usuarioYaEstaRelacionadoConElNegocio)
                    {
                        return new Respuesta<long> { Datos = 0, Estado = EstadosDeRespuesta.NoProceso, Mensaje = "El usuario ya esta relacionado con el negocio" };
                    }
                    //El usuario ya esta registrado y se va vincular con el negocio
                    else
                    {
                        context.Add(new UsuarioRolNegocio { UsuarioId = usuarioId, RolId = value.RolId, NegocioId = value.NegocioId });
                        await context.SaveChangesAsync();

                        respuesta.Datos = usuarioId;
                        respuesta.Mensaje = "El usuario se vinculo correctamente con el negocio";
                    }
                }
                //Caso: El nuevo empleado no esta registado como Usuario
                else
                {
                    context.Add(value);
                    await context.SaveChangesAsync();
                    
                    respuesta.Datos = 0;
                    respuesta.Mensaje = "Se asignara el usuario al negocio cuando este se registre"; 
                }

            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al guardar el usuario desde la pagina empleados";
            }
            return respuesta;
        }


        [HttpPost]
        [Route("{action}")]
        public async Task<Respuesta<long>> EliminarUsuarioDelNegocio(UsuarioRolNegocio value)
        {
            var respuesta = new Respuesta<long> { Estado = EstadosDeRespuesta.Correcto };
            try
            {
                var usuarioRolNegocio = await context.UsuariosRolesNegocios.
                    AnyAsync(x => x.UsuarioId == value.UsuarioId && x.RolId == value.RolId && x.NegocioId == value.NegocioId);

                if (usuarioRolNegocio)
                {
                    context.UsuariosRolesNegocios.Remove(value);
                    context.SaveChangesAsync();
                }

                respuesta.Mensaje = "El usuario se desvinculo correctamente con el usuario";
            }
            catch (Exception ex)
            {
                respuesta.Estado = EstadosDeRespuesta.Error;
                respuesta.Mensaje = $"Error al tratar de desvincular el usuario del negocio.";
            }
            return respuesta;
        }

    }
}
