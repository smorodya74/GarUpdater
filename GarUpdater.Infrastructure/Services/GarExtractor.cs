using GarUpdater.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.IO.Compression;

namespace GarUpdater.Infrastructure.Services
{
    public class GarExtractor : IGarExtractor
    {
        private readonly ILogger<GarExtractor> _logger;

        public GarExtractor(ILogger<GarExtractor> logger)
        {
            _logger = logger;
        }

        public async Task<string> ExtractAsync(string zipPath, CancellationToken ct = default)
        {
            if (!File.Exists(zipPath))
                throw new FileNotFoundException("Архив для распаковки не найден", zipPath);

            // Определяем папку архива
            var archiveFolder = Path.GetDirectoryName(zipPath) ?? 
                throw new InvalidOperationException("Не удалось определить папку архива");

            // Создаём подпапку для распаковки
            var extractFolder = Path.Combine(archiveFolder, "Extracted");
            Directory.CreateDirectory(extractFolder);

            _logger.LogInformation("Начинаем распаковку {Zip} в {Folder}", zipPath, extractFolder);

            await Task.Run(() => ZipFile.ExtractToDirectory(zipPath, extractFolder, true), ct);

            _logger.LogInformation("Распаковка завершена. Файлы находятся в {Folder}", extractFolder);

            return extractFolder;
        }
    }
}
