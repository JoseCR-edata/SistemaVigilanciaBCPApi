
namespace SistemaVigilanciaBCPApi.Services
{
    public class TareaValidaUsuarioActivo : BackgroundService
    {
        private readonly ILogger<TareaValidaUsuarioActivo> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public TareaValidaUsuarioActivo(ILogger<TareaValidaUsuarioActivo> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Tarea periódica iniciada.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Ejecutando tarea periódica...");

                // Lógica de tu tarea periódica aquí
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    // Ejemplo: Realizar alguna tarea en un servicio registrado en el contenedor de servicios
                    var someService = scope.ServiceProvider.GetRequiredService<ILoginService>();
                    await someService.ExpulsarUsuariosInactivos();
                }

                // Esperar 5 minutos antes de ejecutar la siguiente iteración
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }

            _logger.LogInformation("Tarea periódica detenida.");
        }
    }
}
