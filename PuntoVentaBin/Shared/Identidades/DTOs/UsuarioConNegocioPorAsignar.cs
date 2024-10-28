using System.ComponentModel.DataAnnotations;

namespace PuntoVentaBin.Shared.Identidades.DTOs
{
    public class UsuarioConNegocioPorAsignar
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string Email { get; set; }

        public int RolId { get; set; }
        public long NegocioId { get; set; }
    }
}
