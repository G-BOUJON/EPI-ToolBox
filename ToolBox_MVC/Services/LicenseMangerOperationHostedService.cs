using ToolBox_MVC.Services.Factories;

namespace ToolBox_MVC.Services
{
    public class LicenseMangerOperationHostedService : BackgroundService
    {
        private PeriodicTimer Timer = new PeriodicTimer(new TimeSpan(0,1,0));

        public IServiceProvider Services { get; set; }

        public LicenseMangerOperationHostedService(IServiceProvider serviceProvider) 
        { 
            Services = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = Services.CreateScope())
            {
                var scopedService = scope.ServiceProvider.GetService<IPeriodicOperations>();

                while (!stoppingToken.IsCancellationRequested && await Timer.WaitForNextTickAsync(stoppingToken))
                {
                    await scopedService.DoWork();
                }
            }
        }
    }
}
