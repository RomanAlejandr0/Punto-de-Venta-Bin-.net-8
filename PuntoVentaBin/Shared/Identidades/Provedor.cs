using System.ComponentModel.DataAnnotations;

namespace PuntoVentaBin.Shared.Identidades
{
    public class Provedor
    {
        [Key]
        public long Id { get; set; }
        public long EmpresaId { get; set; }
        public int RolId { get; set; }
        public string Nombre { get; set; }    
        public string Empresa { get; set; }
        public string TelefonoNegocio { get; set; } //TODO: TELEFONO CELULAR Y DE NEGOCIO, COMO PODEMOS ESPECIFICAR??
        public  string TelefonoCelular { get; set; }
        public string CorreoElectronico { get; set;}
    }
}
