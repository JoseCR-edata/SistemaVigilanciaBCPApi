using APIBCPSistemaVigilancia.Models.BDBCPSistemaVigilancia;
using SistemaVigilanciaBCPApi.Models;

namespace SistemaVigilanciaBCPApi.Services
{
    public interface IAuditoriaIngresosService
    {
        public void RegistrarIngreso(DatosToken datosToken, string ip);
        public void RegistrarSalida(DatosToken datosToken, string ip);
        public void RegistrarExpulsion(DatosToken datosToken,string razon, string ip);
        public void RegistrarIngresoFallido(string usuario, string ip, string mensage);
        public void RegistrarSalidaTokenExpirado(string usuario, string ip);
        public void RegistrarBusquedaSesiones(DatosToken usuariosConectado, string eventoRealizado, string ip);
        public List<dynamic> ObtenerAuditoriaIngresos(AuditoriaFiltro filtro);
        

    }
}
