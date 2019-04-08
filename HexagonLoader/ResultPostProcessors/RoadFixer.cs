
//using System.Composition;

using WorldHexagonMap.Core.Domain.Constants;

namespace WorldHexagonMap.HexagonDataLoader.ResultPostProcessors
{

    /// <summary>
    /// Fixes inconsistencies on roads
    /// </summary>
    //[Export("postprocessor_handler_road_fixer", typeof(IPostProcessor))]
    public class RoadFixer : PathFixer
    {
        protected override string ProcessedHexagonDataType => HexagonDataType.Road;
    }
}
