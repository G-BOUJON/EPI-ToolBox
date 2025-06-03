using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Repositories;
using ToolBox_MVC.Services.ActiveDirectory.Sync;
using ToolBox_MVC.Services.MFiles;
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
                    await ExecuteSyncOnActiveDirectories();

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
                var scopedSyncService = scope.ServiceProvider.GetRequiredService<IMfSyncService>();
                var scopedActivationService = scope.ServiceProvider.GetRequiredService<IMfilesAccountActivationHandler>();

                if (scopedSyncService == null)
                {
                    return;
                }
                if (!scopedSyncService.TryConnections(server.Id))
                {
                    _logger.LogError("Connection to server {Server} impossible. Execution aborted.", server.Name);
                    return;
                }

                await scopedSyncService.SyncAccountsAsync(server.Id);
                await scopedSyncService.SyncGroupsAsync(server.Id);

                if (server.AutomaticOP.AutoActivationHandling)
                {
                    foreach (var account in await scopedActivationService.GetAccountsToReactivateAsync(server.Id))
                    {
                        try
                        {
                            await scopedActivationService.ModifyMFilesAccountStatus(server.Id, account.UserId, true, true);
                            
                        }
                        catch
                        {
                            // error handling
                        }
                    }
                    _logger.LogInformation("{Time} : Reativation finished on server {Server}", TimeOnly.FromDateTime(DateTime.Now), server.Name);
                }
            
            }

            _logger.LogInformation("{Time} : Operation finished on server {Server}", TimeOnly.FromDateTime(DateTime.Now), server.Name);
        }

        private async Task ExecuteSyncOnActiveDirectories()
        {
            using (var scope = _serviceScope.CreateScope())
            {
                var adRepo = scope.ServiceProvider.GetRequiredService<IGenericRepository<Models.ActiveDirectory>>();
                var syncAd = scope.ServiceProvider.GetRequiredService<IActiveDirectorySyncService>();

                foreach (var ad in await adRepo.GetAllAsync())
                {
                    if (ad.LastSync < DateOnly.FromDateTime(DateTime.Now))
                    {
                        _logger.LogInformation("{Time} : Sync started on AD {Server}", TimeOnly.FromDateTime(DateTime.Now), ad.Name);
                        await syncAd.SyncActiveDirectory(ad.ID);
                    }
                }
            }
        }
    }
}
