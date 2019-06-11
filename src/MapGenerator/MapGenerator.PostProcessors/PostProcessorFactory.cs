using MapGenerator.Core.Contracts;
using System;

namespace HexagonDataLoder.PostProcessors
{
    public class PostProcessorFactory : IPostProcessorFactory
    {
        public IPostProcessor GetInstance(string name)
        {
            //TODO: Plugable logic instead of having this hardcoded?
            switch (name)
            {
                case "postprocessor_handler_level_normalizer":
                    return new LevelNormalizer();
                case "postprocessor_handler_slope":
                    return new Slope();
                case "postprocessor_handler_area_edges":
                    return new AreaEdges();
                case "postprocessor_handler_road_fixer":
                    return new RoadFixer();
                case "postprocessor_handler_railway_fixer":
                    return new RailwayFixer();
                default:
                    throw new NotSupportedException($"Type {name} does not exist");

            }
        }
    }
}