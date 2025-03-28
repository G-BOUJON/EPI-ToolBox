using MFilesAPI;

namespace ToolBox_MVC.Services
{
    public static class TranslatorService
    {
        public readonly static Dictionary<MFLicenseType, string> LicenseDictionnary = new Dictionary<MFLicenseType, string>()
        {
            {MFLicenseType.MFLicenseTypeReadOnlyLicense,"Lecture Seule" },
            {MFLicenseType.MFLicenseTypeNamedUserLicense, "Nominative" },
            {MFLicenseType.MFLicenseTypeConcurrentUserLicense, "Concurrente" }
        };

        public static string TranslateMFLicense(MFLicenseType licenseType)
        {
            return LicenseDictionnary[licenseType];
        }

        
    }
}
