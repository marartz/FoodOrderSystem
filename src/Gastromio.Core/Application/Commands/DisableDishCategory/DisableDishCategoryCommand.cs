using Gastromio.Core.Domain.Model.DishCategories;

namespace Gastromio.Core.Application.Commands.DisableDishCategory
{
    public class DisableDishCategoryCommand : ICommand<bool>
    {
        public DisableDishCategoryCommand(DishCategoryId categoryId)
        {
            CategoryId = categoryId;
        }

        public DishCategoryId CategoryId { get; }
    }
}
