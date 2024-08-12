using APIBCPSistemaVigilancia.Models.BDBCPSistemaVigilancia;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SistemaVigilanciaBCPApi.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateToken(UsuariosConectado usuariosConectado, string tipoUsuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //var keyBytes = Encoding.ASCII.GetBytes(key);

            var claims = new ClaimsIdentity();
            //claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.Usuario));
            claims.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, usuariosConectado.Usuario));
            claims.AddClaim(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.AddClaim(new Claim(JwtRegisteredClaimNames.Iat,DateTime.Now.ToString()));
            claims.AddClaim(new Claim("UsuarioConectado", JsonConvert.SerializeObject(usuariosConectado)));
            claims.AddClaim(new Claim("TipoUsuario", tipoUsuario));
            var num = Convert.ToDouble(_configuration["JwtSettings:ExpiresInHours"]);
            //var credencialesToken = new SigningCredentials(
            //    new SymmetricSecurityKey(keyBytes),
            //    SecurityAlgorithms.HmacSha256Signature
            //    );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddHours(num),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

            string tokenCreado = tokenHandler.WriteToken(tokenConfig);
            return tokenCreado;
        }
    }
}
