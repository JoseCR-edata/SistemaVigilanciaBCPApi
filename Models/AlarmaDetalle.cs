namespace SistemaVigilanciaBCPApi.Models
{
    public class AlarmaDetalle
    {
        public string NombreCamara { get; set; }

        public DateTime? HoraAlarma { get; set; }

        public int? TipoAlarma { get; set; }

        public int? NivelAlarma { get; set; }

        public string? imagen { get; set; }
    }
}
