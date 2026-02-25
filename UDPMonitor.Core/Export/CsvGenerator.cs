using System.Globalization;
using System.Text;

namespace UDPMonitor.Core.Export
{
    public abstract class CsvGenerator<T>
    {
        public string Directory { get; set; }
        public string FileName { get; set; }
        public string CompletePath { get => $@"{Directory}\{FileName}"; }

        public string Separator { get; set; }
        public NumberFormatInfo NumberFormat { get; set; }

        public abstract string BuildHeader(T data);

        public abstract string BuildContent(T data);

        public Encoding Encoding { get; }

        protected CsvGenerator(string separator = "#", Encoding encoding = null, NumberFormatInfo numberFormat = null)
        {
            Separator = separator;
            Encoding = encoding != null ? encoding : Encoding.ASCII;
            NumberFormat = numberFormat != null ? numberFormat : new CultureInfo("en-US", false).NumberFormat;
        }

        public abstract T ParseCSV(string csvFilePath);
    }
}