using APIBCPSistemaVigilancia.Models.BDBCPSistemaVigilancia;
using Microsoft.EntityFrameworkCore;

namespace SistemaVigilanciaBCPApi.Services
{
    public class SeguridadService : ISeguridadService
    {
        private readonly BCPSistemaVigilanciaContext _context;

        public SeguridadService(BCPSistemaVigilanciaContext context)
        {
            _context = context;
        }

        public async Task<List<UsuariosConectado>> ObtenerUsuariosConectados()
        {
            var listaUsuariosConectados = await _context.UsuariosConectado.ToListAsync();
            return listaUsuariosConectados;
        }
        #region Grupos
        public async Task<string> CrearGrupo(Grupo grupoAgregar)
        {
            try
            {
                //Validar si existe un grupo con este nombre
                var buscarGrupo = await _context.Grupo.Where(x => x.Nombre == grupoAgregar.Nombre).FirstOrDefaultAsync();
                if(buscarGrupo != null)
                {
                    return "GrupoExiste";
                }
                await _context.Grupo.AddAsync(grupoAgregar);
                await _context.SaveChangesAsync();
                //var crearNuevosPermisos = await InyectarPermisosGrupos(grupoAgregar.Id);
                return "Ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async Task<string> ModificarGrupo(Grupo grupo)
        {
            try
            {
                var grupoModificar = _context.Grupo.FindAsync(grupo.Id).Result;
                grupoModificar.Nombre = grupo.Nombre;
                grupoModificar.Activo = grupo.Activo;
                await _context.SaveChangesAsync();
                return "Ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<Grupo> ObtenerGrupo(int id)
        {
            var res = await _context.Grupo.FindAsync(id);
            return res;
        }

        public async Task<List<Grupo>> ObtenerGrupos()
        {
            return await _context.Grupo.ToListAsync();
        }

        public async Task<string> ModificarPermisoGrupo(List<PermisoGrupo> permisos)
        {
            try
            {
                //Validar si existe
                var listaPermisosModulos=await _context.PermisoGrupo.Where(x=>x.GrupoId == permisos[0].GrupoId).ToListAsync();
                List<PermisoGrupo> permisosModificar=new List<PermisoGrupo>();
                foreach (var item in permisos)
                {
                    var encontrado = false;
                    foreach (var item1 in listaPermisosModulos)
                    {
                        if(item1.GrupoId == item.GrupoId && item1.ModuloId == item.ModuloId)
                        {
                            item1.PuedeIngresar = item.PuedeIngresar;
                            permisosModificar.Add(item1);
                            encontrado = true;
                            _context.PermisoGrupo.Entry(item1).State = EntityState.Modified;
                            break;
                        }
                    }
                    if (!encontrado)
                    {
                        //Agregar
                        await _context.PermisoGrupo.AddAsync(item);
                    }

                }
                //await _context.PermisoGrupo.AddRangeAsync(permisosModificar);
                await _context.SaveChangesAsync();

                return "Ok";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            
        }

        private async Task<bool> InyectarPermisosGrupos(int idGrupo)
        {
            try
            {
                var listaModulos = await _context.Modulo.ToListAsync();
                var listaPermisosAgregados = new List<PermisoGrupo>();
                foreach (var item in listaModulos)
                {
                    var permisoGrupo = new PermisoGrupo();
                    permisoGrupo.GrupoId = idGrupo;
                    permisoGrupo.ModuloId = item.Id;
                    permisoGrupo.PuedeIngresar = false;
                    listaPermisosAgregados.Add(permisoGrupo);
                }
                await _context.PermisoGrupo.AddRangeAsync(listaPermisosAgregados);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
        #region Permisos de Grupos
        public async Task<List<PermisoGrupo>> ObtenerListaPermisosGrupo(int idGrupo)
        {
            return await _context.PermisoGrupo.Where(x=>x.GrupoId == idGrupo && x.PuedeIngresar == true).ToListAsync();
        }

        public async Task<List<PermisoGrupo>> ObtenerListaPermisosGrupoNombre(string nombreGrupo)
        {
            var idGrupo = await _context.Grupo.Where(x => x.Nombre == nombreGrupo).FirstOrDefaultAsync();
            return await _context.PermisoGrupo.Where(x => x.GrupoId == idGrupo.Id && x.PuedeIngresar == true).ToListAsync();
        }

        public async Task<List<Modulo>> ObtenerListaModulos()
        {
            return await _context.Modulo.ToListAsync();
        }
        #endregion
    }
}
