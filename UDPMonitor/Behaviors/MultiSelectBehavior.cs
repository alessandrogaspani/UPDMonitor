using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace UDPMonitor.Behaviors
{
    public static class MultiSelectBehavior
    {
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.RegisterAttached(
                "SelectedItems",
                typeof(IList),
                typeof(MultiSelectBehavior),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemsChanged));

        public static void SetSelectedItems(DependencyObject element, IList value)
            => element.SetValue(SelectedItemsProperty, value);

        public static IList GetSelectedItems(DependencyObject element)
            => (IList)element.GetValue(SelectedItemsProperty);

        private static readonly DependencyProperty IsHookedProperty =
            DependencyProperty.RegisterAttached("IsHooked", typeof(bool), typeof(MultiSelectBehavior), new PropertyMetadata(false));

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as ListView;
            if (listView == null) return;

            Hook(listView);

            // se la collection nel VM notifica, posso riflettere anche VM -> UI
            var oldObs = e.OldValue as INotifyCollectionChanged;
            if (oldObs != null) oldObs.CollectionChanged -= (s, args) => SyncFromVmToUi(listView);

            var newObs = e.NewValue as INotifyCollectionChanged;
            if (newObs != null) newObs.CollectionChanged += (s, args) => SyncFromVmToUi(listView);

            // prima sync
            SyncFromVmToUi(listView);
        }

        private static void Hook(ListView listView)
        {
            if ((bool)listView.GetValue(IsHookedProperty)) return;

            listView.SelectionChanged += (s, e) =>
            {
                var bound = GetSelectedItems(listView);
                if (bound == null) return;

                // UI -> VM
                foreach (var item in e.RemovedItems)
                    if (bound.Contains(item)) bound.Remove(item);

                foreach (var item in e.AddedItems)
                    if (!bound.Contains(item)) bound.Add(item);
            };

            listView.SetValue(IsHookedProperty, true);
        }

        private static bool _syncing;

        private static void SyncFromVmToUi(ListView listView)
        {
            if (_syncing) return;
            var bound = GetSelectedItems(listView);
            if (bound == null) return;

            try
            {
                _syncing = true;

                listView.SelectedItems.Clear();
                foreach (var item in bound)
                    listView.SelectedItems.Add(item);
            }
            finally
            {
                _syncing = false;
            }
        }
    }
}
