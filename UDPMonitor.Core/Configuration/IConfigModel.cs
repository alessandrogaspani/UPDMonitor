namespace UDPMonitor.Core.Configuration
{
    public interface IConfigModel
    {
        int ConfigVersion { get; set; }
        Guid SchemaID { get; set; }
        string Software { get; set; }
    }
}
