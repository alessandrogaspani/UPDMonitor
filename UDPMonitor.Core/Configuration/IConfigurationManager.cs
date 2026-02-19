using System.IO;
using System.Text.Json.Nodes;
using UDPMonitor.Core.Extensions;
using UDPMonitor.Core.Serialization;

namespace UDPMonitor.Core.Configuration
{
    public interface IConfigurationManager<out T> where T : ConfigModel, new()
    {
        T Configuration { get; }

        void UpdateConfiguration(bool executeBackup = true);
    }

    public abstract class JsonFile_ConfigManager<T> : IConfigurationManager<T> where T : ConfigModel, new()
    {
        protected string _filePath;
        protected IJsonConfigMigration[] _migrations;

        public T Configuration { get; protected set; }

        protected string _backupFilePath;

        protected JsonFile_ConfigManager(string filePath)
        {
            _filePath = filePath;

            string fileName = Path.GetFileName(filePath);
            fileName = "backup_" + fileName;
            string fileDirectory = Path.GetDirectoryName(filePath);
            _backupFilePath = Path.Combine(fileDirectory, "Backup", fileName);
        }

        public void ReadFromFile()
        {
            ConfigModel configModel = null;

            if (File.Exists(_filePath))
            {
                configModel = JsonConversion.ReadFromFile<ConfigModel>(_filePath);

                if (configModel == default)
                {
                    MoveFileToBadFileDirectory(_filePath);

                    configModel = TryReadFromBackup();
                }
            }
            else
            {
                configModel = TryReadFromBackup();
            }

            if (_migrations.HasElements())
            {
                MigrateConfiguration(configModel);
            }

            Configuration = JsonConversion.ReadFromFile<T>(_filePath);

            string softwareVersion = $"UDPManager {ApplicationService.Version}";

            if (Configuration.Software != softwareVersion)
            {
                Configuration.Software = softwareVersion;

                JsonConversion.WriteToFile(Configuration, _filePath);
            }

            JsonConversion.WriteToFile(Configuration, _backupFilePath);
        }

        private void MigrateConfiguration(ConfigModel configModel)
        {
            if (configModel.ConfigVersion > 0 && !Array.Exists(_migrations, migration => migration.GetSchemaID() == configModel.SchemaID && migration.GetSchemaVersion() == configModel.ConfigVersion))
            {
                MoveFileToBadFileDirectory(_filePath);

                configModel.ConfigVersion = 0;
            }

            int maxSupportedVersion = _migrations.Max(migration => migration.GetSchemaVersion());

            if (configModel.ConfigVersion <= maxSupportedVersion)
            {
                var eligibleMigrations = _migrations
                    .Where(migration => migration.GetSchemaVersion() > configModel.ConfigVersion)
                    .OrderBy(migration => migration.GetSchemaVersion())
                    .ToList();

                foreach (var migration in eligibleMigrations)
                {
                    try
                    {
                        JsonObject configFile = File.Exists(_filePath) ? JsonConversion.ReadFromFile<JsonObject>(_filePath) : new JsonObject();

                        var updatedConfig = migration.Migrate(configFile);

                        updatedConfig["ConfigVersion"] = migration.GetSchemaVersion();
                        updatedConfig["SchemaID"] = migration.GetSchemaID();

                        JsonConversion.WriteToFile(updatedConfig, _filePath);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Errore durante la migrazione: {migration.GetSchemaID()}: {e.Message}");
                    }
                }
            }
        }

        private ConfigModel TryReadFromBackup()
        {
            ConfigModel configModel = null;

            if (File.Exists(_backupFilePath))
            {
                configModel = JsonConversion.ReadFromFile<ConfigModel>(_backupFilePath);

                if (configModel != null)
                {
                    File.Move(_backupFilePath, _filePath);
                }
                else
                {
                    File.Delete(_backupFilePath);
                }
            }

            return configModel ?? new ConfigModel();
        }

        private static void MoveFileToBadFileDirectory(string _filepath)
        {
            string fileName = Path.GetFileName(_filepath);
            DirectoryInfo badFilesDirectory = new DirectoryInfo($@"{ApplicationService.ConfigFolderPath}\BadFiles");
            badFilesDirectory.Create();

            File.Move(_filepath, $@"{badFilesDirectory}\{fileName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}");
        }

        public void UpdateConfiguration(bool executeBackup = true)
        {
            JsonConversion.WriteToFile(Configuration, _filePath);

            if (executeBackup)
            {
                JsonConversion.WriteToFile(Configuration, _backupFilePath);
            }
        }
    }

}
