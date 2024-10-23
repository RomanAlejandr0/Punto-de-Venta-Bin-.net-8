using System.ComponentModel.DataAnnotations;

namespace PuntoVentaBin.Shared.Identidades
{
    public class Usuario
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "Limite de caracteres: 100.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [RegularExpression(@"(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,100}",
        ErrorMessage = "Formato de contraseña invalido")]
        public string Password { get; set; }

        //[Required(ErrorMessage = "La confirmación de contraseña es requerida.")]
        //[Compare("NuevaContraseña", ErrorMessage = "Las contraseñas no coinciden.")]
        //public string ConfirmarContraseña { get; set; } //No mapea a base de datos

        public DateTime FechaRegistro { get; set; }
        public bool CuentaActivada { get; set; } 
        public string? TokenConfirmacion { get; set; }
        public string? TokenRecuperacion { get; set; }
        public DateTime? FechaExpiracionTokenRecuperacion { get; set; }

        public List<UsuariosRolesNegocios> UsuariosRolesNegocios { get; set; }

    }


    public class UserInfo
    {
        [Required(ErrorMessage = "El Email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La Contraseña es requerida")]
        public string Password { get; set; }

    }
    public class UserToken
    {
        public string Token { get; set; }
        public DateTime? Expiration { get; set; }
    }

    public enum PerfilUsuario : int
    {
        Propietario = 1,
        Administrador = 2,
        Vendedor = 3
    }
}
