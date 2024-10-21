using System.ComponentModel.DataAnnotations;

namespace PuntoVentaBin.Shared.Identidades.Productos
{
    public class Categoria
    {
        [Key]
        public long Id { get; set; }
        public long NegocioId { get; set; }
        
        [Required(ErrorMessage ="El nombre es requerido")]
        public string Nombre { get; set; }

    }
}
