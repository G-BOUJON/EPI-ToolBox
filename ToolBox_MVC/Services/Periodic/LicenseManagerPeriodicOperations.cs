using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Repositories;
using ToolBox_MVC.Services.MFiles.Sync;

namespace ToolBox_MVC.Services.Periodic
{
    public class LicenseManagerPeriodicOperations : IPeriodicOperations
    {
        private readonly IServiceScopeFactory _serviceScope;
        private readonly List<MFilesServer> Servers;

        private readonly ILogger _logger;

        public LicenseManagerPeriodicOperations(IServiceScopeFactory serviceProvider, IServerRepository serverRepo, ILogger<LicenseManagerPeriodicOperations> logger)
        {
            _serviceScope = serviceProvider;
            Servers = Task.Run(serverRepo.GetAllAsync).Result;
            _logger = logger;
        }

        public async Task DoWork()
        {
            var currentTime = TimeOnly.FromDateTime(DateTime.Now);

            var taskList = new List<Task>();

            foreach (var server in Servers)
            {
                if (RightHour(server, currentTime))
                {
                    taskList.Add(ExecuteJobsOnServerAsync(server));
                    _logger.LogInformation("{Time} : Operation started on server {Server}",TimeOnly.FromDateTime(DateTime.Now), server.Name);
                }
            }

            await Task.WhenAll(taskList);
            _logger.LogInformation("{Time} : All operation finished", TimeOnly.FromDateTime(DateTime.Now));
        }

        private bool RightHour(MFilesServer server, TimeOnly hourMinutes) 
        {
            return (server.SyncTime.Minute == hourMinutes.Minute
                && server.SyncTime.Hour == hourMinutes.Hour);
        }

        private async Task ExecuteJobsOnServerAsync(MFilesServer server)
        {
            using (var scope = _serviceScope.CreateScope())
            {
                var scopedSyncService = scope.ServiceProvider.GetRequiredService<ISyncService>();
                var scopedActivationService = scope.ServiceProvider.GetRequiredService<IMfilesAccountActivationHandler>();

                if (scopedSyncService == null)
                {
                    return;
                }

                await scopedSyncService.SyncAccountsAsync(server.Id);
                await scopedSyncService.SyncGroupsAsync(server.Id);

                // VOIR COMMENT GERER LES COMPTES QUI DOIVENT RESTER ACTIF MAIS N'EXISTE QUE SUR MFILES
                //if (server.AutomaticOP.AutoActivationHandling)
                //{
                //    await scopedActivationService.ModifyAllIncorrectAccounts(server.Id);
                //}
            
            }

            _logger.LogInformation("{Time} : Operation finished on server {Server}", TimeOnly.FromDateTime(DateTime.Now), server.Name);
        }
    }
}
