namespace UDPMonitor.Core.Extensions
{
    public static class IList_Extensions
    {
        public static bool IsEmpty<T>(this IList<T> list)
        {
            return list.Count == 0;
        }

        public static bool HasElements<T>(this IList<T> list, int minNumber = 0)
        {
            if (list == null || list.Count == 0)
                return false;

            return list.Count >= minNumber;
        }

        public static T BinarySearch<T, TKey>(this IList<T> list,
            TKey key,
            Func<T, TKey> keySelector,
            Func<T, T, T> closestSelector = null) where TKey : IComparable<TKey>
        {
            if (list == null || list.Count == 0)
                return default;

            int min = 0;
            int max = list.Count - 1;

            while (min <= max)
            {
                int mid = min + ((max - min) / 2);
                T midItem = list[mid];
                TKey midKey = keySelector(midItem);
                int comp = midKey.CompareTo(key);

                if (comp < 0)
                {
                    min = mid + 1;
                }
                else if (comp > 0)
                {
                    max = mid - 1;
                }
                else
                {
                    return midItem;
                }
            }

            // Se l'elemento non è stato trovato, gestiamo il valore più vicino
            if (closestSelector != null)
            {
                if (min >= list.Count) return list[max]; // Il più vicino è max
                if (max < 0) return list[min]; // Il più vicino è min

                return closestSelector.Invoke(list[max], list[min]);
            }

            return default;
        }

        public static T NextOf<T>(this IList<T> list, T item, bool cyclical = false)
        {
            var indexOf = list.IndexOf(item);

            if (cyclical)
            {
                return list[indexOf == list.Count - 1 ? 0 : indexOf + 1];
            }
            else
            {
                return list[indexOf == list.Count - 1 ? indexOf : indexOf + 1];
            }
        }

        public static T NextOf<T>(this IList<T> list, T item, int step, bool cyclical = false)
        {
            var indexOf = list.IndexOf(item);

            if (cyclical)
            {
                return list[indexOf + step < list.Count - 1 ? 0 : indexOf + step];
            }
            else
            {
                return list[indexOf + step < list.Count - 1 ? indexOf : indexOf + step];
            }
        }

        public static T PreviousOf<T>(this IList<T> list, T item, bool cyclical = false)
        {
            var indexOf = list.IndexOf(item);

            if (cyclical)
            {
                return list[indexOf > 0 ? indexOf - 1 : list.Count - 1];
            }
            else
            {
                return list[indexOf > 0 ? indexOf - 1 : 0];
            }
        }

        public static T PreviousOf<T>(this IList<T> list, T item, int step, bool cyclical = false)
        {
            var indexOf = list.IndexOf(item);

            if (cyclical)
            {
                return list[indexOf > 0 ? indexOf - 1 : (list.Count - 1) - step];
            }
            else
            {
                return list[indexOf - step > 0 ? indexOf - step : 0];
            }
        }

        public static T GetElementAtOrDefault<T>(this IList<T> list, int index, T valueOnMissing = default(T))
        {
            if (list.Count > index)
            {
                return list[index];
            }
            else
            {
                return valueOnMissing;
            }
        }

        public static void InsertFirst<T>(this IList<T> list, T value)
        {
            if (list != null)
                list.Insert(0, value);
        }

        public static void InsertLast<T>(this IList<T> list, T value)
        {
            if (list != null)
                list.Add(value);
        }

        public static void GetElementsFrom<T>(this IList<T> list, IList<T> source)
        {
            if (list != null)
            {
                list.Clear();

                for (int iElement = 0; iElement < source.Count; iElement++)
                    list.Add(source[iElement]);
            }
        }

        public static void RemoveBefore<T>(this IList<T> list, T element)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            int index = list.IndexOf(element);
            if (index > 0)
            {
                for (int i = index - 1; i >= 0; i--)
                {
                    list.RemoveAt(i);
                }
            }
        }
    }
}