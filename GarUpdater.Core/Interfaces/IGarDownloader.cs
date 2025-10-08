namespace GarUpdater.Core.Interfaces
{
    public interface IGarDownloader
    {
        /// <summary>
        /// Скачивает последний файл gar_delta_xml.zip в папку загрузки (Downloads). 
        /// </summary>
        /// // <returns>Локальный путь к скачанному ZIP.</returns>
        Task<string> DownloadLatestDeltaAsync(CancellationToken ct = default);
    }
}
