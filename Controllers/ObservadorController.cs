using APIBCPSistemaVigilancia.Models.BDBCPSistemaVigilancia;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SistemaVigilanciaBCPApi.Hubs;
using SistemaVigilanciaBCPApi.Models;
using SistemaVigilanciaBCPApi.Services;
using System.Net;
using System.Security.Cryptography;

namespace SistemaVigilanciaBCPApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ObservadorController : ControllerBase
    {
        private readonly IHubContext<ControlUsuarioHub> _hubContext;
        private readonly BCPSistemaVigilanciaContext _contextBCP;
        private readonly ICriptografia _criptografia;
        private readonly IAuditoriaActividadesService _auditoriaActividadesService;
        private readonly IAuditoriaIngresosService _auditoriaIngresosService;
        private readonly ILoginService _loginService;

        public ObservadorController(IHubContext<ControlUsuarioHub> hubContext, ICriptografia criptografia, IAuditoriaActividadesService auditoriaActividadesService, ILoginService loginService, IAuditoriaIngresosService auditoriaIngresosService)
        {
            this._hubContext = hubContext;
            _criptografia = criptografia;
            _auditoriaActividadesService = auditoriaActividadesService;
            _loginService = loginService;
            _auditoriaIngresosService = auditoriaIngresosService;
        }

        [HttpPost]
        [Route("ExpulsarUsuarioPorNuevaSesion/{usuario}")]
        public async Task<IActionResult> ExpulsarUsuarioPorNuevaSesion(string usuario)
        {
            await _hubContext.Clients.All.SendAsync("ExpulsarUsuarioPorNuevaSesion", usuario);
            return StatusCode(StatusCodes.Status200OK, "Ok");
        }

        [Authorize]
        [HttpPost]
        [Route("ExpulsarUsuario/{usuario}")]
        public async Task<IActionResult> ExpulsarUsuario(string usuario)
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
            var res = await _loginService.ExpulsarUsuario(usuario);
            if (res == "Ok")
            {
                await _hubContext.Clients.All.SendAsync("ExpulsarUsuario", usuario);
                _auditoriaActividadesService.RegistrarExpulsionUsuario(tokenResultado, usuario, ip);
                return StatusCode(StatusCodes.Status200OK, "Ok");
            }
            return StatusCode(StatusCodes.Status200OK, "Error");
        }

        [Authorize]
        [HttpGet]
        [Route("ActualizarListaUsuarios")]
        public async Task<IActionResult> ActualizarListaUsuarios()
        {
            await _hubContext.Clients.All.SendAsync("ActualizarListaUsuarios");

            return StatusCode(StatusCodes.Status200OK, "Ok");
        }
    }
}
