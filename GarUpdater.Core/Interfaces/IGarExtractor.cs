namespace GarUpdater.Core.Interfaces
{
    public interface IGarExtractor
    {
        /// <summary>
        /// Распаковывает загруженный архив.
        /// </summary>
        /// <returns>Путь к распакованной папке.</returns>
        Task<string> ExtractAsync(string zipPath, CancellationToken ct = default);
    }

}
