namespace PuntoVentaBin.Shared.Identidades
{
    public class Empleado
    {
        public long Id { get; set; }
        public long usuarioId { get; set; }
        public string Nombre { get; set; }
        public int RolId { get; set; }
    }
}
