using System;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Core.Services;
using WorldHexagonMap.HexagonDataLoader.GeoDataParsers;

namespace WorldHexagonMap.HexagonDataLoader.HexagonProcessors
{
    public class HexagonProcessorFactory : IHexagonProcessorFactory
    {

        public IHexagonProcessor GetInstance(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.Path:
                    return new PathProcessor();
                case DataType.Area:
                    return new AreaProcessor();
                case DataType.Pixel:
                    return new PixelProcessor();
                default:
                    throw new NotSupportedException("DataType not supported: " + dataType);
            }
        }
    }
}