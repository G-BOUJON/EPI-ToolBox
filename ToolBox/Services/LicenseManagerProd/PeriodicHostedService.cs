﻿namespace ToolBox.Services.LicenseManagerProd
{
    public class PeriodicHostedService : BackgroundService
    {
        private readonly ILogger<PeriodicHostedService> logger;
        private IWebHostEnvironment WebHostEnvironment { get; set; }
        private readonly IServiceScopeFactory factory;
        private readonly TimeSpan period = TimeSpan.FromSeconds(5);
        public bool isEnabled = false;

        public PeriodicHostedService(ILogger<PeriodicHostedService> logger, IServiceScopeFactory factory)
        {
            this.logger = logger;
            this.factory = factory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Activation du service asynchrone de LicenseManagerProd");
            using PeriodicTimer timer = new PeriodicTimer(period);
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await using AsyncServiceScope asyncScope = factory.CreateAsyncScope();
                    SampleService sampleService = asyncScope.ServiceProvider.GetRequiredService<SampleService>();
                    await sampleService.DoSomethingAsync();
                }
                catch (Exception ex)
                {
                    logger.LogInformation($"Failed to execute with exception message : {ex.Message}");
                }
            }
        }
    }
}
