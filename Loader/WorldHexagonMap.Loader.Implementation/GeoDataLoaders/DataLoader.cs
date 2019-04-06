using System;
using System.Linq;
using WorldHexagonMap.Core.Domain;
using GeoAPI.Geometries;

namespace WorldHexagonMap.Loader.Implementation.GeoDataLoaders
{
    public abstract class DataLoader
    {
        
        protected PointXY[][] ConvertGeometryToPointXY(IGeometry geometry)
        {
            PointXY[][] points = null;

            if (geometry.GeometryType == "LineString")
            {
                var lineString = geometry as ILineString;
                if (lineString == null)
                {
                    throw new ArgumentException("Invalid ILineString instance");
                }

                var coordinates = lineString.Coordinates;
                points = new PointXY[1][];
                points[0] = coordinates.Select(c => new PointXY(c.X, c.Y)).ToArray();

            }
            else if (geometry.GeometryType == "Polygon")
            {
                var polygon = geometry as IPolygon;
                if (polygon == null)
                {
                    throw new ArgumentException("Invalid IPolygon instance");
                }

                var coordinates = polygon.Coordinates;
                points = new PointXY[1][];
                points[0] = coordinates.Select(c => new PointXY(c.X, c.Y)).ToArray();
                
            }
            else if (geometry.GeometryType == "MultiPolygon")
            {
                var multiPolygon = geometry as IMultiPolygon;
                if (multiPolygon == null)
                {
                    throw new ArgumentException("Invalid IMultiPolygon instance");
                }

                int numGeometries = multiPolygon.Geometries.Length;

                points = new PointXY[numGeometries][];

                for (var i = 0; i < numGeometries; i++)
                {
                    var polygon = multiPolygon.Geometries[i] as IPolygon;
                    if (polygon == null)
                    {
                        throw new ArgumentException("Invalid IPolygon instance");
                    }

                    var polygonCoordinates = polygon.Coordinates;
                    points[i] = polygonCoordinates.Select(c => new PointXY(c.X, c.Y)).ToArray(); 
                }

            }
            else if (geometry.GeometryType == "MultiLineString")
            {
                var multiLineString = geometry as IMultiLineString;
                if (multiLineString == null)
                {
                    throw new ArgumentException("Invalid IMultiLineString instance");
                }

                int numGeometries = multiLineString.Geometries.Length;

                points = new PointXY[numGeometries][];

                for (var i = 0; i < numGeometries; i++)
                {
                    var lineString = multiLineString.Geometries[i] as ILineString;
                    if (lineString == null)
                    {
                        throw new ArgumentException("Invalid ILineString instance");
                    }

                    var lineStringCoordinates = lineString.Coordinates;
                    points[i] = lineStringCoordinates.Select(c => new PointXY(c.X, c.Y)).ToArray();
                }
            }

            return points;
        }
    }
}
