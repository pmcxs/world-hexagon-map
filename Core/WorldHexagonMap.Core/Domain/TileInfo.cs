using System;
using System.Text;

namespace WorldHexagonMap.Core.Domain
{
    [Serializable]
    public class TileInfo
    {
        public TileInfo(int zoomLevel, int tileX, int tileY)
        {
            Z = zoomLevel;
            X = tileX;
            Y = tileY;
        }

        public int Z { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        private bool Equals(TileInfo other)
        {
            return Z == other.Z && X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Z;
                hashCode = (hashCode * 397) ^ X;
                hashCode = (hashCode * 397) ^ Y;
                return hashCode;
            }
        }

        public static string GetQuadkey(int z, int x, int y)
        {
            var quadKey = new StringBuilder();
            for (var i = z; i > 0; i--)
            {
                var digit = '0';
                var mask = 1 << (i - 1);
                if ((x & mask) != 0) digit++;
                if ((y & mask) != 0)
                {
                    digit++;
                    digit++;
                }

                quadKey.Append(digit);
            }

            return quadKey.ToString();
        }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TileInfo) obj);
        }
    }
}