using System.ComponentModel.DataAnnotations;

namespace PuntoVentaBin.Shared.Identidades
{
    public class Cliente
    {
        [Key]
        public long Id { get; set; }
        public long NegocioId { get; set; }

        [Required(ErrorMessage = "El nombre es requerido.")]

        public string Nombre { get; set; }
        public string Email { get; set; }

        public List<Venta> Ventas { get; set; }
        
    }
}
