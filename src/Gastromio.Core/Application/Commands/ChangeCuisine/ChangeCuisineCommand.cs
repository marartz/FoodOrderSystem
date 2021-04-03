using Gastromio.Core.Domain.Model.Cuisines;

namespace Gastromio.Core.Application.Commands.ChangeCuisine
{
    public class ChangeCuisineCommand : ICommand<bool>
    {
        public ChangeCuisineCommand(CuisineId cuisineId, string name)
        {
            CuisineId = cuisineId;
            Name = name;
        }

        public CuisineId CuisineId { get; }
        public string Name { get; }
    }
}
