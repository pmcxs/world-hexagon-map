using System;

namespace HexagonDataLoader.Core.Common.Enums
{
    [Flags]
    public enum BoundaryMask
    {
        None = 0,
        TopLeft = 1,
        Top = 2,
        TopRight = 4,
        BottomRight = 8,
        Bottom = 16,
        BottomLeft = 32
    }
}