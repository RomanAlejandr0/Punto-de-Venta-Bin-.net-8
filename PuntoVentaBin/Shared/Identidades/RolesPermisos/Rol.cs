using System.ComponentModel.DataAnnotations;

namespace PuntoVentaBin.Shared.Identidades.Adm_PerfilTareas
{
    public class Rol
    {
        [Key]
        [Required(ErrorMessage = "El perfil es requerido")]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public bool Activo { get; set; }
        public List<UsuarioRolNegocio> UsuariosRolesNegocios { get; set; }

    }
}
