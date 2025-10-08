using GarUpdater.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.IO.Compression;

namespace GarUpdater.Infrastructure.Services
{
    public class GarExtractor : IGarExtractor
    {
        private readonly ILogger<GarExtractor> _logger;
        private readonly string _extractFolder;

        public GarExtractor(ILogger<GarExtractor> logger)
        {
            _logger = logger;

            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            _extractFolder = Path.Combine(userProfile, "Downloads", "GarUpdater", "Extracted");
        }

        public async Task<string> ExtractAsync(string zipPath, CancellationToken ct = default)
        {
            if (!File.Exists(zipPath))
                throw new FileNotFoundException("Архив для распаковки не найден", zipPath);

            // Определяем папку архива
            var archiveFolder = Path.GetDirectoryName(zipPath) ?? 
                throw new InvalidOperationException("Не удалось определить папку архива");

            // Создаём подпапку для распаковки
           

            _logger.LogInformation("Начинаем распаковку {Zip} в {Folder}", zipPath, _extractFolder);

            await Task.Run(() => ZipFile.ExtractToDirectory(zipPath, _extractFolder, true), ct);

            _logger.LogInformation("Распаковка завершена. Файлы находятся в {Folder}", _extractFolder);

            return _extractFolder;
        }
    }
}
