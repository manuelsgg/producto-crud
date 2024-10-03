namespace ProductApi.Models
{
    public class Documento
    {
        public int DocumentoId { get; set; }
        public string NombreDocumento { get; set; }
        public string Tipo { get; set; }
        public string Ruta { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
    }
}
