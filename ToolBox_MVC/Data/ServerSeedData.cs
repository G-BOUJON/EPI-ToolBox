using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Services.ActiveDirectory;
using ToolBox_MVC.Services.MFiles;
using ToolBox_MVC.Services.Repository;

namespace ToolBox_MVC.Data
{
    public static class ServerSeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ToolBoxDbContext(serviceProvider.GetRequiredService<DbContextOptions<ToolBoxDbContext>>()))
            {
                var adCredStore = new ADCredentialStore(context, serviceProvider.GetRequiredService<IDataProtectionProvider>());
                adCredStore.AddOrUpdateCredential(new ADConnexionInfos()
                {
                    Container = "DC=epidom,DC=ch",
                    Domain = "epi-srv-dc.epidom.ch",
                    Username = "EPI\\S_MFiles_T",
                    Password = "Epi2@22"
                });
                if (context.MFilesServers.Any())
                {
                    return;
                }
                context.MFilesServers.Add(
                    new MFilesServer
                    {
                        Name = "Production",
                        NetworkAddress = "mfiles.epi.ge.ch",
                        ProtocolSequence = "ncacn_ip_tcp",
                        EndPoint = "2266",
                        VaultGuid = "{D6B60CC3-9531-417A-8819-AFBD4E37DABF}",
                        ADCredentialId = 1
                    }
                );
                context.MFilesServers.Add(
                    new MFilesServer
                    {
                        Name = "Certification",
                        NetworkAddress = "epi-srv-mf-fc",
                        ProtocolSequence = "ncacn_ip_tcp",
                        EndPoint = "2266",
                        VaultGuid = "{4D648C2F-ADA6-4714-A37B-D41FFAFD286B}",
                        ADCredentialId = 1
                    }
                );
                context.MFilesServers.Add(
                    new MFilesServer
                    {
                        Name = "Test",
                        NetworkAddress = "epi-srv-mf-ft",
                        ProtocolSequence = "ncacn_ip_tcp",
                        EndPoint = "2266",
                        VaultGuid = "{26282E76-C66E-4BD0-8772-AA70B6B91B2F}",
                        ADCredentialId = 1
                    }
                );
                context.SaveChanges();
                var credStore = new MFilesCredentialStore(context, serviceProvider.GetRequiredService<IDataProtectionProvider>());
                credStore.UpdateCredentials(new Models.Credentials("intratest01", "Epi2020.", "EPI"), 1);
                credStore.UpdateCredentials(new Models.Credentials("intratest01", "Epi2020.", "EPI"), 2);
                credStore.UpdateCredentials(new Models.Credentials("intratest01", "Epi2020.", "EPI"), 3);
            }
            
        }
    }
}
