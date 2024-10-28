using System.ComponentModel.DataAnnotations;

namespace PuntoVentaBin.Shared.Identidades
{
    public class Provedor
    {
        [Key]
        public long Id { get; set; }
        public long NegocioId { get; set; }
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El nombre de la empresa es requerido.")]
        public string Empresa { get; set; }
        public string? TelefonoNegocio { get; set; } //TODO: TELEFONO CELULAR Y DE NEGOCIO, COMO PODEMOS ESPECIFICAR??
        public  string? TelefonoCelular { get; set; }
        public string? CorreoElectronico { get; set;}

    }
}
