using System;

namespace WorldHexagonMap.Core.Domain.Enums
{
    /// <summary>
    /// </summary>
    [Flags]
    public enum WayMask
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