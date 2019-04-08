
using WorldHexagonMap.Core.Domain.Constants;

namespace WorldHexagonMap.HexagonDataLoader.ResultPostProcessors
{

    /// <summary>
    /// Fixes inconsistencies on railways
    /// </summary>
    //[Export("postprocessor_handler_railway_fixer", typeof(IPostProcessor))]
    public class RailwayFixer : PathFixer
    {
        protected override string ProcessedHexagonDataType => HexagonDataType.Railway;
    }
}
