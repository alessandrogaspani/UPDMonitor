using System.Net;
using System.Net.Sockets;
using UDPMonitor.Core.Network.Udp;

namespace UDPMonitor.Tests.Network.Udp;

public sealed class UdpInChannelTests
{
    private static readonly TimeSpan ReceiveTimeout = TimeSpan.FromSeconds(5);

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

        Assert.Equal(payload, await WaitForTaskAsync(received.Task));
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

    private static async Task<T> WaitForTaskAsync<T>(Task<T> task)
    {
        var completed = await Task.WhenAny(task, Task.Delay(ReceiveTimeout));

        Assert.Same(task, completed);
        return await task;
    }

    private static int GetFreeUdpPort()
    {
        using var udpClient = new UdpClient(0);
        return ((IPEndPoint)udpClient.Client.LocalEndPoint!).Port;
    }
}
