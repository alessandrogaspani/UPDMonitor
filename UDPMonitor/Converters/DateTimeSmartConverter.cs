using System;
using System.Globalization;
using System.Windows.Data;

namespace UDPMonitor.Converters
{
    public sealed class DateTimeSmartConverter : IValueConverter
        {
            private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;
            private const string InputFormat = "yyyy-MM-ddTHH:mm:ss.fff";

            // param opzionale:
            // - "time" -> solo HH:mm:ss.fff
            // - "full" -> yyyy-MM-dd HH:mm:ss.fff
            // - default -> smart (oggi = time, altrimenti short date+time)
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value == null) return string.Empty;

                var mode = (parameter as string)?.Trim().ToLowerInvariant();

                DateTime dt;

                if (value is DateTime d)
                {
                    dt = d;
                }
                else
                {
                    var s = value as string;
                    if (string.IsNullOrWhiteSpace(s))
                        return string.Empty;

                    if (!DateTime.TryParseExact(s, InputFormat, Invariant,
                            DateTimeStyles.AssumeLocal, out dt))
                    {
                        // fallback generico (se un domani cambi formato)
                        if (!DateTime.TryParse(s, culture, DateTimeStyles.AssumeLocal, out dt))
                            return s;
                    }
                }

                // Formati output
                const string timeFmt = "HH:mm:ss.fff";
                const string fullFmt = "yyyy-MM-dd HH:mm:ss.fff";
                const string shortDateTimeFmt = "MM-dd HH:mm:ss.fff"; // compatto per tabella

                if (mode == "time")
                    return dt.ToString(timeFmt, Invariant);

                if (mode == "full")
                    return dt.ToString(fullFmt, Invariant);

                // smart
                var today = DateTime.Today;
                if (dt.Date == today)
                    return dt.ToString(timeFmt, Invariant);

                return dt.ToString(shortDateTimeFmt, Invariant);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
                => Binding.DoNothing;
        }

}
