﻿using MapGenerator.Core.Contracts;

namespace HexagonDataLoder.PostProcessors
{
    public interface IPostProcessorFactory
    {
        IPostProcessor GetInstance(string name);
    }
}