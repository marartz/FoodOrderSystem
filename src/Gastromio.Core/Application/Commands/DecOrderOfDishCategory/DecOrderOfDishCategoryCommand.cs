using Gastromio.Core.Domain.Model.DishCategories;

namespace Gastromio.Core.Application.Commands.DecOrderOfDishCategory
{
    public class DecOrderOfDishCategoryCommand : ICommand<bool>
    {
        public DecOrderOfDishCategoryCommand(DishCategoryId categoryId)
        {
            CategoryId = categoryId;
        }
        
        public DishCategoryId CategoryId { get; } 
    }
}
