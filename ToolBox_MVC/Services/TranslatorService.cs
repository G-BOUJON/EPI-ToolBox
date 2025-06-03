using MFilesAPI;
using ToolBox_MVC.Areas.LicenseManager.Services;

namespace ToolBox_MVC.Services
{
    public static class TranslatorService
    {
        public readonly static Dictionary<MFLicenseType, string> LicenseDictionnary = new Dictionary<MFLicenseType, string>()
        {
            {MFLicenseType.MFLicenseTypeReadOnlyLicense,"Lecture Seule" },
            {MFLicenseType.MFLicenseTypeNamedUserLicense, "Nominative" },
            {MFLicenseType.MFLicenseTypeConcurrentUserLicense, "Concurrente" },
            {MFLicenseType.MFLicenseTypeNone, "Aucune Licence" }
        };

        public readonly static Dictionary<MFLoginAccountType, string> AccountTypeDictionnary = new Dictionary<MFLoginAccountType, string>()
        {
            {MFLoginAccountType.MFLoginAccountTypeWindows, "Compte Windows" },
            {MFLoginAccountType.MFLoginAccountTypeMFiles, "Compte M-Files" }
        };

        public readonly static Dictionary<OperationType, string> OperationTypeDictionnary = new Dictionary<OperationType, string>()
        {
            {OperationType.Activation, "Réactivation" },
            {OperationType.Restoration, "Restauration" },
            {OperationType.Removal, "Retrait" }
        };

        public static string TranslateMFLicense(MFLicenseType licenseType)
        {
            return LicenseDictionnary[licenseType];
        }

        public static string TranslateMFAccountType(MFLoginAccountType accountType)
        {
            return AccountTypeDictionnary[accountType];
        }

        

        
    }
}
