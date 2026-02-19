using System;
using System.Globalization;
using System.Windows.Data;

namespace UDPMonitor.Converters
{
    public sealed class RelativeTimeHumanConverter : IValueConverter
    {
        private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;

            // Supporta sia stringa ("hh:mm:ss.fff") sia TimeSpan (se un domani cambi tipo)
            if (value is TimeSpan ts)
                return Format(ts);

            var s = value as string;
            if (string.IsNullOrWhiteSpace(s))
                return string.Empty;

            // Permettiamo un eventuale prefisso +/-
            int sign = 1;
            s = s.Trim();
            if (s.StartsWith("-", StringComparison.Ordinal))
            {
                sign = -1;
                s = s.Substring(1);
            }
            else if (s.StartsWith("+", StringComparison.Ordinal))
            {
                s = s.Substring(1);
            }

            // Prova a parsare come TimeSpan (accetta anche "hh:mm:ss.fff")
            if (TimeSpan.TryParse(s, Invariant, out var parsed))
                return Format(TimeSpan.FromTicks(parsed.Ticks * sign));

            // fallback: lascia com'è
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;

        private static string Format(TimeSpan ts)
        {
            var negative = ts.Ticks < 0;
            ts = ts.Duration();

            // TotalMilliseconds
            var totalMs = ts.TotalMilliseconds;

            string core;

            if (totalMs < 1000)
            {
                // 0..999 ms
                core = $"{Math.Round(totalMs):0} ms";
            }
            else if (ts.TotalSeconds < 60)
            {
                // 1.00 .. 59.99 s
                core = $"{ts.TotalSeconds:0.##} s";
            }
            else if (ts.TotalMinutes < 60)
            {
                // 1m 05s
                core = $"{(int)ts.TotalMinutes}m {ts.Seconds:00}s";
            }
            else if (ts.TotalHours < 24)
            {
                // 1h 02m 03s
                core = $"{(int)ts.TotalHours}h {ts.Minutes:00}m {ts.Seconds:00}s";
            }
            else
            {
                // 2d 03h 04m
                core = $"{ts.Days}d {ts.Hours:00}h {ts.Minutes:00}m";
            }

            return negative ? "-" + core : core;
        }
    }

}
