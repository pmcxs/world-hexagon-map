using System;

namespace WorldHexagonMap.Core.Domain
{
    [Serializable]
    public class HexagonDefinition
    {

        public HexagonDefinition(double edgeSize)
        {
            EdgeSize = edgeSize;


            double h = Math.Sin(30.0 * Math.PI / 180) * edgeSize;
            double r = Math.Cos(30.0 * Math.PI / 180) * edgeSize;
            double b = edgeSize + 2.0 * h;
            double a = 2.0 * r;

            NarrowWidth = edgeSize + h;
            Diameter = b;
            Height = a;


        }

        public double EdgeSize { get; set; }

        public double Diameter { get; set; }

        public double Height { get; set; }

        public double NarrowWidth { get; set; }
    }
}
