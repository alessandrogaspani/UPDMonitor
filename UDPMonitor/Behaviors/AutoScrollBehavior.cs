using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace UDPMonitor.Behaviors
{
    public static class AutoScrollBehavior
    {
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(AutoScrollBehavior),
                new PropertyMetadata(false, OnIsEnabledChanged));

        public static bool GetIsEnabled(DependencyObject obj) => (bool)obj.GetValue(IsEnabledProperty);
        public static void SetIsEnabled(DependencyObject obj, bool value) => obj.SetValue(IsEnabledProperty, value);

        private static readonly DependencyProperty SubscriptionProperty =
            DependencyProperty.RegisterAttached(
                "Subscription",
                typeof(Subscription),
                typeof(AutoScrollBehavior),
                new PropertyMetadata(null));

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as ListView;
            if (listView == null) return;

            (listView.GetValue(SubscriptionProperty) as Subscription)?.Dispose();
            listView.ClearValue(SubscriptionProperty);

            if (!(e.NewValue is bool) || !(bool)e.NewValue) return;

            var sub = new Subscription(listView);
            listView.SetValue(SubscriptionProperty, sub);
            sub.Attach();
        }

        private sealed class Subscription : IDisposable
        {
            private readonly ListView _listView;
            private INotifyCollectionChanged _incc;
            private ScrollViewer _scrollViewer;

            // newest on top => pinned quando sei a TOP
            private bool _pinnedToTop = true;

            public Subscription(ListView listView) => _listView = listView;

            public void Attach()
            {
                _listView.Loaded += OnLoaded;
                _listView.Unloaded += OnUnloaded;

                HookCollection();
                HookScrollViewer();
            }

            private void OnLoaded(object sender, RoutedEventArgs e)
            {
                HookCollection();
                HookScrollViewer();
                _pinnedToTop = IsAtTop();
            }

            private void OnUnloaded(object sender, RoutedEventArgs e) => Dispose();

            private void HookCollection()
            {
                var newIncc = _listView.ItemsSource as INotifyCollectionChanged;

                if (_incc != null)
                    _incc.CollectionChanged -= OnCollectionChanged;

                _incc = newIncc;

                if (_incc != null)
                    _incc.CollectionChanged += OnCollectionChanged;
            }

            private void HookScrollViewer()
            {
                if (_scrollViewer != null)
                    _scrollViewer.ScrollChanged -= OnScrollChanged;

                _scrollViewer = FindScrollViewer(_listView);
                if (_scrollViewer != null)
                    _scrollViewer.ScrollChanged += OnScrollChanged;
            }

            private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
            {
                // aggiornare pinned solo quando è scroll "utente" (Extent invariato)
                if (Math.Abs(e.ExtentHeightChange) < 0.0001 && Math.Abs(e.ViewportHeightChange) < 0.0001)
                {
                    _pinnedToTop = IsAtTop();
                }
            }

            private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action != NotifyCollectionChangedAction.Add &&
                    e.Action != NotifyCollectionChangedAction.Reset)
                    return;

                if (!_pinnedToTop)
                    return;

                // newest on top => scrolla al PRIMO elemento
                _listView.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (_listView.Items.Count > 0)
                        _listView.ScrollIntoView(_listView.Items[0]);

                    _pinnedToTop = true;
                }), DispatcherPriority.Background);
            }

            private bool IsAtTop()
            {
                if (_scrollViewer == null) return true;

                const double tolerance = 1.0;
                return _scrollViewer.VerticalOffset <= tolerance;
            }

            private static ScrollViewer FindScrollViewer(DependencyObject root)
            {
                if (root == null) return null;
                if (root is ScrollViewer sv) return sv;

                int count = VisualTreeHelper.GetChildrenCount(root);
                for (int i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(root, i);
                    var found = FindScrollViewer(child);
                    if (found != null) return found;
                }
                return null;
            }

            public void Dispose()
            {
                _listView.Loaded -= OnLoaded;
                _listView.Unloaded -= OnUnloaded;

                if (_incc != null)
                    _incc.CollectionChanged -= OnCollectionChanged;

                if (_scrollViewer != null)
                    _scrollViewer.ScrollChanged -= OnScrollChanged;

                _incc = null;
                _scrollViewer = null;
            }
        }
    }
}