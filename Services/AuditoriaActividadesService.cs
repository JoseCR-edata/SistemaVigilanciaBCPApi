using APIBCPSistemaVigilancia.Models.BDBCPSistemaVigilancia;
using Microsoft.EntityFrameworkCore;
using SistemaVigilanciaBCPApi.Models;
using System.Globalization;

namespace SistemaVigilanciaBCPApi.Services
{
    public class AuditoriaActividadesService : IAuditoriaActividadesService
    {
        private readonly BCPSistemaVigilanciaContext _context;

        public AuditoriaActividadesService(BCPSistemaVigilanciaContext context)
        {
            _context = context;
        }

        public List<dynamic> ObtenerAuditoriaCambios(AuditoriaFiltro filtro)
        {
            var contieneFecha = false;
            //Primero filtrar por fecha si tiene
            var auditoriaActividadesLista = new List<AuditoriaCambios>();
            if (filtro.FechaInicio != "" || filtro.FechaFin != "")
            {
                if (filtro.FechaInicio != "" && filtro.FechaFin == "")
                {
                    var fechaInicio = DateTime.ParseExact(filtro.FechaInicio, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
                    auditoriaActividadesLista = _context.AuditoriaCambios.AsEnumerable().Where(x => Convert.ToDateTime(x.Fecha) >= fechaInicio).ToList();
                }
                if (filtro.FechaInicio == "" && filtro.FechaFin != "")
                {
                    var fechaFin = DateTime.ParseExact(filtro.FechaFin, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
                    auditoriaActividadesLista = _context.AuditoriaCambios.AsEnumerable().Where(x => Convert.ToDateTime(x.Fecha) <= fechaFin).ToList();
                }
                if (filtro.FechaInicio != "" && filtro.FechaFin != "")
                {
                    var fechaInicio = DateTime.ParseExact(filtro.FechaInicio, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
                    var fechaFin = DateTime.ParseExact(filtro.FechaFin, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
                    auditoriaActividadesLista = _context.AuditoriaCambios.AsEnumerable().Where(x => Convert.ToDateTime(x.Fecha) >= fechaInicio && Convert.ToDateTime(x.Fecha) <= fechaFin).ToList();
                }
                contieneFecha = true;
            }
            if (auditoriaActividadesLista.Count == 0 && contieneFecha)
            {
                return auditoriaActividadesLista.Cast<dynamic>().ToList(); ;
            }
            if (auditoriaActividadesLista.Count == 0)
            {
                auditoriaActividadesLista = _context.AuditoriaCambios.ToList();
            }

            var cambiosQuery = auditoriaActividadesLista.AsQueryable();
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

        public void RegistrarBusquedaActividades(DatosToken usuariosConectado, string eventoRealizado, string ip)
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

        public void RegistrarBusquedaAlarma(DatosToken usuariosConectado, string eventoRealizado,string ip)
        {
            try
            {
                AuditoriaCambios auditoriaCambios = new AuditoriaCambios();
                auditoriaCambios.Usuario = usuariosConectado.Usuario;
                auditoriaCambios.Accion = "Busqueda";
                auditoriaCambios.EventoRealizado = eventoRealizado;
                auditoriaCambios.Modulo = "Alarmas";
                auditoriaCambios.Fecha = DateTime.Now.ToString();
                auditoriaCambios.Ip = ip;
                _context.AuditoriaCambios.Add(auditoriaCambios);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public void RegistrarBusquedaDetalleAlarma(DatosToken usuariosConectado, string idAlarma, string ip)
        {
            try
            {
                AuditoriaCambios auditoriaCambios = new AuditoriaCambios();
                auditoriaCambios.Usuario = usuariosConectado.Usuario;
                auditoriaCambios.Accion = "Busqueda";
                auditoriaCambios.EventoRealizado = "Ingreso al detalle de la alarma " + idAlarma;
                auditoriaCambios.Modulo = "Alarmas";
                auditoriaCambios.Fecha = DateTime.Now.ToString();
                auditoriaCambios.Ip = ip;
                _context.AuditoriaCambios.Add(auditoriaCambios);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public void RegistrarCrearGrupo(DatosToken usuariosConectado, string nombreGrupo, string ip)
        {
            try
            {
                AuditoriaCambios auditoriaCambios = new AuditoriaCambios();
                auditoriaCambios.Usuario = usuariosConectado.Usuario;
                auditoriaCambios.Accion = "Crear";
                auditoriaCambios.EventoRealizado = "Creo el grupo " + nombreGrupo;
                auditoriaCambios.Modulo = "Permisos de Acceso";
                auditoriaCambios.Fecha = DateTime.Now.ToString();
                auditoriaCambios.Ip = ip;
                _context.AuditoriaCambios.Add(auditoriaCambios);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public void RegistrarExpulsionUsuario(DatosToken usuariosConectado,string usuarioExpulsado, string ip)
        {
            try
            {
                AuditoriaCambios auditoriaCambios = new AuditoriaCambios();
                auditoriaCambios.Usuario = usuariosConectado.Usuario;
                auditoriaCambios.Accion = "Expulsión";
                auditoriaCambios.EventoRealizado = "El usuario " + usuariosConectado.Usuario + " Expulso al usuario " + usuarioExpulsado;
                auditoriaCambios.Modulo = "Alarmas";
                auditoriaCambios.Fecha = DateTime.Now.ToString();
                auditoriaCambios.Ip = ip;
                _context.AuditoriaCambios.Add(auditoriaCambios);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public void RegistrarIngresoGrupoDetalle(DatosToken usuariosConectado,string nombreGrupo, string ip)
        {
            try
            {
                AuditoriaCambios auditoriaCambios = new AuditoriaCambios();
                auditoriaCambios.Usuario = usuariosConectado.Usuario;
                auditoriaCambios.Accion = "Busqueda";
                auditoriaCambios.EventoRealizado = "Ingreso al detalle del grupo " + nombreGrupo;
                auditoriaCambios.Modulo = "Permisos de Acceso";
                auditoriaCambios.Fecha = DateTime.Now.ToString();
                auditoriaCambios.Ip = ip;
                _context.AuditoriaCambios.Add(auditoriaCambios);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public void RegistrarModificarPermisosGrupo(DatosToken usuariosConectado, string eventoRealizado, string ip)
        {
            try
            {
                AuditoriaCambios auditoriaCambios = new AuditoriaCambios();
                auditoriaCambios.Usuario = usuariosConectado.Usuario;
                auditoriaCambios.Accion = "Busqueda";
                auditoriaCambios.EventoRealizado = eventoRealizado;
                auditoriaCambios.Modulo = "Alarmas";
                auditoriaCambios.Fecha = DateTime.Now.ToString();
                auditoriaCambios.Ip = ip;
                _context.AuditoriaCambios.Add(auditoriaCambios);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
