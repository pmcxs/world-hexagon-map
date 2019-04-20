﻿namespace WorldHexagonMap.HexagonDataLoader.GeoDataParsers
{
    public interface IGeoDataParserFactory
    {
        IGeoDataParser GetInstance(string source);

        IGeoDataParser GetInstance<T>() where T : IGeoDataParser;
    }
}