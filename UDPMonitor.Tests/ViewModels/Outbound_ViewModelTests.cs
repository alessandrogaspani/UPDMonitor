using Moq;
using Prism.Services.Dialogs;
using UDPMonitor.Business.Interfaces;
using UDPMonitor.ViewModels;

namespace UDPMonitor.Tests.ViewModels;

public sealed class Outbound_ViewModelTests
{
    private readonly Mock<IOutboundService> _mockOutboundService;
    private readonly Mock<IDialogService> _mockDialogService;

    private const int DefaultPort = 5000;
    private const string DefaultIpAddress = "127.0.0.1";

    public Outbound_ViewModelTests()
    {
        _mockOutboundService = new Mock<IOutboundService>();
        _mockDialogService = new Mock<IDialogService>();
    }

    private Outbound_ViewModel CreateViewModel()
    {
        _mockOutboundService.Setup(s => s.Port).Returns(DefaultPort);
        _mockOutboundService.Setup(s => s.IPAddress).Returns(DefaultIpAddress);
        return new(_mockOutboundService.Object, _mockDialogService.Object);
    }

    private void AllowConnectionConfiguration()
    {
        _mockOutboundService.SetupSet(s => s.Port = It.IsAny<int>());
        _mockOutboundService.SetupSet(s => s.IPAddress = It.IsAny<string>());
    }

    private Outbound_ViewModel CreateConnectedViewModel()
    {
        AllowConnectionConfiguration();

        var vm = CreateViewModel();
        vm.ToggleConnectionCommand.Execute();
        return vm;
    }

    [Trait("Category", "Unit")]
    [Fact]
    public void Constructor_SetsInitialValuesFromService()
    {
        var vm = CreateViewModel();

        Assert.Equal("5000", vm.Port);
        Assert.Equal("127.0.0.1", vm.IpAddress);
        Assert.Equal("30", vm.AutoSendHz);
        Assert.False(vm.IsConnected);
        Assert.False(vm.IsAutoSending);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public void ConnectCommand_WhenDisconnected_CallsConnectAndSetsConnected()
    {
        AllowConnectionConfiguration();

        var vm = CreateViewModel();

        vm.ToggleConnectionCommand.Execute();

        _mockOutboundService.VerifySet(s => s.Port = 5000, Times.Once);
        _mockOutboundService.VerifySet(s => s.IPAddress = "127.0.0.1", Times.Once);
        _mockOutboundService.Verify(s => s.Connect(), Times.Once);
        Assert.True(vm.IsConnected);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public void ToggleConnectionCommand_WhenConnected_CallsDisconnectAndSetsDisconnected()
    {
        var vm = CreateConnectedViewModel();

        vm.ToggleConnectionCommand.Execute();

        _mockOutboundService.Verify(s => s.Disconnect(), Times.Once);
        Assert.False(vm.IsConnected);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public void SendCommand_WhenConnectedAndHasText_CallsSendMessage()
    {
        var vm = CreateConnectedViewModel();

        vm.TextMessage = "hello";
        vm.SendCommand.Execute();

        _mockOutboundService.Verify(s => s.SendMessage("hello"), Times.Once);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public void SendCommand_WhenNotConnected_DoesNotCallSendMessage()
    {
        var vm = CreateViewModel();

        vm.TextMessage = "hello";
        vm.SendCommand.Execute();

        _mockOutboundService.Verify(s => s.SendMessage(It.IsAny<string>()), Times.Never);
    }
}
