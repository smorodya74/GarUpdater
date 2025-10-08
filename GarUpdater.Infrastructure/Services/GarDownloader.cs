using GarUpdater.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GarUpdater.Infrastructure.Services
{
    public class GarDownloader : IGarDownloader
    {
        private readonly HttpClient _http;
        private readonly ILogger<GarDownloader> _logger;
        private readonly string _downloadUrl;
        private readonly string _targetFileName;
        private readonly string _downloadFolder;

        public GarDownloader(IHttpClientFactory httpFactory, IConfiguration config, ILogger<GarDownloader> logger)
        {
            _http = httpFactory.CreateClient("GarUpdaterClient");
            _logger = logger;

            var section = config.GetSection("GarUpdater");
            _downloadUrl = section.GetValue<string>("DownloadUrl")
                           ?? throw new ArgumentException("GarUpdater:DownloadUrl is not configured");

            _targetFileName = section.GetValue<string>("TargetFileName") ?? "gar_delta_xml.zip";

            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            _downloadFolder = Path.Combine(userProfile, "Downloads", "GarUpdater");


        }

        public async Task<string> DownloadLatestDeltaAsync(CancellationToken ct = default)
        {
            Directory.CreateDirectory(_downloadFolder);
            var zipPath = Path.Combine(_downloadFolder, _targetFileName);

            _logger.LogInformation("Начинаем скачивание {Url}", _downloadUrl);

            using var responce = await _http.GetAsync(_downloadUrl, HttpCompletionOption.ResponseHeadersRead, ct);
            responce.EnsureSuccessStatusCode();

            var totalBytes = responce.Content.Headers.ContentLength ?? -1L;
            var canReportProgress = totalBytes != -1;

            await using var fs = File.Create(zipPath);
            await using var rs = await responce.Content.ReadAsStreamAsync(ct);

            var buffer = new byte[81920]; // 80 KB
            long totalRead = 0;
            int read;

            Console.WriteLine($"Скачиваем: {_targetFileName}");
            while ((read = await rs.ReadAsync(buffer.AsMemory(0, buffer.Length), ct)) > 0)
            {
                await fs.WriteAsync(buffer.AsMemory(0, read), ct);
                totalRead += read;

                if (canReportProgress)
                {
                    int percent = (int)((totalRead * 100) / totalBytes);
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write($"Прогресс: {percent}%   ");
                }
            }

            if (canReportProgress)
                Console.WriteLine(); // перевод строки после завершения

            _logger.LogInformation("Архив успешно скачан в {Path}", zipPath);
            return zipPath;
        }
    }
}
