namespace WorldHexagonMap.Core.Domain
{
    public class BoundingBox
    {
        public double North;
        public double South;
        public double East;
        public double West;

        public BoundingBox()
        {

        }

        public BoundingBox(PointXY nw, PointXY se)
        {
            North = nw.Y;
            South = se.Y;
            West = nw.X;
            East = se.X;
        }
    }
}