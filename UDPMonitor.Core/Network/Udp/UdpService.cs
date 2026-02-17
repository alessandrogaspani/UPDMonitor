using System;
using System.Collections.Generic;
using System.Text;

namespace UDPMonitor.Core.Network.Udp
{
    public static class UdpService
    {
        // NOTE: Dictionary<> non è thread-safe -> proteggiamo tutto con lock
        private static readonly object _gate = new object();

        private static readonly Dictionary<string, UdpInChannel> _inChannels =
            new Dictionary<string, UdpInChannel>(StringComparer.Ordinal);

        private static readonly Dictionary<string, UdpOutChannel> _outChannels =
            new Dictionary<string, UdpOutChannel>(StringComparer.Ordinal);

        #region IN_CHANNEL

        /// <summary>
        /// Crea e registra un canale IN. Se isListening=true parte subito.
        /// Se il tag esiste già, lancia eccezione (per evitare sovrascritture silenziose).
        /// </summary>
        public static void AddInChannel(string tag, int port, Action<byte[]> onDataReceivedCallback, bool isListening = false)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));
            if (onDataReceivedCallback == null) throw new ArgumentNullException(nameof(onDataReceivedCallback));

            var inChannel = new UdpInChannel(port, onDataReceivedCallback);

            lock (_gate)
            {
                if (_inChannels.ContainsKey(tag))
                    throw new ArgumentException($"InChannel '{tag}' già esiste.", nameof(tag));

                _inChannels.Add(tag, inChannel);
            }

            // fuori dal lock: evita di bloccare altre operazioni mentre parte la ricezione
            if (isListening)
                inChannel.StartReceiving();
        }

        public static void StartReceiving(string tag)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));

            UdpInChannel inChannel;
            lock (_gate)
            {
                _inChannels.TryGetValue(tag, out inChannel);
            }

            inChannel?.StartReceiving();
        }

        public static void StopReceiving(string tag)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));

            UdpInChannel inChannel;
            lock (_gate)
            {
                _inChannels.TryGetValue(tag, out inChannel);
            }

            inChannel?.StopReceiving();
        }

        public static void CloseInChannel(string tag)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));

            UdpInChannel inChannel = null;

            lock (_gate)
            {
                if (_inChannels.TryGetValue(tag, out inChannel))
                {
                    // rimuovo PRIMA dal dizionario: così nessun altro thread lo trova mentre lo sto chiudendo
                    _inChannels.Remove(tag);
                }
            }

            if (inChannel == null)
                return;

            // fuori dal lock: Stop/Close possono bloccare o generare eccezioni
            if (inChannel.IsListening)
                inChannel.StopReceiving();

            inChannel.CloseChannel();
        }

        #endregion IN_CHANNEL

        #region OUT_CHANNEL

        /// <summary>
        /// Crea e registra un canale OUT.
        /// Se il tag esiste già, lancia eccezione.
        /// </summary>
        public static void AddOutChannel(string tag, int port, string ipAddress, Action<byte[]> onDataTransmittedCallback = null)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));
            if (ipAddress == null) throw new ArgumentNullException(nameof(ipAddress));

            var outChannel = new UdpOutChannel(port, ipAddress, onDataTransmittedCallback);

            lock (_gate)
            {
                if (_outChannels.ContainsKey(tag))
                    throw new ArgumentException($"OutChannel '{tag}' già esiste.", nameof(tag));

                _outChannels.Add(tag, outChannel);
            }
        }

        public static void Send(string tag, string message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            SendBytes(tag, Encoding.UTF8.GetBytes(message));
        }

        public static void SendBytes(string tag, byte[] dataToSend)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));
            if (dataToSend == null) throw new ArgumentNullException(nameof(dataToSend));

            UdpOutChannel outChannel;
            lock (_gate)
            {
                _outChannels.TryGetValue(tag, out outChannel);
            }

            outChannel?.Send(dataToSend);
        }

        public static void CloseOutChannel(string tag)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tag));

            UdpOutChannel outChannel = null;

            lock (_gate)
            {
                if (_outChannels.TryGetValue(tag, out outChannel))
                {
                    // rimuovo PRIMA dal dizionario
                    _outChannels.Remove(tag);
                }
            }

            if (outChannel == null)
                return;

            outChannel.CloseChannel();
        }

        #endregion OUT_CHANNEL

        #region UTILITIES

        /// <summary>
        /// Chiude tutti i canali (utile in shutdown).
        /// </summary>
        public static void CloseAll()
        {
            List<UdpInChannel> inToClose;
            List<UdpOutChannel> outToClose;

            lock (_gate)
            {
                inToClose = new List<UdpInChannel>(_inChannels.Values);
                outToClose = new List<UdpOutChannel>(_outChannels.Values);
                _inChannels.Clear();
                _outChannels.Clear();
            }

            foreach (var ch in inToClose)
            {
                try
                {
                    if (ch.IsListening) ch.StopReceiving();
                }
                catch { /* opzionale: log */ }

                try { ch.CloseChannel(); }
                catch { /* opzionale: log */ }
            }

            foreach (var ch in outToClose)
            {
                try { ch.CloseChannel(); }
                catch { /* opzionale: log */ }
            }
        }

        #endregion UTILITIES
    }
}
