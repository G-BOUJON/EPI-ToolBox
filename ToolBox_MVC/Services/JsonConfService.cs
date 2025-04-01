using System.Configuration;
using System.Text.Json;
using ToolBox.Models;

namespace ToolBox.Services
{
    public class JsonConfService
    {
        public string ConfigurationJsonFileName { get; private set; }

        public JsonConfService(ServerType server) 
        {
            ConfigurationJsonFileName = FilePathService.LicenseManagerPath(server) + "configuration.json";
        }

        public Config GetConf()
        {
            using (var jsonFileReader = File.OpenText(ConfigurationJsonFileName))
            {
                return JsonSerializer.Deserialize<Config>(jsonFileReader.ReadToEnd());
            }
        }

        public void changeHour(string hour)
        {
            Config configuration = GetConf();

            configuration.hour = hour;

            serializeConfig(configuration);
        }

        public void changeActivity(bool activity)
        {
            Config configuration = GetConf();

            configuration.active = activity;

            serializeConfig(configuration);
        }

        public void changeFrequence(int frequence)
        {
            Config configuration = GetConf();

            configuration.frequence = frequence;

            serializeConfig(configuration);
        }

        public void deleteGroup(string groupName)
        {
            List<Group> updatedGroups = new List<Group>();
            Config configuration = GetConf();
            IEnumerable<Group> groups = configuration.groups;
            groups = groups.ToList();

            foreach (Group grp in groups)
            {
                if (grp.name != groupName)
                {
                    updatedGroups.Add(grp);
                }
            }
            configuration.groups = updatedGroups;

            serializeConfig(configuration);
        }

        public bool addGroup(Group group)
        {
            // Initialisation
            Config configuration = GetConf();
            List<Group> groups = configuration.groups;
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
                configuration.groups = groups;
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
            configuration.maintainedAccounts.Add(username);

            // Sortie
            serializeConfig(configuration);
        }

        public void deleteMaintainedAccount(string username)
        {
            // Initialisation
            Config configuration = GetConf();
            List<string> maintainedAccounts = new List<string>();

            // Traitement
            foreach (string acct in configuration.maintainedAccounts)
            {
                if (acct != username)
                {
                    maintainedAccounts.Add(acct);
                }
            }
            configuration.maintainedAccounts = maintainedAccounts;

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
                    configuration
                );
            }
        }

        public List<string> GetMaintainedAccounts()
        {
            return this.GetConf().maintainedAccounts;
        }
    }
}
