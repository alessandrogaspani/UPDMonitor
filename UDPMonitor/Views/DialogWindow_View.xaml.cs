using Prism.Services.Dialogs;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;

namespace UDPMonitor.Views
{
    public partial class DialogWindow_View : Window, IDialogWindow
    {
        public DialogWindow_View()
        {
            InitializeComponent();
        }

        public IDialogResult Result { get; set; }

        private const int WM_MOUSEACTIVATE = 0x0021;
        private const int MA_NOACTIVATE = 0x0003;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_MOUSEACTIVATE)
            {
                handled = true;
                return (IntPtr)MA_NOACTIVATE;
            }
            else
            {
                return IntPtr.Zero;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var a = Application.Current.MainWindow;
            a.Activate();
        }
    }
}