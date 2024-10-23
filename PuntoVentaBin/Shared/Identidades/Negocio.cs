using System.ComponentModel.DataAnnotations;

namespace PuntoVentaBin.Shared.Identidades
{
    public class Negocio
    {
        public Negocio()
        {
            Clientes = new List<Cliente>();
        }

        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "El Tipo de Negocio es requerido")]
        public string TipoNegocio { get; set; }

        [Required(ErrorMessage = "El Nombre del Negocio es requerido")]
        [StringLength(100, ErrorMessage = "Limite de caracteres: 100.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El Codigo Postal del Negocio es requerido")]
        [RegularExpression(@"^\d{5}$", ErrorMessage = "Codigo postal invalido")]
        public string CodigoPostal { get; set; }
        public DateTime FechaRegistro { get; set; }

        public List<Cliente> Clientes { get; set; }
        public List<UsuariosRolesNegocios> UsuariosRolesNegocios { get; set; }

    }
}