using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using APIBCPSistemaVigilancia.Models.BDBCPSistemaVigilancia;

namespace SistemaVigilanciaBCPApi.Hubs
{
    public class ControlUsuarioHub: Hub
    {
        private readonly BCPSistemaVigilanciaContext _dbContext;
        private readonly IHubContext<ControlUsuarioHub> _hubContext;

        public ControlUsuarioHub(BCPSistemaVigilanciaContext dbContext, IHubContext<ControlUsuarioHub> hubContext)
        {
            _dbContext = dbContext;
            _hubContext = hubContext;
        }

        public async Task SendUsers()
        {
            var prodList = await _dbContext.UsuariosConectado.ToListAsync();
            foreach (var emp in prodList)
            {
                await _dbContext.Entry(emp).ReloadAsync();
            }
            await _hubContext.Clients.All.SendAsync("ReceivedUsers", prodList);

        }
        public async Task ExpulsarUsuarioPorNuevaSesion(string nombreUsuario)
        {
            await Clients.All.SendAsync("ExpulsarUsuarioPorNuevaSesion", nombreUsuario);
        }
    }
}
