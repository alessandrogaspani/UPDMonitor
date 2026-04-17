using System.Net;
using System.Net.Sockets;
using UDPMonitor.Core.Network.Udp;

namespace UDPMonitor.Tests.Network.Udp;

public sealed class UdpOutChannelTests
{
    [Trait("Category", "Integration")]
    [Fact]
    public void Constructor_WithInvalidIpAddress_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => new UdpOutChannel(5000, "invalid-ip"));
        Assert.Equal("ipAddress", exception.ParamName);
    }

    [Trait("Category", "Integration")]
    [Fact]
    public void Send_WithNullPayload_ThrowsArgumentNullException()
    {
        using var channel = new UdpOutChannel(5000, "127.0.0.1");

        var exception = Assert.Throws<ArgumentNullException>(() => channel.Send(null!));
        Assert.Equal("data", exception.ParamName);
    }

    [Trait("Category", "Integration")]
    [Fact]
    public async Task Send_WithValidPayload_TransmitsBytesAndInvokesCallback()
    {
        using var receiver = new UdpClient(0);
        var port = ((IPEndPoint)receiver.Client.LocalEndPoint!).Port;
        var payload = "hello"u8.ToArray();
        var transmitted = new TaskCompletionSource<byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var channel = new UdpOutChannel(port, "127.0.0.1", bytes => transmitted.TrySetResult(bytes));

        var receiveTask = receiver.ReceiveAsync();

        channel.Send(payload);

        var completedReceive = await Task.WhenAny(receiveTask, Task.Delay(TimeSpan.FromSeconds(2)));
        var completedCallback = await Task.WhenAny(transmitted.Task, Task.Delay(TimeSpan.FromSeconds(2)));

        Assert.Same(receiveTask, completedReceive);
        Assert.Same(transmitted.Task, completedCallback);
        Assert.Equal(payload, receiveTask.Result.Buffer);
        Assert.Equal(payload, await transmitted.Task);
    }
}
