using APIBCPSistemaVigilancia.Models.BDBCPSistemaVigilancia;

namespace SistemaVigilanciaBCPApi.Services
{
    public interface ITokenService
    {
        string GenerateToken(UsuariosConectado usuariosConectado,string tipoUsuario);
    }
}
