using UDPMonitor.Business.Models;

namespace UDPMonitor.Business.Interfaces
{
    public delegate void InMessageReceivedEventHandler(InMessage message);

    public interface IInboundService
    {
        event InMessageReceivedEventHandler OnInboundMessageReceived;

        void StartListen(int port);
        void StopListen(string port);
    }
}