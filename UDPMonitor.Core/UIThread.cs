using System.Windows;
using System.Windows.Threading;

namespace TBMX.Core.Services
{
    public static class UIThread
    {
        private static readonly int _uiThreadId;

        static UIThread()
        {
            _uiThreadId = Application.Current.Dispatcher.Thread.ManagedThreadId;
        }

        public static void Invoke(Action method)
        {
            if (Thread.CurrentThread.ManagedThreadId != _uiThreadId)
            {
                Application.Current?.Dispatcher.Invoke(DispatcherPriority.Normal, method);
                return;
            }

            method();
        }

        public static void BeginInvoke(Action method)
        {
            if (Thread.CurrentThread.ManagedThreadId != _uiThreadId)
            {
                Application.Current?.Dispatcher.BeginInvoke(DispatcherPriority.Normal, method);
                return;
            }

            method();
        }
    }
}