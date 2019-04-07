using System;
//using System.Composition;
using WorldHexagonMap.Core.Domain;
using WorldHexagonMap.Core.Domain.Enums;

namespace WorldHexagonMap.Loader.PostProcessors
{

    /// <summary>
    /// Returns a mask indicating the various edges on which this hexagon has a downward slope 
    /// </summary>
    //[Export("postprocessor_handler_slope", typeof(IPostProcessor))]
    public class Slope : IPostProcessor
    {
        public void ProcessHexagon(Hexagon hexagon, Hexagon[] neighbourHexagons)
        {
            hexagon.HexagonData.Slope = BoundaryMask.None;

            for (int i = 0; i < neighbourHexagons.Length; i++)
            {

                if (hexagon.HexagonData.Level > neighbourHexagons[i].HexagonData.Level)
                {
                    hexagon.HexagonData.Slope |= (BoundaryMask)((int)Math.Pow(2, i));
                }
            }
        }
    }
}
