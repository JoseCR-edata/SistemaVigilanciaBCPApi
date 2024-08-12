using APIBCPSistemaVigilancia.Models.BDBCPSistemaVigilancia;
using SistemaVigilanciaBCPApi.Models;

namespace SistemaVigilanciaBCPApi.Services
{
    public interface IAuditoriaActividadesService
    {
        public void RegistrarCrearGrupo(DatosToken usuariosConectado, string nombreGrupo, string ip);
        public void RegistrarModificarPermisosGrupo(DatosToken usuariosConectado, string eventoRealizado, string ip);
        public void RegistrarExpulsionUsuario(DatosToken usuariosConectado, string usuarioExpulsado, string ip);
        public void RegistrarBusquedaAlarma(DatosToken usuariosConectado, string eventoRealizado, string ip);
        public void RegistrarBusquedaDetalleAlarma(DatosToken usuariosConectado, string idAlarma, string ip);
        public void RegistrarBusquedaActividades(DatosToken usuariosConectado, string eventoRealizado, string ip);
        public void RegistrarIngresoGrupoDetalle(DatosToken usuariosConectado,string nombreGrupo, string ip);
        public List<dynamic> ObtenerAuditoriaCambios(AuditoriaFiltro filtro);
    }
}
