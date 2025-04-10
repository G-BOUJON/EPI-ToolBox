using System.Configuration;
using System.Text.Json;
using ToolBox_MVC.Models;
using ToolBox_MVC.Services.JsonConverters;

namespace ToolBox_MVC.Services.JsonServices
{
    public class JsonConfService : IConfigurationHandler
    {
        private JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            Converters = { new TimeOnlyConverter() },
            WriteIndented = true
        };
        public string ConfigurationJsonFileName { get; private set; }
        public ServerType Server {  get; private set; }

        public JsonConfService(ServerType server) 
        {
            Server = server;
            ConfigurationJsonFileName = FilePathService.LicenseManagerPath(server) + "configuration.json";
        }

        public Config GetConfiguration()
        {
            using (var jsonFileReader = File.OpenText(ConfigurationJsonFileName))
            {
                return JsonSerializer.Deserialize<Config>(jsonFileReader.ReadToEnd(),serializerOptions);
            }
        }

        public void changeHour(TimeOnly hour)
        {
            Config configuration = GetConfiguration();

            configuration.Hour = hour;

            UpdateConfiguration(configuration);
        }

        public void changeActivity(bool activity)
        {
            Config configuration = GetConfiguration();

            configuration.ActiveSuppression = activity;

            UpdateConfiguration(configuration);
        }

        public void changeFrequence(int frequence)
        {
            Config configuration = GetConfiguration();

            configuration.Frequence = frequence;

            UpdateConfiguration(configuration);
        }

        public void deleteGroup(string groupName)
        {
            List<Group> updatedGroups = new List<Group>();
            Config configuration = GetConfiguration();
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

            UpdateConfiguration(configuration);
        }

        public bool addGroup(Group group)
        {
            // Initialisation
            Config configuration = GetConfiguration();
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
                UpdateConfiguration(configuration);
            }

            // Sortie
            return !existingGroup;
        }

        public void AddMaintainedAccount(string username)
        {

            // Initialisation
            Config configuration = GetConfiguration();

            // Traitement
            configuration.MaintainedAccounts.Add(username);

            // Sortie
            UpdateConfiguration(configuration);
        }

        public void RemoveMaintainedAccount(string username)
        {
            // Initialisation
            Config configuration = GetConfiguration();
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
            UpdateConfiguration(configuration);
        }

        public void UpdateConfiguration(Config configuration)
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
            return GetConfiguration().MaintainedAccounts;
        }
    }
}
