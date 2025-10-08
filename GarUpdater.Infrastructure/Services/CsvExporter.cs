using GarUpdater.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text;

namespace GarUpdater.Infrastructure.Services
{
    public class CsvExporter : ICsvExporter
    {
        private readonly ILogger<CsvExporter> _logger;
        private readonly string _resultFolder;

        public CsvExporter(ILogger<CsvExporter> logger)
        {
            _logger = logger;

            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            _resultFolder = Path.Combine(userProfile, "Downloads", "GarUpdater", "Result");
        }

        public async Task ExportAsync<T>(IAsyncEnumerable<T> records, string csvFileName, CancellationToken ct = default)
        {
            Directory.CreateDirectory(_resultFolder);

            var csvPath = Path.Combine(_resultFolder, Path.GetFileNameWithoutExtension(csvFileName) + ".csv");

            _logger.LogInformation("Начинаем экспорт в CSV: {Path}", csvPath);

            await using var writer = new StreamWriter(csvPath, false, Encoding.UTF8);
            var props = typeof(T).GetProperties();

            // Заголовки CSV
            await writer.WriteLineAsync(string.Join(",", props.Select(p => p.Name)));

            // Строки CSV
            await foreach (var record in records.WithCancellation(ct))
            {
                var values = props.Select(p =>
                    (p.GetValue(record)?.ToString() ?? "")
                        .Replace("\"", "\"\"")
                        .Replace(",", " ")
                );

                await writer.WriteLineAsync(string.Join(",", values));
            }

            _logger.LogInformation("Экспорт завершён: {Path}", csvPath);
        }
    }
}
