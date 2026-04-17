using Moq;
using Prism.Services.Dialogs;
using UDPMonitor.Core;
using UDPMonitor.ViewModels;

namespace UDPMonitor.Tests.ViewModels;

public sealed class MainWindow_ViewModelTests
{
    private readonly Mock<IDialogService> _mockDialogService;

    public MainWindow_ViewModelTests()
    {
        _mockDialogService = new Mock<IDialogService>();
    }

    private MainWindow_ViewModel CreateViewModel()
        => new(_mockDialogService.Object);

    [Trait("Category", "Unit")]
    [Fact]
    public void Constructor_SetsTitleFromApplicationService()
    {
        var vm = CreateViewModel();

        Assert.Equal(ApplicationService.ApplicationName, vm.Title);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public void OpenAboutDialogCommand_ShowsAboutDialogWithExpectedParameters()
    {
        var vm = CreateViewModel();
        IDialogParameters? capturedParameters = null;
        string? capturedViewName = null;

        _mockDialogService
            .Setup(s => s.ShowDialog(
                It.IsAny<string>(),
                It.IsAny<IDialogParameters>(),
                It.IsAny<Action<IDialogResult>>()))
            .Callback<string, IDialogParameters, Action<IDialogResult>>((viewName, parameters, _) =>
            {
                capturedViewName = viewName;
                capturedParameters = parameters;
            });

        vm.OpenAboutDialogCommand.Execute();

        Assert.Equal(App.RegisteredViews.DialogAbout_View, capturedViewName);
        Assert.NotNull(capturedParameters);
        Assert.Equal(ApplicationService.ApplicationName, capturedParameters!.GetValue<string>("ApplicationName"));
        Assert.Equal(ApplicationService.Creator, capturedParameters.GetValue<string>("Creator"));
        Assert.Equal(ApplicationService.Version.ToString(), capturedParameters.GetValue<string>("Version"));

        _mockDialogService.Verify(
            s => s.ShowDialog(It.IsAny<string>(), It.IsAny<IDialogParameters>(), It.IsAny<Action<IDialogResult>>()),
            Times.Once);
    }
}
