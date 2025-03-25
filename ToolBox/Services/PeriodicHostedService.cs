namespace ToolBox.Services
{
    public class PeriodicHostedService : BackgroundService
    {
        private readonly ILogger<PeriodicHostedService> logger;
        private IWebHostEnvironment WebHostEnvironment { get; set; }
        private readonly IServiceScopeFactory factory;
        private readonly TimeSpan period = TimeSpan.FromSeconds(5);
        public bool isEnabled = false;
        public ServerType serverType;

        public PeriodicHostedService(ILogger<PeriodicHostedService> logger, IServiceScopeFactory factory, ServerType server)
        {
            this.logger = logger;
            this.factory = factory;
            this.serverType = server;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Activation du service asynchrone de LicenseManager"+serverType);
            using PeriodicTimer timer = new PeriodicTimer(period);
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await using AsyncServiceScope asyncScope = factory.CreateAsyncScope();
                    SampleService sampleService = asyncScope.ServiceProvider.GetRequiredService<SampleService>();
                    await sampleService.DoSomethingAsync(serverType);
                }
                catch (Exception ex)
                {
                    logger.LogInformation($"Failed to execute with exception message : {ex.Message}");
                }
            }
        }
    }
}
