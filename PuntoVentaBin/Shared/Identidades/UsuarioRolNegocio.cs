using PuntoVentaBin.Shared.Identidades.Adm_PerfilTareas;

namespace PuntoVentaBin.Shared.Identidades
{
    public class UsuarioRolNegocio
    {
        public long UsuarioId { get; set; }
        public UsuarioBin Usuario { get; set; }

        public int RolId { get; set; }
        public Rol Rol { get; set; }

        public long NegocioId { get; set; }
        public Negocio Negocio { get; set; }
    }
}
