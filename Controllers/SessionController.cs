using APIBCPSistemaVigilancia.Models.BDBCPSistemaVigilancia;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVigilanciaBCPApi.Models;
using SistemaVigilanciaBCPApi.Services;
using System.IdentityModel.Tokens.Jwt;

namespace SistemaVigilanciaBCPApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly ICriptografia _criptografia;
        private readonly IAuditoriaIngresosService _auditoriaIngresosService;

        public SessionController(ILoginService loginService, ICriptografia criptografia, IAuditoriaIngresosService auditoriaIngresosService)
        {
            _loginService = loginService;
            _criptografia = criptografia;
            _auditoriaIngresosService = auditoriaIngresosService;
        }

        [HttpPost]
        [Route("ValidarUsuario")]
        public async Task<IActionResult> ValidarUsuario(UsuarioCredenciales usuario)
        {
            string ip = "";
            try
            {
                string publicIpAddress = HttpContext.Request.Headers["X-Forwarded-For"];
                string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                if (!string.IsNullOrEmpty(publicIpAddress))
                {
                    // La dirección IP pública puede contener múltiples direcciones separadas por comas.
                    // En este caso, toma la primera dirección IP de la lista.
                    publicIpAddress = publicIpAddress.Split(',', StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                    ipAddress = publicIpAddress;
                }
                ip = ipAddress;
            }
            catch(Exception ex)
            {

            }


            try
            {
               var resultado =  await _loginService.ValidarAcceso(usuario);
                if(resultado.Item1 == "Ok")
                {
                    //Obtener datos de token
                    var tokenResultado = _criptografia.ObtenerDatosToken(resultado.Item2);
                    _auditoriaIngresosService.RegistrarIngreso(tokenResultado, ip);
                    return StatusCode(StatusCodes.Status200OK, new {message= resultado.Item1, token=resultado.Item2});
                }
                _auditoriaIngresosService.RegistrarIngresoFallido(usuario.User, ip, resultado.Item1);
                return StatusCode(StatusCodes.Status200OK, new { message = resultado.Item1});

            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [Authorize]
        [HttpPost]
        [Route("ConectarUsuario")]
        public async Task<IActionResult> ConectarUsuario()
        {
            string publicIpAddress = HttpContext.Request.Headers["X-Forwarded-For"];
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            if (!string.IsNullOrEmpty(publicIpAddress))
            {
                // La dirección IP pública puede contener múltiples direcciones separadas por comas.
                // En este caso, toma la primera dirección IP de la lista.
                publicIpAddress = publicIpAddress.Split(',', StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                ipAddress = publicIpAddress;
            }
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var tokenResultado=_criptografia.ObtenerDatosToken(token);
            
            try
            {
                UsuariosConectado usuarios = new UsuariosConectado();
                usuarios.Usuario = tokenResultado.Usuario;
                usuarios.Grupo = tokenResultado.Grupo;
                usuarios.Ip = ipAddress;
                var res = await _loginService.ConectarUsuario(usuarios);
                //var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (res)
                {
                    return StatusCode(StatusCodes.Status200OK, "Ok");
                }
                return StatusCode(StatusCodes.Status200OK, "Fallo al conectar usuario");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        [Route("ValidarUsuarioActivo/{usuario}")]
        public async Task<IActionResult> ValidarUsuarioActivo(string usuario)
        {
            try
            {
                var res = await _loginService.ValidarUsuarioActivo(usuario);
                return StatusCode(StatusCodes.Status200OK, res);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, true);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("DesconectarUsuario/{usuario}/{razon}")]
        public async Task<IActionResult> DesconectarUsuario(string usuario,string razon)
        {
            string ip = "";
            try
            {
                string publicIpAddress = HttpContext.Request.Headers["X-Forwarded-For"];
                string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                if (!string.IsNullOrEmpty(publicIpAddress))
                {
                    // La dirección IP pública puede contener múltiples direcciones separadas por comas.
                    // En este caso, toma la primera dirección IP de la lista.
                    publicIpAddress = publicIpAddress.Split(',', StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                    ipAddress = publicIpAddress;
                }
                ip = ipAddress;
            }
            catch (Exception ex)
            {

            }
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var tokenResultado = _criptografia.ObtenerDatosToken(token);
            try
            {
                if(razon != "Expulsado")
                {
                    await _loginService.ExpulsarUsuario(usuario);
                }
                //var res = await _loginService.ExpulsarUsuario(usuario);
                _auditoriaIngresosService.RegistrarExpulsion(tokenResultado, razon, ip);
                return StatusCode(StatusCodes.Status200OK, "Ok");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, true);
            }
        }

        [HttpPost]
        [Route("DesconectarUsuarioTokenExpirado/{usuario}")]
        public async Task<IActionResult> DesconectarUsuarioTokenExpirado(string usuario)
        {
            string ip = "";
            try
            {
                string publicIpAddress = HttpContext.Request.Headers["X-Forwarded-For"];
                string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                if (!string.IsNullOrEmpty(publicIpAddress))
                {
                    // La dirección IP pública puede contener múltiples direcciones separadas por comas.
                    // En este caso, toma la primera dirección IP de la lista.
                    publicIpAddress = publicIpAddress.Split(',', StringSplitOptions.RemoveEmptyEntries)[0].Trim();
                    ipAddress = publicIpAddress;
                }
                ip = ipAddress;
            }
            catch (Exception ex)
            {

            }

            try
            {
                var res = await _loginService.DesconectarUsuarioTokenExpirado(usuario);
                _auditoriaIngresosService.RegistrarSalidaTokenExpirado(usuario, ip);
                return StatusCode(StatusCodes.Status200OK, res);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, true);
            }
        }
    }
}
