using APIBCPSistemaVigilancia.Models.BDBCPSistemaVigilancia;

namespace SistemaVigilanciaBCPApi.Services
{
    public interface ISeguridadService
    {
        public Task<string> CrearGrupo(Grupo grupoAgregar);
        public Task<List<UsuariosConectado>> ObtenerUsuariosConectados();
        public Task<List<Grupo>> ObtenerGrupos();
        public Task<List<PermisoGrupo>> ObtenerListaPermisosGrupo(int idGrupo);
        public Task<List<PermisoGrupo>> ObtenerListaPermisosGrupoNombre(string nombreGrupo);
        public Task<Grupo> ObtenerGrupo(int id);
        public Task<List<Modulo>> ObtenerListaModulos();
        public Task<string> ModificarGrupo(Grupo grupo);
        public Task<string> ModificarPermisoGrupo(List<PermisoGrupo> permiso);
    }
}
