using System;
using System.Collections.Generic;
using BitMiracle.LibTiff.Classic;
using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.HexagonDataLoader.GeoDataParsers
{
    public class GeoTiffParser : IGeoDataParser
    {
        /// <summary>
        ///     Information obtained from: http://duff.ess.washington.edu/data/raster/drg/docs/geotiff.txt
        /// </summary>
        /// <param name="sourceData"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public IEnumerable<GeoData> ParseGeodataFromSource(LayersConfiguration sourceData, string filePath)
        {
            Tiff.SetErrorHandler(new NoWarningsTiffErrorHandler());
            using (var tiff = Tiff.Open(filePath, "r"))
            {
                var modelPixelScaleTag = tiff.GetField((TiffTag) 33550);
                var modelTiepointTag = tiff.GetField((TiffTag) 33922);

                var modelPixelScale = modelPixelScaleTag[1].GetBytes();
                var pixelSizeX = BitConverter.ToDouble(modelPixelScale, 0);
                var pixelSizeY = BitConverter.ToDouble(modelPixelScale, 8) * -1;

                var modelTransformation = modelTiepointTag[1].GetBytes();
                var originLon = BitConverter.ToDouble(modelTransformation, 24);
                var originLat = BitConverter.ToDouble(modelTransformation, 32);

                var startLat = originLat + pixelSizeY / 2.0;
                var startLon = originLon + pixelSizeX / 2.0;


                //TODO: Check if band is stored in 1 byte or 2 bytes. If 2, the following code would be required
                //var scanline16Bit = new ushort[tiff.ScanlineSize() / 2];
                //Buffer.BlockCopy(scanline, 0, scanline16Bit, 0, scanline.Length);

                var currentLat = startLat;
                var currentLon = startLon;

                var imageLength = tiff.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                var buf = new byte[tiff.ScanlineSize()];

                for (var i = 0; i < imageLength; i++)
                {
                    tiff.ReadScanline(buf, i); //Loading ith Line            

                    var latitude = currentLat + pixelSizeY * i;

                    for (var j = 0; j < buf.Length; j++)
                    {
                        var longitude = currentLon + pixelSizeX * j;

                        var geodata = new GeoData
                        {
                            Values = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase),
                            Points = new PointXY[1][]
                        };

                        if (sourceData.Interpolate)
                            geodata.Points[0] = new[]
                            {
                                new PointXY(longitude - pixelSizeX, latitude - pixelSizeY), //TOP-LEFT
                                new PointXY(longitude + pixelSizeX, latitude + pixelSizeY) //BOTTOM-RIGHT
                            };
                        else
                            geodata.Points[0] = new[] {new PointXY(longitude, latitude)};

                        geodata.Values.Add("custom_1", buf[j]);

                        yield return geodata;
                    }
                }
            }
        }

        private class NoWarningsTiffErrorHandler : TiffErrorHandler
        {
            public override void WarningHandler(Tiff tif, string method, string format, params object[] args)
            {
            }
        }

        public void Dispose()
        {
        }
    }
}