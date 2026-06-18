using Moq;
using Prism.Services.Dialogs;
using System.Threading;
using System.Windows;
using UDPMonitor.Business.Interfaces;
using UDPMonitor.Business.Models;
using UDPMonitor.ViewModels;

namespace UDPMonitor.Tests.ViewModels;

public sealed class Inbound_ViewModelTests
{
    private readonly Mock<IInboundService> _mockInboundService;
    private readonly Mock<IDialogService> _mockDialogService;

    private const int DefaultPort = 9000;

    public Inbound_ViewModelTests()
    {
        _mockInboundService = new Mock<IInboundService>();
        _mockDialogService = new Mock<IDialogService>();
    }

    private Inbound_ViewModel CreateViewModel()
    {
        _mockInboundService.Setup(s => s.Port).Returns(DefaultPort);
        return new(_mockInboundService.Object, _mockDialogService.Object);
    }

    private void AllowPortAssignment()
        => _mockInboundService.SetupSet(s => s.Port = It.IsAny<int>());

    [Trait("Category", "Unit")]
    [Fact]
    public void Constructor_SetsPortFromService()
    {
        var vm = CreateViewModel();

        Assert.Equal("9000", vm.Port);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public void Constructor_IsConnected_IsFalse()
    {
        var vm = CreateViewModel();

        Assert.False(vm.IsConnected);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public void ConnectCommand_WhenDisconnected_CallsStartListen()
    {
        // Setup: il mock deve accettare l'assegnazione della proprietà Port
        _mockInboundService.SetupSet(s => s.Port = It.IsAny<int>());

        var vm = CreateViewModel();

        vm.ConnectCommand.Execute();

        // Verify: StartListen deve essere stato chiamato una volta
        _mockInboundService.Verify(s => s.StartListen(), Times.Once);
        Assert.True(vm.IsConnected);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public void ConnectCommand_WhenConnected_CallsStopListen()
    {
        AllowPortAssignment();

        var vm = CreateViewModel();

        vm.ConnectCommand.Execute();
        vm.ConnectCommand.Execute();

        _mockInboundService.Verify(s => s.StopListen(), Times.Once);
        Assert.False(vm.IsConnected);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public void ClearCommand_ResetsCounterAndMessages()
    {
        var vm = CreateViewModel();

        vm.InMessages.Add(new ODTMessage { Text = "test" });
        vm.MessageCounter = 5;

        vm.ClearCommand.Execute();

        Assert.Empty(vm.InMessages);
        Assert.Equal(0, vm.MessageCounter);
    }

    [Trait("Category", "Integration")]
    [Fact]
    public void OnInboundMessageReceived_AddsMessageToInMessages()
    {
        RunOnStaThread(() =>
        {
            if (Application.Current == null)
                _ = new Application();

            var vm = CreateViewModel();

            vm.OnNavigatedTo(null!);

            var message = new InMessage
            {
                TimeStamp = DateTime.Now,
                Text = "hello UDP"
            };

            _mockInboundService.Raise(
                s => s.OnInboundMessageReceived += null,
                message);

            Assert.Single(vm.InMessages);
            Assert.Equal("hello UDP", vm.InMessages[0].Text);
        });
    }

    private static void RunOnStaThread(Action action)
    {
        Exception? failure = null;

        var thread = new Thread(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                failure = ex;
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (failure != null)
            throw new AggregateException(failure);
    }
}