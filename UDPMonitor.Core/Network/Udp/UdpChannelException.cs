using System;

namespace UDPMonitor.Core.Network.Udp
{
    public class UdpChannelException : Exception
    {
        public UdpChannelException() : base()
        {
        }

        public UdpChannelException(string message) : base(message)
        {
        }

        public UdpChannelException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}