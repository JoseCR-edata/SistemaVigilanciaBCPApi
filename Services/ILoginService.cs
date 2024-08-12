using APIBCPSistemaVigilancia.Models.BDBCPSistemaVigilancia;
using SistemaVigilanciaBCPApi.Models;

namespace SistemaVigilanciaBCPApi.Services
{
    public interface ILoginService
    {
        #region Gestion de Usuarios conectados
        public Task<bool> ConectarUsuario(UsuariosConectado user);
        public Task<string> ExpulsarUsuario(string usuarioExpulsado);
        public Task<bool> ExpulsarUsuariosInactivos();
        public Task<string> DesconectarUsuario(string usuario);
        public Task<string> DesconectarUsuarioTokenExpirado(string usuario);
        public Task<bool> ValidarUsuarioActivo(string usuario);
        #endregion
        #region Gestion de Ingreso
        public Task<(string, string)> ValidarAcceso(UsuarioCredenciales usuario);
        #endregion


    }
}
