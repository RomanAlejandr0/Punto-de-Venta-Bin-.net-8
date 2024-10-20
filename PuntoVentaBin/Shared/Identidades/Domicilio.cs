using System.ComponentModel.DataAnnotations;

namespace PuntoVentaBin.Shared.Identidades
{
    public class Domicilio
    {
        [Key]
        public long Id { get; set; }
        public long NeogocioId { get; set; }
        public long ClienteId { get; set; }
        public string Calle { get; set; }
        public string NumeroExterior { get; set; }
        public string Colonia { get; set; }
        public string Localidad { get; set; }
        public string Municipio { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        //[Required(ErrorMessage = "El codigo postal es requerido")]
        public string CodigoPostal { get; set; }

        public DateTime Fecha { get; set; }
    }
}
