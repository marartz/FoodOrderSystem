using Gastromio.Core.Domain.Model.DishCategories;

namespace Gastromio.Core.Application.Commands.EnableDishCategory
{
    public class EnableDishCategoryCommand : ICommand<bool>
    {
        public EnableDishCategoryCommand(DishCategoryId categoryId)
        {
            CategoryId = categoryId;
        }

        public DishCategoryId CategoryId { get; }
    }
}
