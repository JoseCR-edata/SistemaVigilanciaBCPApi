
using APIBCPSistemaVigilancia.Models.BDBCPSistemaVigilancia;
using SistemaVigilanciaBCPApi.Models;
using VideovigilanciaDB.Models.VideovigilanciaBCP;

namespace SistemaVigilanciaBCPApi.Services
{
    public interface IAlarma
    {
        public List<dynamic> GetAlarmas(AlarmaFiltro alarmaFiltro);
        public Task<AlarmaDetalle> GetAlarma(int id);
        public Task<string> PruebaImagenAlarma(string id);
        public List<TipoAlarma> GetAlarmasTipo();
        public List<NivelAlarma> GetAlarmasNivel();
    }
}
