using APIBCPSistemaVigilancia.Models.BDBCPSistemaVigilancia;
using Microsoft.EntityFrameworkCore;
using SistemaVigilanciaBCPApi.Models;
using System.DirectoryServices;
using System.Globalization;

namespace SistemaVigilanciaBCPApi.Services
{
    public class LoginService : ILoginService
    {
        private readonly BCPSistemaVigilanciaContext _context;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly ICriptografia _criptografia;
        public LoginService(BCPSistemaVigilanciaContext context, IConfiguration configuration, ITokenService tokenService, ICriptografia criptografia)
        {
            _context = context;
            _configuration = configuration;
            _tokenService = tokenService;
            _criptografia = criptografia;
        }

        public async Task<List<UsuariosConectado>> ObtenerUsuariosConectados()
        {
            var listaUsuariosConectados = await _context.UsuariosConectado.ToListAsync();
            return listaUsuariosConectados;
        }

        public async Task<bool> ConectarUsuario(UsuariosConectado user)
        {
            //Agregar Usuario a tabla de usuarios conectados
            try
            {
                var usuario = await _context.UsuariosConectado.FindAsync(user.Usuario);
                if (usuario == null)
                {
                    user.Estado = true;
                    user.FechaIngreso = DateTime.Now;
                    user.Ip = user.Ip;

                    user.UltimaConsulta = DateTime.Now;
                    await _context.Procedures.AgregarUsuarioConectadoAsync(user.Usuario, user.FechaIngreso, user.Ip, user.UltimaConsulta,user.Grupo,true);
                    //await _context.UsuariosConectados.AddAsync(user);
                    //await _context.SaveChangesAsync();
                }
                else
                {
                    usuario.Estado = true;
                    usuario.FechaIngreso = DateTime.Now;
                    user.Ip = user.Ip;
                    user.UltimaConsulta = DateTime.Now;
                    await _context.Procedures.ModificarUsuarioConectadoAsync(usuario.Usuario, usuario.FechaIngreso, usuario.Ip, usuario.UltimaConsulta, user.Grupo, true);
                    //await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<string> DesconectarUsuario(string usuario)
        {
            try
            {
                var usuarioConectado = await _context.UsuariosConectado.FindAsync(usuario);
                usuarioConectado.Estado = false;
                await _context.SaveChangesAsync();
                return "Ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> ExpulsarUsuario(string usuarioExpulsado)
        {
            try
            {
                var usuarioConectado = await _context.UsuariosConectado.FindAsync(usuarioExpulsado);
                usuarioConectado.Estado = false;
                await _context.SaveChangesAsync();
                return "Ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<bool> ExpulsarUsuariosInactivos()
        {
            try
            {
                var tiempoExpiracionMinutos = (Convert.ToInt16(_configuration["JwtSettings:ExpiresInHours"]) * 60 + 1);
                //var tiempoExpiracionMinutos = 21;
                var horaServidor = DateTime.Now;
                var usuarioConectado = await _context.UsuariosConectado.Where(x => x.Estado == true).ToListAsync();
                string formato = "yyyy-MM-dd HH:mm:ss.fff";
                foreach (var item in usuarioConectado)
                {
                    //Restar tiempos
                    DateTime fechaIngreso = Convert.ToDateTime(item.FechaIngreso);
                    TimeSpan? resultadoResta = horaServidor - fechaIngreso;
                    var res = TimeSpan.FromMinutes(tiempoExpiracionMinutos) - resultadoResta.Value;
                    if (res <= TimeSpan.Zero)
                    {
                        item.Estado=false;
                    }
                }
                await _context.SaveChangesAsync();
                //var usuarioConectado = await _context.UsuariosConectados.FindAsync("");
                //usuarioConectado.Estado = false;
                //await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<(string, string)> ValidarAcceso(UsuarioCredenciales usuario)
        {
            try
            {
                var rutaLDAP = _configuration["Configuraciones:LDAP"];
                var superUsuario = _configuration["Configuraciones:Superadmin:usuario"];
                var passwordSuperUsuario = _configuration["Configuraciones:Superadmin:password"];
                var grupoSuperUsuario = _configuration["Configuraciones:Superadmin:grupo"];
                if (usuario.User == superUsuario && usuario.Password == passwordSuperUsuario)
                {
                    UsuariosConectado usuarios = new UsuariosConectado();
                    usuarios.Usuario = superUsuario;
                    usuarios.Grupo = grupoSuperUsuario;

                    var token = _tokenService.GenerateToken(usuarios,"superUsuario");
                    return ("Ok",token);
                }
                System.DirectoryServices.DirectoryEntry srvLDAPActiveDirectory;
                srvLDAPActiveDirectory = new System.DirectoryServices.DirectoryEntry(rutaLDAP, usuario.User, usuario.Password);
                DirectorySearcher searchEntry = new DirectorySearcher(srvLDAPActiveDirectory);
                searchEntry.Filter = "(sAMAccountName=" + usuario.User + ")";
                //Validar si existe en BCP
                SearchResult result = searchEntry.FindOne();
                if (result == null)
                {
                    return ("Usuario incorrecto o no autorizado","");
                }
                //Validar si esta en un grupo habilitado
                System.DirectoryServices.DirectoryEntry userEntry = result.GetDirectoryEntry();
                foreach (string groupDN in userEntry.Properties["memberOf"])
                {
                    int equalsIndex = groupDN.IndexOf("=", 1);
                    int commaIndex = groupDN.IndexOf(",", 1);
                    string groupName = groupDN.Substring((equalsIndex + 1), (commaIndex - equalsIndex) - 1);

                    // Validar si esta en un grupo permitido
                    var grupoPermitido = await _context.Grupo.Where(x => x.Nombre == groupName && x.Activo == true).ToListAsync();
                    if (grupoPermitido.Count > 0)
                    {
                        UsuariosConectado conectado = new UsuariosConectado();
                        conectado.Usuario = usuario.User;
                        conectado.Grupo = groupName;
                        var token2 = _tokenService.GenerateToken(conectado, "usuario");
                        return ("Ok", token2);
                    }
                }
                return ("Usuario incorrecto o no autorizado", "");
            }
            catch (Exception ex)
            {
                return (ex.Message,"");
            }
        }

        public async Task<bool> ValidarUsuarioActivo(string usuario) {
            try
            {
                var res = await _context.UsuariosConectado.Where(x => x.Usuario == usuario && x.Estado == true).FirstOrDefaultAsync();
                if (res == null)
                {
                    return false;
                }
                return true;
            }
            catch(Exception ex)
            {
                return true;
            }
        }

        public async Task<string> DesconectarUsuarioTokenExpirado(string usuario)
        {
            try
            {
                var user = await _context.UsuariosConectado.Where(x => x.Usuario == usuario).FirstOrDefaultAsync();
                user.Estado = false;
                await _context.SaveChangesAsync();
                return "Ok";
            }catch(Exception ex)
            {
                return ex.Message;
            }
        }

        #region Funciones de apoyo

        #endregion
    }
}
