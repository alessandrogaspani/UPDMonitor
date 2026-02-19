using System.Text.Json.Nodes;

namespace UDPMonitor.Core.Configuration
{
    public interface IJsonConfigMigration
    {
        int GetSchemaVersion();

        Guid GetSchemaID();

        JsonObject Migrate(JsonObject oldConfig);
    }

    public abstract class JsonConfigBaseMigration : IJsonConfigMigration
    {
        public abstract Guid GetSchemaID();

        public abstract int GetSchemaVersion();

        public abstract JsonObject Migrate(JsonObject oldConfig);

        protected JsonObject UpdateFrom(JsonObject oldConfig, JsonObject defaultConfig, Dictionary<string, bool> updateRules, string path = "")
        {
            var result = new JsonObject();

            foreach (var property in defaultConfig)
            {
                string fullPath = string.IsNullOrEmpty(path) ? property.Key : $"{path}.{property.Key}";

                if (oldConfig.ContainsKey(property.Key))
                {
                    var oldValue = oldConfig[property.Key];
                    var defaultValue = property.Value;

                    if (ShouldUpdateProperty(fullPath, updateRules))
                    {
                        result[property.Key] = defaultValue?.DeepClone();
                    }
                    else if (oldValue is JsonObject oldObject && defaultValue is JsonObject defaultObject)
                    {
                        result[property.Key] = UpdateFrom(oldObject, defaultObject, updateRules, fullPath);
                    }
                    else
                    {
                        result[property.Key] = oldValue?.DeepClone();
                    }
                }
                else
                {
                    result[property.Key] = property.Value?.DeepClone();
                }
            }

            return result;
        }

        private static bool ShouldUpdateProperty(string propertyName, Dictionary<string, bool> updateRules)
        {
            return updateRules.TryGetValue(propertyName, out bool shouldUpdate) && shouldUpdate;
        }
    }

}