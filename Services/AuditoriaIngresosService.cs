using APIBCPSistemaVigilancia.Models.BDBCPSistemaVigilancia;
using Microsoft.EntityFrameworkCore;
using SistemaVigilanciaBCPApi.Models;
using System.Globalization;

namespace SistemaVigilanciaBCPApi.Services
{
    public class AuditoriaIngresosService : IAuditoriaIngresosService
    {
        private readonly BCPSistemaVigilanciaContext _context;

        public AuditoriaIngresosService(BCPSistemaVigilanciaContext context)
        {
            _context = context;
        }

        public List<dynamic> ObtenerAuditoriaIngresos(AuditoriaFiltro filtro)
        {
            var contieneFecha = false;
            //Primero filtrar por fecha si tiene
            var auditoriaIngresoLista = new List<AuditoriaIngreso>();
            if(filtro.FechaInicio != "" || filtro.FechaFin != "")
            {
                if(filtro.FechaInicio != "" && filtro.FechaFin == "")
                {
                    var fechaInicio = DateTime.ParseExact(filtro.FechaInicio, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
                    auditoriaIngresoLista = _context.AuditoriaIngreso.AsEnumerable().Where(x=> Convert.ToDateTime(x.Fecha) >= fechaInicio).ToList();
                }
                if (filtro.FechaInicio == "" && filtro.FechaFin != "")
                {
                    var fechaFin = DateTime.ParseExact(filtro.FechaFin, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
                    auditoriaIngresoLista = _context.AuditoriaIngreso.AsEnumerable().Where(x => Convert.ToDateTime(x.Fecha) <= fechaFin).ToList();
                }
                if (filtro.FechaInicio != "" && filtro.FechaFin != "")
                {
                    var fechaInicio = DateTime.ParseExact(filtro.FechaInicio, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
                    var fechaFin = DateTime.ParseExact(filtro.FechaFin, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
                    auditoriaIngresoLista = _context.AuditoriaIngreso.AsEnumerable().Where(x => Convert.ToDateTime(x.Fecha) >= fechaInicio && Convert.ToDateTime(x.Fecha) <= fechaFin).ToList();
                }
                contieneFecha = true;
            }
            if (auditoriaIngresoLista.Count == 0 && contieneFecha)
            {
                return auditoriaIngresoLista.Cast<dynamic>().ToList(); ;
            }
            if (auditoriaIngresoLista.Count == 0)
            {
                auditoriaIngresoLista = _context.AuditoriaIngreso.ToList();
            }

            var cambiosQuery = auditoriaIngresoLista.AsQueryable();
            if (filtro.Usuario != "")
            {
                cambiosQuery = cambiosQuery.Where(x => x.Usuario == filtro.Usuario);
            }
            if (filtro.Accion != "")
            {
                cambiosQuery = cambiosQuery.Where(x => x.Accion == filtro.Accion);
            }
            if (filtro.Modulo != "")
            {
                cambiosQuery = cambiosQuery.Where(x => x.Modulo == filtro.Modulo);
            }
            var auditoria = cambiosQuery.Select(x => new
            {
                id = x.Id,
                usuario = x.Usuario,
                accion = x.Accion,
                eventoRealizado = x.EventoRealizado,
                fecha = x.Fecha,
                modulo = x.Modulo,
                ip = x.Ip
            }).ToList();
            return auditoria.OrderByDescending(r => r.id).Take(1000).Cast<dynamic>().ToList();
        }

        public void RegistrarBusquedaSesiones(DatosToken usuariosConectado, string eventoRealizado, string ip)
        {
            try
            {
                AuditoriaCambios auditoriaCambios = new AuditoriaCambios();
                auditoriaCambios.Usuario = usuariosConectado.Usuario;
                auditoriaCambios.Accion = "Busqueda";
                auditoriaCambios.EventoRealizado = eventoRealizado;
                auditoriaCambios.Modulo = "Auditoria";
                auditoriaCambios.Fecha = DateTime.Now.ToString();
                auditoriaCambios.Ip = ip;
                _context.AuditoriaCambios.Add(auditoriaCambios);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public void RegistrarExpulsion(DatosToken datosToken, string razon, string ip)
        {
            try
            {
                AuditoriaIngreso auditoriaIngreso = new AuditoriaIngreso();
                auditoriaIngreso.Usuario = datosToken.Usuario;
                auditoriaIngreso.Accion = "Salida";
                if(razon == "cerrarSesion")
                {
                    auditoriaIngreso.EventoRealizado = "Salio de la aplicación";
                }else 
                if(razon == "Expulsado")
                {
                    auditoriaIngreso.EventoRealizado = "Fue expulsado de la aplicación";
                }
                else
                {
                    auditoriaIngreso.EventoRealizado = "";
                }
                auditoriaIngreso.Modulo = "Aplicación";
                auditoriaIngreso.Fecha = DateTime.Now.ToString();
                auditoriaIngreso.Ip = ip;
                _context.AuditoriaIngreso.Add(auditoriaIngreso);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public void RegistrarIngreso(DatosToken datosToken, string ip)
        {
            try
            {
                AuditoriaIngreso auditoriaIngreso = new AuditoriaIngreso();
                auditoriaIngreso.Usuario = datosToken.Usuario;
                auditoriaIngreso.Accion = "Ingreso";
                auditoriaIngreso.EventoRealizado = "Ingreso a la aplicación";
                auditoriaIngreso.Modulo = "Aplicación";
                auditoriaIngreso.Fecha = DateTime.Now.ToString();
                auditoriaIngreso.Ip = ip;
                _context.AuditoriaIngreso.Add(auditoriaIngreso);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public void RegistrarIngresoFallido(string usuario, string ip, string mensage)
        {
            try
            {
                AuditoriaIngreso auditoriaIngreso = new AuditoriaIngreso();
                auditoriaIngreso.Usuario = usuario;
                auditoriaIngreso.Accion = "Ingreso";
                auditoriaIngreso.EventoRealizado = "Ingreso fallido, "+mensage;
                auditoriaIngreso.Modulo = "Aplicación";
                auditoriaIngreso.Fecha = DateTime.Now.ToString();
                auditoriaIngreso.Ip = ip;
                _context.AuditoriaIngreso.Add(auditoriaIngreso);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public void RegistrarSalida(DatosToken datosToken, string ip)
        {
            try
            {
                AuditoriaIngreso auditoriaIngreso = new AuditoriaIngreso();
                auditoriaIngreso.Usuario = datosToken.Usuario;
                auditoriaIngreso.Accion = "Salida";
                auditoriaIngreso.EventoRealizado = "Salio de la aplicación";
                auditoriaIngreso.Modulo = "Aplicación";
                auditoriaIngreso.Fecha = DateTime.Now.ToString();
                auditoriaIngreso.Ip = ip;
                _context.AuditoriaIngreso.Add(auditoriaIngreso);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public void RegistrarSalidaTokenExpirado(string usuario, string ip)
        {
            try
            {
                AuditoriaIngreso auditoriaIngreso = new AuditoriaIngreso();
                auditoriaIngreso.Usuario = usuario;
                auditoriaIngreso.Accion = "Salida";
                auditoriaIngreso.EventoRealizado = "Salio de la aplicación por token expirado";
                auditoriaIngreso.Modulo = "Aplicación";
                auditoriaIngreso.Fecha = DateTime.Now.ToString();
                auditoriaIngreso.Ip = ip;
                _context.AuditoriaIngreso.Add(auditoriaIngreso);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
