using Gastromio.Core.Domain.Model.DishCategories;

namespace Gastromio.Core.Application.Commands.IncOrderOfDishCategory
{
    public class IncOrderOfDishCategoryCommand : ICommand<bool>
    {
        public IncOrderOfDishCategoryCommand(DishCategoryId categoryId)
        {
            CategoryId = categoryId;
        }
        
        public DishCategoryId CategoryId { get; } 
    }
}
