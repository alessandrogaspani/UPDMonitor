using System.Text.Json.Nodes;
using UDPMonitor.Core.Configuration;

namespace UDPMonitor.Business.Configuration.Migrations
{
    public class _01_FirstMigration : JsonConfigBaseMigration
    {
        public override Guid GetSchemaID()
        {
            return new Guid("B1F8C9E3-5A6D-4E2F-9A7B-1234567890AB");
        }

        public override int GetSchemaVersion()
        {
            return 1;
        }

        public override JsonObject Migrate(JsonObject oldConfig)
        {
            var defaultConfig = GenerateDefaultConfig();

            var updateRules = new Dictionary<string, bool>
            {
                { "Outbound.Comment", true },
                { "Inbound.Comment", true }
            };

            var updatedConfig = UpdateFrom(oldConfig, defaultConfig, updateRules);

            return updatedConfig;
        }

        private static JsonObject GenerateDefaultConfig()
        {
            var config = new JsonObject
            {
                ["Inbound"] = new JsonObject
                {
                    ["Comment"] = "Default inbound configuration",
                    ["Port"] = 500
                },
                ["Outbound"] = new JsonObject
                {
                    ["Comment"] = "Default outbound configuration",
                    ["IP"] = "127.0.0.1",
                    ["Port"] = 500
                },
            };

            return config;
        }
    }
}