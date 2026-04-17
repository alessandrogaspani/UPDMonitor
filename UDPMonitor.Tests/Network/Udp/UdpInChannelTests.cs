using System.Net;
using System.Net.Sockets;
using UDPMonitor.Core.Network.Udp;

namespace UDPMonitor.Tests.Network.Udp;

public sealed class UdpInChannelTests
{
    [Trait("Category", "Integration")]
    [Fact]
    public void StartReceiving_WhenAlreadyListening_ThrowsUdpChannelException()
    {
        var port = GetFreeUdpPort();
        using var channel = new UdpInChannel(port, _ => { });

        channel.StartReceiving();

        var exception = Assert.Throws<UdpChannelException>(channel.StartReceiving);
        Assert.Equal("Channel is already listening", exception.Message);
    }

    [Trait("Category", "Integration")]
    [Fact]
    public void StopReceiving_WhenNotListening_ThrowsUdpChannelException()
    {
        var port = GetFreeUdpPort();
        using var channel = new UdpInChannel(port, _ => { });

        var exception = Assert.Throws<UdpChannelException>(channel.StopReceiving);
        Assert.Equal("Channel is not listening", exception.Message);
    }

    [Trait("Category", "Integration")]
    [Fact]
    public async Task StartReceiving_WhenDataArrives_InvokesCallback()
    {
        var port = GetFreeUdpPort();
        var payload = "ping"u8.ToArray();
        var received = new TaskCompletionSource<byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var channel = new UdpInChannel(port, bytes => received.TrySetResult(bytes));
        using var sender = new UdpClient();

        channel.StartReceiving();
        await sender.SendAsync(payload, payload.Length, new IPEndPoint(IPAddress.Loopback, port));

        var completed = await Task.WhenAny(received.Task, Task.Delay(TimeSpan.FromSeconds(2)));

        Assert.Same(received.Task, completed);
        Assert.Equal(payload, await received.Task);
    }

    [Trait("Category", "Integration")]
    [Fact]
    public void CloseChannel_WhenListening_DoesNotThrow()
    {
        var port = GetFreeUdpPort();
        using var channel = new UdpInChannel(port, _ => { });

        channel.StartReceiving();

        var exception = Record.Exception(channel.CloseChannel);

        Assert.Null(exception);
    }

    private static int GetFreeUdpPort()
    {
        using var udpClient = new UdpClient(0);
        return ((IPEndPoint)udpClient.Client.LocalEndPoint!).Port;
    }
}
