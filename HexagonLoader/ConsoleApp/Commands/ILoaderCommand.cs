using System.Collections.Generic;
using WorldHexagonMap.Core.Domain;

namespace WorldHexagonMap.HexagonDataLoader.ConsoleApp.Commands
{
    public interface ILoaderCommand
    {
        CommandResult Process(CommandArguments args);

    }
}