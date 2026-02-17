namespace UDPMonitor.Business.Interfaces
{
    public delegate void OutMessageSentEventHandler(string content);

    public interface IOutboundService
    {
        event OutMessageSentEventHandler OnOutMessageSent;

        void Connect(string ipAddress, int port);
        void Disconnect();
        void SendMessage(string message);
    }
}