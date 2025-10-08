namespace GarUpdater.Core.Interfaces
{
    public interface ICsvExporter
    {
        /// <summary>
        /// Экспортирует поток данных в CSV-файл.
        /// </summary>
        Task ExportAsync<T>(
            IAsyncEnumerable<T> records, 
            string csvPath, 
            CancellationToken ct = default);
    }
}
