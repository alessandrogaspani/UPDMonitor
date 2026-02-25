using System.IO;
using System.Text;

namespace UDPMonitor.Core.Export
{
    public static class CSV
    {
        public static void GenerateCSV<T>(T data, string filePath, CsvGenerator<T> generator)
        {
            string directoryPath = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            FileStream stream = File.Open(filePath, FileMode.Create, FileAccess.Write);

            StringBuilder sb = new StringBuilder();

            using (StreamWriter writer = new StreamWriter(stream, generator.Encoding != null ? generator.Encoding : Encoding.ASCII))
            {
                sb.AppendLine(generator.BuildHeader(data));
                sb.Append(generator.BuildContent(data));

                writer.Write(sb.ToString());
            }

            stream.Close();
        }

        public static void GenerateCSV<T>(T data, string filePath, Func<T, string> buildContent, Func<T, string> buildHeader = null, Encoding encoding = null)
        {
            string directoryPath = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            FileStream stream = File.Open(filePath, FileMode.Create, FileAccess.Write);

            using (StreamWriter writer = new StreamWriter(stream, encoding != null ? encoding : Encoding.ASCII))
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine(buildHeader?.Invoke(data));
                sb.AppendLine(buildContent?.Invoke(data));

                writer.Write(sb.ToString());
            }

            stream.Close();
        }

        public static async Task GenerateCSVAsync<T>(T data, string filePath, Func<T, string> buildContent, Func<T, string> buildHeader = null, Encoding encoding = null)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                FileStream stream = File.Open(filePath, FileMode.Create, FileAccess.Write);

                using (StreamWriter writer = new StreamWriter(stream, encoding != null ? encoding : Encoding.ASCII))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(buildHeader?.Invoke(data));
                    sb.AppendLine(buildContent?.Invoke(data));

                    await writer.WriteAsync(sb.ToString());
                }

                stream.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static async Task GenerateCSVAsync<T>(T data, string filePath, CsvGenerator<T> generator)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                FileStream stream = File.Open(filePath, FileMode.Create, FileAccess.Write);

                using (StreamWriter writer = new StreamWriter(stream, generator.Encoding != null ? generator.Encoding : Encoding.ASCII))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine(generator.BuildHeader(data));
                    sb.Append(generator.BuildContent(data));

                    await writer.WriteAsync(sb.ToString());
                }

                stream.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static T ReadCSV<T>(string filePath, CsvGenerator<T> generator)
        {
            return generator.ParseCSV(filePath);
        }
    }

    public static class CSV_Extensions
    {
        public static void ToCSV<T>(this T data, string filePath, CsvGenerator<T> generator)
        {
            CSV.GenerateCSV(data, filePath, generator);
        }

        public static void ToCSV<T>(this T data, string filePath, Func<T, string> buildContent, Func<T, string> buildHeader = null, Encoding encoding = null)
        {
            CSV.GenerateCSV(data, filePath, buildContent, buildHeader, encoding);
        }

        public static async Task ToCSVAsync<T>(this T data, string filePath, Func<T, string> buildContent, Func<T, string> buildHeader = null, Encoding encoding = null)
        {
            await CSV.GenerateCSVAsync(data, filePath, buildContent, buildHeader, encoding);
        }

        public static async Task ToCSVAsync<T>(T data, string filePath, CsvGenerator<T> generator)
        {
            await CSV.GenerateCSVAsync(data, filePath, generator);
        }
    }
}