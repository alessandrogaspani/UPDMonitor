using System.Windows.Input;

namespace UDPMonitor
{
    public static class WindowCommands
    {
        public static readonly RoutedUICommand Minimize =
            new RoutedUICommand("Minimize", "Minimize", typeof(WindowCommands));

        public static readonly RoutedUICommand ToggleMaximize =
            new RoutedUICommand("ToggleMaximize", "ToggleMaximize", typeof(WindowCommands));
    }
}