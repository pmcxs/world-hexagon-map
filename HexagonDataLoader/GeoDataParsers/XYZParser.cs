using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using NetTopologySuite.Geometries;
using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.HexagonDataLoader.GeoDataParsers
{
    public class XYZParser : IGeoDataParser
    {
        public IEnumerable<GeoData> ParseGeodataFromSource(LayersConfiguration sourceData, string filePath)
        {
            foreach (var line in File.ReadLines(filePath))
            {
                if (line == null) continue;

                var components = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                var longitude = double.Parse(components[0], new CultureInfo("en-US"));
                var latitude = double.Parse(components[1], new CultureInfo("en-US"));

                var geodata = new GeoData
                {
                    Values = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase),
                    Points = new PointXY[1][]
                };


                if (Math.Abs(sourceData.XOffset) > (decimal) 0.0 && Math.Abs(sourceData.YOffset) > (decimal) 0.0)
                    geodata.Points[0] = new[]
                    {
                        new PointXY(longitude - (double) sourceData.XOffset,
                            latitude - (double) sourceData.YOffset), //TOP-LEFT
                        
                        new PointXY(longitude + (double) sourceData.XOffset,
                            latitude + (double) sourceData.YOffset) //BOTTOM-RIGHT
                    };
                else
                {
                    geodata.Points[0] = new[] {new PointXY(longitude, latitude)};
                }

                var values = components.Skip(2);

                var fieldCount = 0;
                foreach (var value in values)
                {
                    geodata.Values.Add("custom_" + fieldCount++, value);
                }

                yield return geodata;
            }
        }

        public void Dispose()
        {
        }
    }
}