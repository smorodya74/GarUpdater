using GarUpdater.Core.Models;

namespace GarUpdater.Core.Interfaces
{
    public interface IGarXmlParser
    {
        /// <summary>
        /// Построчно десериализует XML-элемент указанного имени.
        /// </summary>
        /// <returns>T объект, модель которой описана в Core</returns>
        IAsyncEnumerable<HouseParam> ParseHouseParamsAsync(
            string xmlPath, 
            CancellationToken ct = default);
    }
}