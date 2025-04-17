using Microsoft.EntityFrameworkCore;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;

namespace ToolBox_MVC.Data
{
    public static class ServerSeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ToolBoxDbContext(serviceProvider.GetRequiredService<DbContextOptions<ToolBoxDbContext>>()))
            {
                if (context.MFilesServers.Any())
                {
                    return;
                }
                context.MFilesServers.Add(
                    new MFilesServer
                    {
                        Id = 1,
                        Name = "Production",
                        NetworkAddress = "mfiles.epi.ge.ch",
                        ProtocolSequence = "ncacn_ip_tcp",
                        EndPoint = "2266",
                        VaultGuid = "{D6B60CC3-9531-417A-8819-AFBD4E37DABF}"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
