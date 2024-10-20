using System.ComponentModel.DataAnnotations;


namespace PuntoVentaBin.Shared.Identidades
{
    public class EmailModel
    {
        [Required(ErrorMessage = "El correo electrónico es requerido.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public string Email { get; set; }
    }
}
