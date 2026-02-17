using UDPMonitor.Business.Interfaces;
using UDPMonitor.Business.Models;
using UDPMonitor.Core.Network.Udp;

namespace UDPMonitor.Business
{
    public class InboundService : IInboundService
    {
        private const string InChannelTag = "inChannel";

        public event InMessageReceivedEventHandler OnInboundMessageReceived;

        public InboundService()
        {
        }

        public void StartListen(int port)
        {
            UdpService.AddInChannel(InChannelTag, port, OnDataReceived);
            UdpService.StartReceiving(InChannelTag);
        }

        private void OnDataReceived(byte[] bytes)
        {
            var message = new InMessage()
            {
                TimeStamp = DateTime.Now,
                Text = System.Text.Encoding.UTF8.GetString(bytes),
            };

            OnInboundMessageReceived?.Invoke(message);
        }

        public void StopListen(string port)
        {
            UdpService.StopReceiving(InChannelTag);
            UdpService.CloseInChannel(InChannelTag);
        }
    }
}