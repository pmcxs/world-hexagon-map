using HexagonDataLoader.Core.Common.Constants;

namespace HexagonDataLoder.PostProcessors
{
    /// <summary>
    ///     Fixes inconsistencies on railways
    /// </summary>
    //[Export("postprocessor_handler_railway_fixer", typeof(IPostProcessor))]
    public class RailwayFixer : PathFixer
    {
        protected override string ProcessedHexagonDataType => HexagonDataType.Railway;
    }
}