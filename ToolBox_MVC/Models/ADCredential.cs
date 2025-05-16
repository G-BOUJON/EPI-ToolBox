using Microsoft.EntityFrameworkCore;

namespace ToolBox_MVC.Models
{
    [Owned]
    public class ADCredential
    {
        public string Domain { get; set; }
        public string Container { get; set; }
        public string EncryptedUsername { get; set; }
        public string EncryptedPassword { get; set; }

        public ADCredential(string domain, string container, string encryptedUsername, string encryptedPassword)
        {
            Domain = domain;
            Container = container;
            EncryptedUsername = encryptedUsername;
            EncryptedPassword = encryptedPassword;
        }

        public ADCredential(ADCredential adcred)
        {
            Domain = adcred.Domain;
            Container = adcred.Container;
            EncryptedUsername = adcred.EncryptedUsername;
            EncryptedPassword = adcred.EncryptedPassword;
        }
    }
}
