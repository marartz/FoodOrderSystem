using Gastromio.Core.Domain.Model.Dishes;

namespace Gastromio.Core.Application.Commands.DecOrderOfDish
{
    public class DecOrderOfDishCommand : ICommand<bool>
    {
        public DecOrderOfDishCommand(DishId dishId)
        {
            DishId = dishId;
        }

        public DishId DishId { get; }
    }
}
