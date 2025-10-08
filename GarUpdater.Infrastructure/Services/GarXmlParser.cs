using GarUpdater.Core.Interfaces;
using GarUpdater.Core.Models;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Xml;

namespace GarUpdater.Infrastructure.Services
{
        public class GarXmlParser : IGarXmlParser
        {
            private readonly ILogger<GarXmlParser> _logger;

            public GarXmlParser(ILogger<GarXmlParser> logger)
            {
                _logger = logger;
            }

            public async IAsyncEnumerable<HouseParam> ParseHouseParamsAsync(
                string xmlPath,
                [EnumeratorCancellation] CancellationToken ct = default)
            {
                _logger.LogInformation("Начинаем парсинг XML: {Path}", xmlPath);

                using var reader = XmlReader.Create(xmlPath, new XmlReaderSettings
                {
                    Async = true,
                    IgnoreComments = true,
                    IgnoreWhitespace = true
                });

                while (await reader.ReadAsync())
                {
                    ct.ThrowIfCancellationRequested();

                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "PARAM")
                    {
                        var hp = new HouseParam
                        {
                            Id = reader.GetAttribute("ID")!,
                            ObjectId = reader.GetAttribute("OBJECTID")!,
                            ChangeId = reader.GetAttribute("CHANGEID")!,
                            ChangeIdEnd = reader.GetAttribute("CHANGEIDEND"),
                            TypeId = reader.GetAttribute("TYPEID")!,
                            Value = reader.GetAttribute("VALUE"),
                            UpdateDate = DateTime.Parse(reader.GetAttribute("UPDATEDATE")!),
                            StartDate = DateTime.Parse(reader.GetAttribute("STARTDATE")!),
                            EndDate = DateTime.Parse(reader.GetAttribute("ENDDATE")!)
                        };

                        yield return hp;
                    }
                }
                _logger.LogInformation("Парсинг завершён");
            }
        }
    }
