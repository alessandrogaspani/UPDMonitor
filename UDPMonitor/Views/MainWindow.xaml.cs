using System;
using System.Windows;
using System.Windows.Input;

namespace UDPMonitor.Views
{
    public partial class MainWindow_View : Window
    {
        private Point _dragStartPoint;
        private bool _maybeDrag;
        private IInputElement _dragSource;

        public MainWindow_View()
        {
            InitializeComponent();
        }

        private void CanExecute_Always(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
            e.Handled = true;
        }

        private void Executed_Minimize(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
            e.Handled = true;
        }

        private void Executed_ToggleMaximize(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = (WindowState == WindowState.Maximized)
                ? WindowState.Normal
                : WindowState.Maximized;

            e.Handled = true;
        }

        private void TitleBar_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;

            // Doppio click => toggle maximize/restore
            if (e.ClickCount == 2)
            {
                WindowState = (WindowState == WindowState.Maximized)
                    ? WindowState.Normal
                    : WindowState.Maximized;

                e.Handled = true;
                _maybeDrag = false;
                Mouse.Capture(null);
                return;
            }

            // prepara il drag (ma non fare ancora nulla)
            _dragStartPoint = e.GetPosition(this);
            _maybeDrag = true;
            _dragSource = sender as IInputElement;

            // cattura il mouse per ricevere Move/Up anche se esci dal bordo
            if (_dragSource != null)
                Mouse.Capture(_dragSource);

            e.Handled = true;
        }

        private void TitleBar_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_maybeDrag || e.LeftButton != MouseButtonState.Pressed)
                return;

            var current = e.GetPosition(this);

            // soglia per distinguere click da drag
            const double threshold = 6;
            if (Math.Abs(current.X - _dragStartPoint.X) < threshold &&
                Math.Abs(current.Y - _dragStartPoint.Y) < threshold)
                return;

            // ora è un drag vero
            _maybeDrag = false;
            Mouse.Capture(null);

            // Se massimizzata: restore e riposiziona in modo "VS-like"
            if (WindowState == WindowState.Maximized)
            {
                // percentuale orizzontale del punto di click
                var percentX = _dragStartPoint.X / Math.Max(1.0, ActualWidth);

                WindowState = WindowState.Normal;
                UpdateLayout();

                // mantieni il mouse "sullo stesso punto" della finestra
                Left = SystemParameters.WorkArea.Left
                       + (SystemParameters.WorkArea.Width * percentX)
                       - (ActualWidth * percentX);

                // stile VS: appoggia in alto (se vuoi seguire Y, dimmelo)
                Top = SystemParameters.WorkArea.Top;
            }

            try
            {
                DragMove();
            }
            catch
            {
                // DragMove può fallire in casi rari, ignoriamo
            }

            e.Handled = true;
        }

        private void TitleBar_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _maybeDrag = false;
            Mouse.Capture(null);
            e.Handled = true;
        }
    }
}
