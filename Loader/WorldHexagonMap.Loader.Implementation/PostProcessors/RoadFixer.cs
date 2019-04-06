﻿
using System.Composition;
using WorldHexagonMap.Core.Domain.Constants;
using WorldHexagonMap.Loader.Contracts;

namespace WorldHexagonMap.Loader.Implementation.PostProcessors
{

    /// <summary>
    /// Fixes incoherences on roads
    /// </summary>
    [Export("postprocessor_handler_road_fixer", typeof(IPostProcessor))]
    public class RoadFixer : WayFixer
    {
        protected override string ProcessedHexagonDataType
        {
            get { return HexagonDataType.Road; }
        }



    }
}