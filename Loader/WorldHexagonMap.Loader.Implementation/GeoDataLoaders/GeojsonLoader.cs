using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Loader.Contracts;
using WorldHexagonMap.Loader.Domain;
using GeoAPI.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.IO;

namespace WorldHexagonMap.Loader.GeoDataLoaders
{
    public class GeojsonLoader : IGeoDataLoader
    {

        public IEnumerable<GeoData> LoadGeodataFromSource(layersLoader sourceData, string filePath)
        {
            string jsonData = File.ReadAllText(filePath);

            var reader = new GeoJsonReader();

            var featureCollection = reader.Read<FeatureCollection>(jsonData);

            return featureCollection.Features.Select<IFeature, GeoData>(ConvertFeatureToGeoData);
        }

        protected GeoData ConvertFeatureToGeoData(IFeature feature)
        {
            var geoData = new GeoData { Values = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase) };

            if (feature.Geometry.GeometryType == "LineString")
            {
                var lineString = feature.Geometry as ILineString;
                if (lineString == null)
                {
                    throw new ArgumentException("Invalid ILineString instance");
                }

                var coordinates = lineString.Coordinates;
                geoData.Points = new PointXY[1][];
                geoData.Points[0] = coordinates.Select(c => new PointXY(c.X, c.Y)).ToArray();

            }
            else if (feature.Geometry.GeometryType == "Polygon")
            {
                var polygon = feature.Geometry as IPolygon;
                if (polygon == null)
                {
                    throw new ArgumentException("Invalid IPolygon instance");
                }

                var coordinates = polygon.Coordinates;
                geoData.Points = new PointXY[1][];
                geoData.Points[0] = coordinates.Select(c => new PointXY(c.X, c.Y)).ToArray();
                
            }
            else if (feature.Geometry.GeometryType == "MultiPolygon")
            {
                var multiPolygon = feature.Geometry as IMultiPolygon;
                if (multiPolygon == null)
                {
                    throw new ArgumentException("Invalid IMultiPolygon instance");
                }

                int numGeometries = multiPolygon.Geometries.Length;

                geoData.Points = new PointXY[numGeometries][];

                for (var i = 0; i < numGeometries; i++)
                {
                    var polygon = multiPolygon.Geometries[i] as IPolygon;
                    if (polygon == null)
                    {
                        throw new ArgumentException("Invalid IPolygon instance");
                    }

                    var polygonCoordinates = polygon.Coordinates;
                    geoData.Points[i] = polygonCoordinates.Select(c => new PointXY(c.X, c.Y)).ToArray(); 
                }

            }
            else if (feature.Geometry.GeometryType == "MultiLineString")
            {
                var multiLineString = feature.Geometry as IMultiLineString;
                if (multiLineString == null)
                {
                    throw new ArgumentException("Invalid IMultiLineString instance");
                }

                int numGeometries = multiLineString.Geometries.Length;

                geoData.Points = new PointXY[numGeometries][];

                for (var i = 0; i < numGeometries; i++)
                {
                    var lineString = multiLineString.Geometries[i] as ILineString;
                    if (lineString == null)
                    {
                        throw new ArgumentException("Invalid ILineString instance");
                    }

                    var lineStringCoordinates = lineString.Coordinates;
                    geoData.Points[i] = lineStringCoordinates.Select(c => new PointXY(c.X, c.Y)).ToArray();
                }
            }

            foreach (string attributeName in feature.Attributes.GetNames())
            {
                geoData.Values[attributeName] = feature.Attributes[attributeName];
            }

            return geoData;
        }
    }
}
