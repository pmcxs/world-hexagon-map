using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.HexagonDataLoader.Domain;
using WorldHexagonMap.Loader.Domain;

namespace WorldHexagonMap.HexagonDataLoader.GeoDataParsers
{
    public class XYZParser : IGeoDataParser
    {

        public IEnumerable<GeoData> ParseGeodataFromSource(layersLoader sourceData, string filePath)
        {

            foreach (var line in File.ReadLines(filePath))
            {
                if (line == null)
                {
                    continue;
                }

                var components = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var longitude = double.Parse(components[0], new CultureInfo("en-US"));
                var latitude = double.Parse(components[1], new CultureInfo("en-US"));

                var geodata = new GeoData
                {
                    Values = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase),
                    Points = new PointXY[1][]
                };

                if (Math.Abs(sourceData.xoffset) > (decimal) 0.0 && Math.Abs(sourceData.yoffset) > (decimal) 0.0)
                {
                    geodata.Points[0] = new[]
                    {
                        new PointXY(longitude - (double) sourceData.xoffset, latitude - (double) sourceData.yoffset),   //TOP-LEFT
                        new PointXY(longitude + (double) sourceData.xoffset, latitude + (double) sourceData.yoffset),   //BOTTOM-RIGHT
                    };
                }
                else
                {
                   geodata.Points[0] = new[] {new PointXY(longitude, latitude)};  
                }


                IEnumerable<string> values = components.Skip(2);

                var fieldCount = 0;
                foreach (string value in values)
                {
                    geodata.Values.Add("custom_" + fieldCount++, value);
                }

                yield return geodata;

            }
            

        }

    }
}
