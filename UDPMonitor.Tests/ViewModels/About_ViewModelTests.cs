using Prism.Services.Dialogs;
using UDPMonitor.Core;
using UDPMonitor.ViewModels;

namespace UDPMonitor.Tests.ViewModels;

public sealed class About_ViewModelTests
{
    [Trait("Category", "Unit")]
    [Fact]
    public void OnDialogOpened_SetsExpectedFields()
    {
        var vm = new About_ViewModel();
        var parameters = new DialogParameters
        {
            { "ApplicationName", "UDPMonitor" },
            { "Version", "1.2.3" },
            { "Creator", "Alessandro Gaspani" }
        };

        vm.OnDialogOpened(parameters);

        Assert.Equal("About UDP Monitor", vm.Title);
        Assert.Equal("UDPMonitor", vm.SoftwareName);
        Assert.Equal("1.2.3", vm.Version);
        Assert.Equal("Alessandro Gaspani", vm.Creator);
        Assert.False(string.IsNullOrWhiteSpace(vm.OS));
        Assert.False(string.IsNullOrWhiteSpace(vm.Runtime));
        Assert.False(string.IsNullOrWhiteSpace(vm.Architecture));
        Assert.Contains(vm.Creator, vm.Copyright);
    }

    [Trait("Category", "Unit")]
    [Fact]
    public void CloseCommand_RaisesRequestClose()
    {
        var vm = new About_ViewModel();
        IDialogResult? result = null;
        vm.RequestClose += r => result = r;

        vm.CloseCommand.Execute();

        Assert.NotNull(result);
        Assert.Equal(ButtonResult.OK, result!.Result);
    }
}
