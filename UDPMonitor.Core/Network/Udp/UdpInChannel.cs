using System.Net;
using System.Net.Sockets;

namespace UDPMonitor.Core.Network.Udp
{
    public class UdpInChannel : IDisposable
    {
        private readonly UdpClient _udpClient;
        private readonly Action<byte[]> _onBytesReceived;

        private CancellationTokenSource _cts;
        private Task _listeningTask;

        public bool IsListening { get; private set; }

        public UdpInChannel(int port, Action<byte[]> onBytesReceived)
        {
            _udpClient = new UdpClient(port);
            _onBytesReceived = onBytesReceived;
        }

        public void StartReceiving()
        {
            if (IsListening)
                throw new UdpChannelException("Channel is already listening");

            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            IsListening = true;

            _listeningTask = Task.Run(async () =>
            {
                var any = new IPEndPoint(IPAddress.Any, 0);

                try
                {
                    while (!token.IsCancellationRequested)
                    {
                        UdpReceiveResult result;

                        var receiveTask = _udpClient.ReceiveAsync();
                        var completed = await Task.WhenAny(receiveTask, Task.Delay(Timeout.Infinite, token))
                                                 .ConfigureAwait(false);

                        if (completed != receiveTask)
                            break; // cancellato

                        result = receiveTask.Result;
                        _onBytesReceived?.Invoke(result.Buffer);
                    }
                }
                catch (ObjectDisposedException)
                {
                    // socket chiuso durante stop/close: ok
                }
                catch (SocketException)
                {
                    // tipico quando chiudi il socket: ok se stai stoppando
                    if (!token.IsCancellationRequested) throw;
                }
                finally
                {
                    IsListening = false;
                }
            }, CancellationToken.None);
        }

        public void StopReceiving()
        {
            if (!IsListening)
                throw new UdpChannelException("Channel is not listening");

            _cts?.Cancel();
        }

        public void CloseChannel()
        {
            StopIfNeeded();
            _udpClient.Close();
            _udpClient.Dispose();
        }

        private void StopIfNeeded()
        {
            if (IsListening)
            {
                _cts?.Cancel();
                // non blocco qui: se vuoi, puoi fare una versione async per await del task
            }
        }


        public void Dispose()
        {
            CloseChannel();
            _cts?.Dispose();
        }
    }
}