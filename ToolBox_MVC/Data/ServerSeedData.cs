using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using ToolBox_MVC.Areas.LicenseManager.Models.DBModels;
using ToolBox_MVC.Models;
using ToolBox_MVC.Repositories;
using ToolBox_MVC.Services.ActiveDirectory;
using ToolBox_MVC.Services.MFiles;
using ToolBox_MVC.Services.Repository;

namespace ToolBox_MVC.Data
{
    public static class ServerSeedData
    {
        public async static Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ToolBoxDbContext(serviceProvider.GetRequiredService<DbContextOptions<ToolBoxDbContext>>()))
            {
                var serverRepo = new ServerRepository(context);
                var adCredStore = new ADCredentialStore(serverRepo, serviceProvider.GetRequiredService<IDataProtectionProvider>());
                var mfCredStore = new MFilesCredentialStore(serverRepo, serviceProvider.GetRequiredService<IDataProtectionProvider>());

                // Code pour initialisé les valeurs de MFilesServer
                /*

                var prod = await serverRepo.GetByIDAsync(1);
                prod.Name = "Production";
                prod.NetworkAddress = "mfiles.epi.ge.ch";
                prod.ProtocolSequence = "ncacn_ip_tcp";
                prod.EndPoint = "2266";
                prod.VaultGuid = "{D6B60CC3-9531-417A-8819-AFBD4E37DABF}";
                prod.Domain = "EPI";

                var cert = await serverRepo.GetByIDAsync(2);
                cert.Name = "Certification";
                cert.NetworkAddress = "epi-srv-mf-fc";
                cert.ProtocolSequence = "ncacn_ip_tcp";
                cert.EndPoint = "2266";
                cert.VaultGuid = "{4D648C2F-ADA6-4714-A37B-D41FFAFD286B}";
                cert.Domain = "EPI";

                var test = await serverRepo.GetByIDAsync(3);
                test.Name = "Test";
                test.NetworkAddress = "epi-srv-mf-ft";
                test.ProtocolSequence = "ncacn_ip_tcp";
                test.EndPoint = "2266";
                test.VaultGuid = "{26282E76-C66E-4BD0-8772-AA70B6B91B2F}";
                test.Domain = "EPI";

                await serverRepo.SaveChangesAsync();

                var adCred = new ADCredential(domain: "epi-srv-dc.epidom.ch", container: "DC=epidom,DC=ch", encryptedUsername: "EPI\\S_MFiles_T", encryptedPassword: "Epi2@22");
                var adCred2 = new ADCredential(domain: "epi-srv-dc.epidom.ch", container: "DC=epidom,DC=ch", encryptedUsername: "EPI\\S_MFiles_T", encryptedPassword: "Epi2@22");
                var adCred3 = new ADCredential(domain: "epi-srv-dc.epidom.ch", container: "DC=epidom,DC=ch", encryptedUsername: "EPI\\S_MFiles_T", encryptedPassword: "Epi2@22");

                await adCredStore.UpdateCredentials(1, adCred);
                await adCredStore.UpdateCredentials(2, adCred2);
                await adCredStore.UpdateCredentials(3, adCred3);

                var mfcred = new MFilesCredential
                {
                    EncryptedUserName = "intratest01",
                    EncryptedPassword = "Epi2020."
                };
                var mfcred2 = new MFilesCredential
                {
                    EncryptedUserName = "intratest01",
                    EncryptedPassword = "Epi2020."
                };
                var mfcred3 = new MFilesCredential
                {
                    EncryptedUserName = "intratest01",
                    EncryptedPassword = "Epi2020."
                };

                await mfCredStore.UpdateCredentials(1, mfcred);
                await mfCredStore.UpdateCredentials(2, mfcred2);
                await mfCredStore.UpdateCredentials(3, mfcred3);

                */
            }   
        }
    }
}
