using System;
using System.Net;
using System.Net.Sockets;

namespace UDPMonitor.Core.Network.Udp
{
    internal sealed class UdpOutChannel : IDisposable
    {
        private readonly IPEndPoint _remote;
        private readonly UdpClient _udpClient;
        private readonly Action<byte[]> _onBytesTransmitted;
        private readonly object _sendLock = new object();

        public UdpOutChannel(int port, string ipAddress, Action<byte[]> onBytesTransmitted = null)
        {
            if (!IPAddress.TryParse(ipAddress, out var ip))
                throw new ArgumentException("Invalid IP address", nameof(ipAddress));

            _remote = new IPEndPoint(ip, port);
            _udpClient = new UdpClient();
            _onBytesTransmitted = onBytesTransmitted;
        }

        public void Send(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            lock (_sendLock)
            {
                _udpClient.Send(data, data.Length, _remote);
            }

            _onBytesTransmitted?.Invoke(data);
        }

        public void Dispose()
        {
            _udpClient.Dispose();
        }

        public void CloseChannel()
        {
            Dispose();
        }
    }
}