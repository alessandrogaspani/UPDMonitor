using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Input;

namespace UDPMonitor.Behaviors
{
    public class DoubleClickInvokeCommandAction : TriggerAction<UIElement>
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(DoubleClickInvokeCommandAction));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        protected override void Invoke(object parameter)
        {
            if (parameter is MouseButtonEventArgs e &&
                e.ChangedButton == MouseButton.Left &&
                e.ClickCount == 2)
            {
                if (Command?.CanExecute(null) == true)
                    Command.Execute(null);

                e.Handled = true;
            }
        }
    }
}
