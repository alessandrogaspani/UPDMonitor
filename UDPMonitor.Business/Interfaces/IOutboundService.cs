namespace UDPMonitor.Business.Interfaces
{
    public delegate void OutMessageSentEventHandler(string content);

    public interface IOutboundService
    {
        event OutMessageSentEventHandler OnOutMessageSent;

        int Port { get; set; }
        string IPAddress { get; set; }

        void Connect();
        void Disconnect();
        void SendMessage(string message);
    }
}