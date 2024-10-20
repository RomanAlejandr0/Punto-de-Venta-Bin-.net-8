using System.ComponentModel.DataAnnotations;

namespace PuntoVentaBin.Shared.Identidades.Catalogos
{
    public class CodigosPostales
    {
        [Key]
        public string c_CodigoPostal { get; set; }

        public string c_Estado { get; set; }

        public string c_Municipio { get; set; }

        public string? c_Localidad { get; set; }

        public Estados Estado { get; set; }

        public Localidades Localidad { get; set; }

        public Municipios Municipio { get; set; }

        public List<Colonias> Colonias { get; set; }

    }
}
