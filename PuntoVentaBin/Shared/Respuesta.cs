namespace PuntoVentaBin.Shared
{
    public class Respuesta<T>
    {
        public EstadosDeRespuesta Estado { get; set; }

        public string Mensaje { get; set; }

        public T Datos { get; set; }
    }

    public enum EstadosDeRespuesta : int
    {
        NoProceso = 0,
        Correcto = 1,
        Error = 2
    }

}

//Mejores temas

/*Light*/
//Emilia Light
//DarknessLight
//

/*Dark*/
//Astolfo
//Azuki
//Chocola
//Cinnamon
//Coconut
//Darkness Dark
//Genos
//HatsuneMiku
//Hayase Nagaroto
//Ishtar Dark
//Jamabi Yumeko
//Rei
//Zero Two dark Obsidian