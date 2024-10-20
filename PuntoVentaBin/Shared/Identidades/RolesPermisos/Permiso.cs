using System.ComponentModel.DataAnnotations;

namespace PuntoVentaBin.Shared.Identidades.Adm_PerfilTareas
{
    public class Permiso
    {
        [Key]
        public int PermisoId { get; set; }

        public string Descripcion { get; set; }

        public DateTime Fecha { get; set; }

        public bool Activo { get; set; }
    }
}
