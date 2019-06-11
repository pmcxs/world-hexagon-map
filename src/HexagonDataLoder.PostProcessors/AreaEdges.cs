using HexagonDataLoader.Core.Common;
using HexagonDataLoader.Core.Common.Enums;
using HexagonDataLoader.Core.Contracts;
using System;

namespace HexagonDataLoder.PostProcessors
{
    /// <summary>
    ///     The area properties will have the following logic:
    ///     Using for example the urban:
    ///     0 - Is not a forest hexagon
    ///     1 - Is an urban hexagon, but it's not a boundary hexagon
    ///     The remaining bits act as a mask
    ///     xxxx1
    /// </summary>
    public class AreaEdges : IPostProcessor
    {
        public void ProcessHexagon(Hexagon hexagon, Hexagon[] neighbourHexagons)
        {
            hexagon.HexagonData.LandBoundary = BoundaryMask.None;
            for (var i = 0; i < neighbourHexagons.Length; i++)
            {
                if (hexagon.HexagonData.Forest > 0 && neighbourHexagons[i].HexagonData.Forest == 0)
                    hexagon.HexagonData.ForestBoundary |= (BoundaryMask)(int)Math.Pow(2, i);

                if (hexagon.HexagonData.Urban > 0 && neighbourHexagons[i].HexagonData.Urban == 0)
                    hexagon.HexagonData.UrbanBoundary |= (BoundaryMask)(int)Math.Pow(2, i);

                if (hexagon.HexagonData.Land > 0 && neighbourHexagons[i].HexagonData.Land == 0)
                    hexagon.HexagonData.LandBoundary |= (BoundaryMask)(int)Math.Pow(2, i);

                if (!string.IsNullOrEmpty(hexagon.HexagonData.Region) &&
                    (string.IsNullOrEmpty(neighbourHexagons[i].HexagonData.Region) ||
                     hexagon.HexagonData.Region != neighbourHexagons[i].HexagonData.Region))
                    hexagon.HexagonData.RegionBoundary |= (BoundaryMask)(int)Math.Pow(2, i);
            }
        }
    }
}