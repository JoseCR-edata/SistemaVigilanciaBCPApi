
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SistemaVigilanciaBCPApi.Models;
using SistemaVigilanciaBCPApi.Services;

namespace SistemaVigilanciaBCPApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataController : ControllerBase
    {
        private readonly IAlarma _alarma;
        private readonly ICriptografia _criptografia;
        private readonly IAuditoriaActividadesService _auditoriaActividadesService;

        public DataController(IAlarma alarma, ICriptografia criptografia, IAuditoriaActividadesService auditoriaActividadesService)
        {
            _alarma = alarma;
            _criptografia = criptografia;
            _auditoriaActividadesService = auditoriaActividadesService;
        }

        [Authorize]
        [HttpPost]
        [Route("ObtenerAlarmas")]
        public async Task<IActionResult> ObtenerAlarmas(AlarmaFiltro filtros)
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


            //Generar texto descriptivo
            var texto = "El usuario " + tokenResultado.Usuario + " busco alarmas";
            if(filtros.NombreCamara != "" || filtros.TipoAlerta != "" || filtros.FechaInicio != "" || filtros.FechaFin != "" || filtros.NivelAlarma != "")
            {
                texto = texto +  " usando los filtros ";
            }
            if (filtros.NombreCamara != "")
            {
                texto = texto + "Nombre de camara: "+filtros.NombreCamara;
            }
            if (filtros.TipoAlerta != "")
            {
                texto = texto + ", Tipo de Alerta: " + filtros.TipoAlerta;
            }
            if (filtros.FechaInicio != "")
            {
                texto = texto + ", Fecha Inicio: " + filtros.FechaInicio;
            }
            if (filtros.FechaFin != "")
            {
                texto = texto + ", Fecha Fin: " + filtros.FechaFin;
            }
            if (filtros.NivelAlarma != "")
            {
                texto = texto + ", Nivel de Alarma: " + filtros.NivelAlarma;
            }
            //
            try
            {
                var rspta= _alarma.GetAlarmas(filtros);
                _auditoriaActividadesService.RegistrarBusquedaAlarma(tokenResultado, texto, ip);
                return StatusCode(StatusCodes.Status200OK, rspta);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("ObtenerAlarma/{idalarma:int}")]
        public async Task<IActionResult> ObtenerAlarma(int idalarma)
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
                var rspta = _alarma.GetAlarma(idalarma).Result;
                _auditoriaActividadesService.RegistrarBusquedaDetalleAlarma(tokenResultado, idalarma.ToString(), ip);
                return StatusCode(StatusCodes.Status200OK, rspta);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("ObtenerTiposAlarma")]
        public IActionResult ObtenerTiposAlarma()
        {
            try
            {
                var rspta = _alarma.GetAlarmasTipo();
                return StatusCode(StatusCodes.Status200OK, rspta);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("ObtenerNivelAlarma")]
        public IActionResult ObtenerNivelAlarma()
        {
            try
            {
                var rspta = _alarma.GetAlarmasNivel();
                return StatusCode(StatusCodes.Status200OK, rspta);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        //[HttpGet]
        //[Route("ObtenerAlarmaPrueba/{id}")]
        //public async Task<IActionResult> ObtenerAlarmaPrueba(string id)
        //{
        //    try
        //    {
        //        var rspta = _alarma.PruebaImagenAlarma(id).Result;
        //        return StatusCode(StatusCodes.Status200OK, rspta);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex);
        //    }
        //}

    }
}
