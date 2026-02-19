using System.Text.Json.Serialization;
using UDPMonitor.Business.Configuration.Migrations;
using UDPMonitor.Core.Configuration;

namespace UDPMonitor.Business.Config
{
    [Serializable]
    public class UDPManagerConfiguration : ConfigModel
    {
        public InboundConfiguration Inbound { get; set; }
        public OutboundConfiguration Outbound { get; set; }
    }

    [Serializable]
    public class InboundConfiguration
    {
        public string Comment { get; set; }
        public int Port { get; set; }
    }

    [Serializable]
    public class OutboundConfiguration
    {
        public string Comment { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
    }

    public class ConfigFile_JsonConfigManager : JsonFile_ConfigManager<UDPManagerConfiguration>
    {
        public ConfigFile_JsonConfigManager(string filePath) : base(filePath)
        {
            _migrations = new IJsonConfigMigration[]
            {
               new _01_FirstMigration()
            };
        }
    }
}