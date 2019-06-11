using System;

namespace HexagonDataLoader.Core.Common
{
    [Serializable]
    public class HexagonDefinition
    {
        public HexagonDefinition(double edgeSize, int referenceZoom)
        {
            EdgeSize = edgeSize;


            var h = Math.Sin(30.0 * Math.PI / 180) * edgeSize;
            var r = Math.Cos(30.0 * Math.PI / 180) * edgeSize;
            var b = edgeSize + 2.0 * h;
            var a = 2.0 * r;

            NarrowWidth = edgeSize + h;
            Diameter = b;
            Height = a;

            ReferenceZoom = referenceZoom;
        }

        public double EdgeSize { get; }

        public double Diameter { get; }

        public double Height { get; }

        public double NarrowWidth { get; }

        public int ReferenceZoom { get; }
    }
}