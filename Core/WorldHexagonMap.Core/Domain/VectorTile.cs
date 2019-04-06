using System;
using System.Collections.Generic;

namespace WorldHexagonMap.Core.Domain
{
    [Serializable]
    public class VectorTile
    {
        public VectorTile()
        {
            Hexagons = new HashSet<Hexagon>();
        }

        public TileInfo TileInfo { get; set; }

        public HexagonDefinition HexagonDefinition { get; set; }

        public HashSet<Hexagon> Hexagons { get; set; }

        public override int GetHashCode()
        {
            return (TileInfo.Z + "_" + TileInfo.X + "_" + TileInfo.Y).GetHashCode();
        }

        public void AddHexagon(Hexagon hexagon)
        {
            Hexagons.Add(hexagon);
        }


        public Dictionary<string, Hexagon> GetHexagonsDictionary()
        {
            var hexagons = new Dictionary<string, Hexagon>();

            foreach (Hexagon hexagon in Hexagons)
            {
                hexagons[hexagon.LocationUV.U + "_" + hexagon.LocationUV.V] = hexagon;
            }

            return hexagons;

        }
    }
}