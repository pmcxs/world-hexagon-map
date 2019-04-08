
using WorldHexagonMap.Core.Domain.Constants;

namespace WorldHexagonMap.Loader.PostProcessors
{

    /// <summary>
    /// Fixes incoherences on roads
    /// </summary>
    //[Export("postprocessor_handler_railway_fixer", typeof(IPostProcessor))]
    public class RailwayFixer : WayFixer
    {
        protected override string ProcessedHexagonDataType
        {
            get { return HexagonDataType.Railway; }
        }



    }
}
