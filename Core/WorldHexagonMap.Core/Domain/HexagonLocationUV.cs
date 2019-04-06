using System;

namespace WorldHexagonMap.Core.Domain
{
    /// <summary>
    /// Represents an hexagon coordinate in U,V terms
    /// </summary>
    /// <remarks>
    /// The U,V axis is represented with u:0,v:0 on the top-left corner.
    /// 
    ///                   *********
    ///                  *         *
    ///                 *           *
    ///                *             *
    ///       *********    U:1,V:-1   *
    ///      *         *             *
    ///     *           *           *
    ///    *             *         *
    ///   *    U:0,V:0    *********
    ///    *             *         *
    ///     *           *           *
    ///      *         *             *
    ///       *********    U:1,V:0    *
    ///                *             *
    ///                 *           *
    ///                  *         *
    ///                   *********
    /// 
    /// </remarks>
    [Serializable]
    public class HexagonLocationUV
    {
        public HexagonLocationUV() { }

        public HexagonLocationUV(int u, int v)
        {
            U = u;
            V = v;
        }

        /// <summary>
        /// Horizontal component of the hexagon
        /// </summary>
        public int U { get; set; }

        /// <summary>
        /// Vertical component of the hexagon
        /// </summary>
        public int V { get; set; }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((HexagonLocationUV) obj);
        }

        protected bool Equals(HexagonLocationUV other)
        {
            return U == other.U && V == other.V;
        }

        public override int GetHashCode()
        {
            return (U + "_" + V).GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("U:{0} V:{1}", this.U, this.V);
        }
    }
}
