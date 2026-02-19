namespace UDPMonitor.Core.Configuration
{
    public class ConfigModel : IConfigModel
    {
        public string Software { get; set; }
        public int ConfigVersion { get; set; }
        public Guid SchemaID { get; set; }
    }
}