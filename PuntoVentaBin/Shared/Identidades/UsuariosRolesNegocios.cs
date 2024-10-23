using PuntoVentaBin.Shared.Identidades.Adm_PerfilTareas;

namespace PuntoVentaBin.Shared.Identidades
{
    public class UsuariosRolesNegocios
    {
        public long UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public long NegocioId { get; set; }
        public Negocio Negocio { get; set; }
        public int RolId { get; set; }
        public Rol Rol { get; set; }
    }
}
