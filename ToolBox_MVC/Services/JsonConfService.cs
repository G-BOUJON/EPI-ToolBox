using System.Configuration;
using System.Text.Json;
using ToolBox_MVC.Models;
using ToolBox_MVC.Services.JsonConverters;

namespace ToolBox_MVC.Services
{
    public class JsonConfService
    {
        private JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            Converters = { new TimeOnlyConverter() },
            WriteIndented = true
        };
        public string ConfigurationJsonFileName { get; private set; }

        public JsonConfService(ServerType server) 
        {
            ConfigurationJsonFileName = FilePathService.LicenseManagerPath(server) + "configuration.json";
        }

        public Config GetConf()
        {
            using (var jsonFileReader = File.OpenText(ConfigurationJsonFileName))
            {
                return JsonSerializer.Deserialize<Config>(jsonFileReader.ReadToEnd(),serializerOptions);
            }
        }

        public void changeHour(TimeOnly hour)
        {
            Config configuration = GetConf();

            configuration.Hour = hour;

            serializeConfig(configuration);
        }

        public void changeActivity(bool activity)
        {
            Config configuration = GetConf();

            configuration.ActiveSuppression = activity;

            serializeConfig(configuration);
        }

        public void changeFrequence(int frequence)
        {
            Config configuration = GetConf();

            configuration.Frequence = frequence;

            serializeConfig(configuration);
        }

        public void deleteGroup(string groupName)
        {
            List<Group> updatedGroups = new List<Group>();
            Config configuration = GetConf();
            IEnumerable<Group> groups = configuration.Groups;
            groups = groups.ToList();

            foreach (Group grp in groups)
            {
                if (grp.name != groupName)
                {
                    updatedGroups.Add(grp);
                }
            }
            configuration.Groups = updatedGroups;

            serializeConfig(configuration);
        }

        public bool addGroup(Group group)
        {
            // Initialisation
            Config configuration = GetConf();
            List<Group> groups = configuration.Groups;
            bool existingGroup = false;

            // Traitement
            foreach (Group grp in groups)
            {
                if (grp.name == group.name)
                {
                    existingGroup = true;
                }
            }

            if (!existingGroup)
            {
                groups.Add(group);
                configuration.Groups = groups;
                serializeConfig(configuration);
            }

            // Sortie
            return !existingGroup;
        }

        public void addMaintainedAccount(string username)
        {

            // Initialisation
            Config configuration = GetConf();

            // Traitement
            configuration.MaintainedAccounts.Add(username);

            // Sortie
            serializeConfig(configuration);
        }

        public void deleteMaintainedAccount(string username)
        {
            // Initialisation
            Config configuration = GetConf();
            List<string> maintainedAccounts = new List<string>();

            // Traitement
            foreach (string acct in configuration.MaintainedAccounts)
            {
                if (acct != username)
                {
                    maintainedAccounts.Add(acct);
                }
            }
            configuration.MaintainedAccounts = maintainedAccounts;

            // Sortie
            serializeConfig(configuration);
        }

        public void serializeConfig(Config configuration)
        {
            File.Delete(ConfigurationJsonFileName);
            using (var outputStream = File.OpenWrite(ConfigurationJsonFileName))
            {
                JsonSerializer.Serialize(
                    new Utf8JsonWriter(outputStream, new JsonWriterOptions
                    {
                        SkipValidation = true,
                        Indented = true
                    }),
                    configuration,
                    serializerOptions
                );
            }
        }

        public List<string> GetMaintainedAccounts()
        {
            return GetConf().MaintainedAccounts;
        }
    }
}
