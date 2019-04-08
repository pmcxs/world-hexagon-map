using System;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Core.Services;
using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;

namespace WorldHexagonMap.HexagonDataLoader.HexagonProcessors
{
    public class HexagonProcessorFactory : IHexagonProcessorFactory
    {
        private readonly HexagonDefinition _hexagonDefinition;
        private readonly IHexagonService _hexagonService;

        public HexagonProcessorFactory(IHexagonService hexagonService, HexagonDefinition hexagonDefinition)
        {
            _hexagonService = hexagonService;
            _hexagonDefinition = hexagonDefinition;
        }

        public IHexagonProcessor GetInstance(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.Way:
                    return new PathProcessor(_hexagonService, _hexagonDefinition);
                case DataType.Area:
                    return new AreaProcessor(_hexagonService, _hexagonDefinition);
                case DataType.Pixel:
                    return new PixelProcessor(_hexagonService, _hexagonDefinition);
                default:
                    throw new NotSupportedException("DataType not supported: " + dataType);
            }
        }
    }
}