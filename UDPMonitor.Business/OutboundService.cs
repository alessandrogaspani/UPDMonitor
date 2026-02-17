using UDPMonitor.Business.Interfaces;
using UDPMonitor.Core.Network.Udp;

namespace UDPMonitor.Business
{
    public class OutboundService : IOutboundService
    {
        private const string OutChanngelTag = "outChannel";

        public event OutMessageSentEventHandler OnOutMessageSent;

        public OutboundService()
        {
        }

        public void Connect(string ipAddress, int port)
        {
            UdpService.AddOutChannel(OutChanngelTag, port, ipAddress, OnMessageSent);
        }

        private void OnMessageSent(byte[] data)
        {
            string message = System.Text.Encoding.UTF8.GetString(data);

            OnOutMessageSent?.Invoke(message);
        }

        public void SendMessage(string message)
        {
            UdpService.Send(OutChanngelTag, message);
        }

        public void Disconnect()
        {
            UdpService.CloseOutChannel(OutChanngelTag);
        }
    }
}