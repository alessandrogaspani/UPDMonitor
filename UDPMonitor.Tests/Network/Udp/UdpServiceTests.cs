using UDPMonitor.Core.Network.Udp;

namespace UDPMonitor.Tests.Network.Udp;

public sealed class UdpServiceTests : IDisposable
{
    public UdpServiceTests()
    {
        UdpService.CloseAll();
    }

    public void Dispose()
    {
        UdpService.CloseAll();
    }

    [Trait("Category", "Unit")]
    [Fact]
    public void AddInChannel_WithNullTag_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => UdpService.AddInChannel(null!, 0, _ => { }));
    }

    [Trait("Category", "Unit")]
    [Fact]
    public void AddInChannel_WithDuplicateTag_ThrowsArgumentException()
    {
        const string tag = "in-duplicate";

        UdpService.AddInChannel(tag, 0, _ => { });

        var exception = Assert.Throws<ArgumentException>(() => UdpService.AddInChannel(tag, 0, _ => { }));
        Assert.Equal("tag", exception.ParamName);
    }

    [Trait("Category", "Unit")]
    [Theory]
    [InlineData("")]
    [InlineData("not-an-ip")]
    [InlineData("999.999.999.999")]
    public void AddOutChannel_WithInvalidIpAddress_ThrowsArgumentException(string ipAddress)
    {
        var exception = Assert.Throws<ArgumentException>(() => UdpService.AddOutChannel(Guid.NewGuid().ToString("N"), 5000, ipAddress));
        Assert.Equal("ipAddress", exception.ParamName);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public void AddOutChannel_WithDuplicateTag_ThrowsArgumentException()
    {
        const string tag = "out-duplicate";

        UdpService.AddOutChannel(tag, 5000, "127.0.0.1");

        var exception = Assert.Throws<ArgumentException>(() => UdpService.AddOutChannel(tag, 5001, "127.0.0.1"));
        Assert.Equal("tag", exception.ParamName);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public void CloseAll_AfterAddingChannels_DoesNotThrow()
    {
        UdpService.AddInChannel("input", 0, _ => { });
        UdpService.AddOutChannel("output", 5000, "127.0.0.1");

        var exception = Record.Exception(UdpService.CloseAll);

        Assert.Null(exception);
    }
}
