using APIBCPSistemaVigilancia.Models.BDBCPSistemaVigilancia;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaVigilanciaBCPApi.Models;
using SistemaVigilanciaBCPApi.Services;

namespace SistemaVigilanciaBCPApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SeguridadController: ControllerBase
    {
        private readonly ISeguridadService _seguridadService;
        private readonly IAuditoriaIngresosService _auditoriaIngresosService;
        private readonly IAuditoriaActividadesService _auditoriaActividadesService;
        private readonly ICriptografia _criptografia;

        public SeguridadController(ISeguridadService seguridadService, IAuditoriaIngresosService auditoriaIngresosService, IAuditoriaActividadesService auditoriaActividadesService, ICriptografia criptografia)
        {
            _seguridadService = seguridadService;
            _auditoriaIngresosService = auditoriaIngresosService;
            _auditoriaActividadesService = auditoriaActividadesService;
            _criptografia = criptografia;
        }

        [Authorize]
        [HttpGet]
        [Route("ObtenerUsuariosConectados")]
        public async Task<IActionResult> ObtenerUsuariosConectados()
        {
            try
            {
                var resultado = await _seguridadService.ObtenerUsuariosConectados();
                return StatusCode(StatusCodes.Status200OK, resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #region Grupo
        [Authorize]
        [HttpGet]
        [Route("ObtenerGrupos")]
        public async Task<IActionResult> ObtenerGrupos()
        {
            try
            {
                var resultado = await _seguridadService.ObtenerGrupos();

                return StatusCode(StatusCodes.Status200OK, resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("ObtenerListaPermisosGrupo/{idGrupo}")]
        public async Task<IActionResult> ObtenerListaPermisosGrupo(int idGrupo)
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
                var nombreGrupo = await _seguridadService.ObtenerGrupo(idGrupo);
                var resultado = await _seguridadService.ObtenerListaPermisosGrupo(idGrupo);
                _auditoriaActividadesService.RegistrarIngresoGrupoDetalle(tokenResultado,nombreGrupo.Nombre, ip);
                return StatusCode(StatusCodes.Status200OK, resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("ObtenerListaPermisosGrupoNombre/{nombreGrupo}")]
        public async Task<IActionResult> ObtenerListaPermisosGrupoNombre(string nombreGrupo)
        {
            try
            {
                var resultado = await _seguridadService.ObtenerListaPermisosGrupoNombre(nombreGrupo);
                return StatusCode(StatusCodes.Status200OK, resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("AgregarGrupo")]
        public async Task<IActionResult> AgregarGrupo(Grupo grupoAgregar)
        {
            try
            {
                var resultado = await _seguridadService.CrearGrupo(grupoAgregar);
                return StatusCode(StatusCodes.Status200OK, resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("EditarGrupo")]
        public async Task<IActionResult> EditarGrupo(Grupo grupo)
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
                var resultado = await _seguridadService.ModificarGrupo(grupo);
                _auditoriaActividadesService.RegistrarCrearGrupo(tokenResultado, grupo.Nombre,ip);
                return StatusCode(StatusCodes.Status200OK, resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("ModificarPermisoGrupo")]
        public async Task<IActionResult> ModificarPermisoGrupo(List<PermisoGrupo> permiso)
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
                var nombreGrupo = await _seguridadService.ObtenerGrupo((int)permiso[0].GrupoId);
                var nombreModulos = await _seguridadService.ObtenerListaModulos();
                //Crear texto de actividad
                var eventoRealizado = "El usuario " + tokenResultado.Usuario + " modifico los permisos del grupo " + nombreGrupo.Nombre + " con los siguientes permisos: ";
                foreach (var item in nombreModulos)
                {
                    foreach (var item1 in permiso)
                    {
                        if(item.Id == item1.ModuloId)
                        {
                            var permisoGrupo = item1.PuedeIngresar==true ? "Puede ver " : "No puede ver ";
                            eventoRealizado += permisoGrupo + item.Nombre+", ";
                            break;
                        }
                    }
                }

                var resultado = await _seguridadService.ModificarPermisoGrupo(permiso);
                _auditoriaActividadesService.RegistrarModificarPermisosGrupo(tokenResultado, eventoRealizado, ip);
                return StatusCode(StatusCodes.Status200OK, resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
        #region Permisos Grupo
        [Authorize]
        [HttpGet]
        [Route("ObtenerListaModulos")]
        public async Task<IActionResult> ObtenerListaModulos()
        {
            try
            {
                var res = await _seguridadService.ObtenerListaModulos();
                return StatusCode(StatusCodes.Status200OK, res);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error");
            }
        }

        #endregion

        #region Auditoria
        [Authorize]
        [HttpPost]
        [Route("ObtenerAuditoriaSesion")]
        public async Task<IActionResult> ObtenerAuditoriaSesion(AuditoriaFiltro filtros)
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
            var texto = "El usuario " + tokenResultado.Usuario + " busco en auditoria sesiones";
            if (filtros.Usuario != "" || filtros.FechaInicio != "" || filtros.FechaFin != "" || filtros.Accion != "" || filtros.Modulo != "")
            {
                texto = texto + " usando los filtros ";
            }
            if (filtros.Usuario != "")
            {
                texto = texto + "Usuario: " + filtros.Usuario;
            }
            if (filtros.FechaInicio != "")
            {
                texto = texto + ", Fecha Inicio: " + filtros.FechaInicio;
            }
            if (filtros.FechaFin != "")
            {
                texto = texto + ", Fecha Fin: " + filtros.FechaFin;
            }
            if (filtros.Accion != "")
            {
                texto = texto + ", Acción: " + filtros.Accion;
            }
            if (filtros.Modulo != "")
            {
                texto = texto + ", Modulo: " + filtros.Modulo;
            }

            try
            {
                var listaAuditoriaSesion = _auditoriaIngresosService.ObtenerAuditoriaIngresos(filtros);
                _auditoriaIngresosService.RegistrarBusquedaSesiones(tokenResultado, texto, ip);
                return StatusCode(StatusCodes.Status200OK, listaAuditoriaSesion);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("ObtenerAuditoriaActividades")]
        public async Task<IActionResult> ObtenerAuditoriaActividades(AuditoriaFiltro filtros)
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
            var texto = "El usuario " + tokenResultado.Usuario + " busco en auditoria actividades";
            if (filtros.Usuario != "" || filtros.FechaInicio != "" || filtros.FechaFin != "" || filtros.Accion != "" || filtros.Modulo != "")
            {
                texto = texto + " usando los filtros ";
            }
            if (filtros.Usuario != "")
            {
                texto = texto + "Usuario: " + filtros.Usuario;
            }
            if (filtros.FechaInicio != "")
            {
                texto = texto + ", Fecha Inicio: " + filtros.FechaInicio;
            }
            if (filtros.FechaFin != "")
            {
                texto = texto + ", Fecha Fin: " + filtros.FechaFin;
            }
            if (filtros.Accion != "")
            {
                texto = texto + ", Acción: " + filtros.Accion;
            }
            if (filtros.Modulo != "")
            {
                texto = texto + ", Modulo: " + filtros.Modulo;
            }

            try
            {
                var listaAuditoria = _auditoriaActividadesService.ObtenerAuditoriaCambios(filtros);
                _auditoriaActividadesService.RegistrarBusquedaActividades(tokenResultado, texto, ip);
                return StatusCode(StatusCodes.Status200OK, listaAuditoria);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}
