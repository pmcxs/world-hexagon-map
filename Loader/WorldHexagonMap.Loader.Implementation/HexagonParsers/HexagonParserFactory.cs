using System;
using WorldHexagonMap.Core.Contracts;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Loader.Domain.Enums;

namespace WorldHexagonMap.Loader.HexagonParsers
{
    public class HexagonParserFactory : IHexagonParserFactory
    {
        private readonly IHexagonService _hexagonService;
        private readonly HexagonDefinition _hexagonDefinition;

        public HexagonParserFactory(IHexagonService hexagonService, HexagonDefinition hexagonDefinition)
        {
            _hexagonService = hexagonService;
            _hexagonDefinition = hexagonDefinition;
        }

        public IHexagonParser GetInstance(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.Way:
                    return new WayParser(_hexagonService, _hexagonDefinition);
                case DataType.Area:
                    return new AreaParser(_hexagonService, _hexagonDefinition);
                case DataType.Pixel:
                    return new PixelParser(_hexagonService, _hexagonDefinition);
                default:
                    throw new NotSupportedException("DataType not supported: " + dataType);
                   
            }
        }
    }
}