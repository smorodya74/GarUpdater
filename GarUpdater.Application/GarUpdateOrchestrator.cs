using GarUpdater.Core.Interfaces;

namespace GarUpdater.Application
{
    public class GarUpdateOrchestrator
    {
        private readonly IGarDownloader _downloader;
        private readonly IGarExtractor _extractor;
        private readonly IGarXmlParser _parser;
        private readonly ICsvExporter _exporter;

        public GarUpdateOrchestrator(
            IGarDownloader downloader,
            IGarExtractor extractor,
            IGarXmlParser parser,
            ICsvExporter exporter)
        {
            _downloader = downloader;
            _extractor = extractor;
            _parser = parser;
            _exporter = exporter;
        }

        public async Task RunAsync(CancellationToken ct = default)
        {
            int maxAttempts = 3;
            string? extractFolder = null;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    var zipPath = await _downloader.DownloadLatestDeltaAsync(ct);

                    extractFolder = await _extractor.ExtractAsync(zipPath, ct);

                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Попытка {attempt} не удалась: {ex.Message}");

                    if (attempt == maxAttempts)
                    {
                        Console.WriteLine("Не удалось скачать и распаковать архив после 3 попыток.");
                        throw;
                    }

                    Console.WriteLine("Повтор через 2 секунды...");
                    await Task.Delay(2000, ct);
                }
            }

            if (extractFolder is null)
                return;

            var xmlFile = Directory.GetFiles(extractFolder, "AS_HOUSES_PARAMS_*.XML", 
                SearchOption.AllDirectories).FirstOrDefault();

            if (xmlFile is null)
            {
                Console.WriteLine("Не найден файл AS_HOUSES_PARAMS_*.XML");
                return;
            }

            var csvPath = Path.ChangeExtension(xmlFile, ".csv");

            var records = _parser.ParseHouseParamsAsync(xmlFile, ct);
            await _exporter.ExportAsync(records, csvPath, ct);

            Console.WriteLine($"\nГотово! CSV создан: {csvPath}");
        }
    }
}