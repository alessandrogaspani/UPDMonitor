using UDPMonitor.Business.Models;

namespace UDPMonitor.Business.Interfaces
{
    public delegate void InMessageReceivedEventHandler(InMessage message);

    public interface IInboundService
    {
        event InMessageReceivedEventHandler OnInboundMessageReceived;

        public int Port { get; set; }

        void StartListen();

        void StopListen();
    }
}