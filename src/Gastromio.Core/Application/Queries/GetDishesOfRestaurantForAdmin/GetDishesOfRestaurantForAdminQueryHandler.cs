using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.DTOs;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.DishCategories;
using Gastromio.Core.Domain.Model.Dishes;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Queries.GetDishesOfRestaurantForAdmin
{
    public class GetDishesOfRestaurantForAdminQueryHandler
        : IQueryHandler<GetDishesOfRestaurantForAdminQuery, ICollection<DishCategoryDTO>>
    {
        private readonly IRestaurantRepository restaurantRepository;
        private readonly IDishCategoryRepository dishCategoryRepository;
        private readonly IDishRepository dishRepository;

        public GetDishesOfRestaurantForAdminQueryHandler(
            IRestaurantRepository restaurantRepository,
            IDishCategoryRepository dishCategoryRepository,
            IDishRepository dishRepository
        )
        {
            this.restaurantRepository = restaurantRepository;
            this.dishCategoryRepository = dishCategoryRepository;
            this.dishRepository = dishRepository;
        }

        public async Task<Result<ICollection<DishCategoryDTO>>> HandleAsync(GetDishesOfRestaurantForAdminQuery query,
            User currentUser, CancellationToken cancellationToken = default)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (currentUser == null)
                return FailureResult<ICollection<DishCategoryDTO>>.Unauthorized();

            if (currentUser.Role < Role.RestaurantAdmin)
                return FailureResult<ICollection<DishCategoryDTO>>.Forbidden();

            Restaurant restaurant;

            if (Guid.TryParse(query.RestaurantId, out var restaurantId))
            {
                restaurant =
                    await restaurantRepository.FindByRestaurantIdAsync(new RestaurantId(restaurantId),
                        cancellationToken);
            }
            else
            {
                restaurant =
                    (await restaurantRepository.FindByRestaurantNameAsync(query.RestaurantId, cancellationToken))
                    .FirstOrDefault();
            }

            if (restaurant == null)
                return FailureResult<ICollection<DishCategoryDTO>>.Create(FailureResultCode.RestaurantDoesNotExist);

            var dishCategories =
                await dishCategoryRepository.FindByRestaurantIdAsync(restaurant.Id, cancellationToken);
            var dishCategoryList = dishCategories != null ? dishCategories.ToList() : new List<DishCategory>();

            var dishes = await dishRepository.FindByRestaurantIdAsync(restaurant.Id, cancellationToken);
            var dishList = dishes != null ? dishes.ToList() : new List<Dish>();

            var result = new List<DishCategoryDTO>();

            foreach (var dishCategory in dishCategoryList.OrderBy(en => en.OrderNo))
            {
                var dishesOfCategory = dishList.Where(en => en.CategoryId == dishCategory.Id)
                    .OrderBy(en => en.OrderNo);

                result.Add(new DishCategoryDTO(dishCategory, dishesOfCategory));
            }

            return SuccessResult<ICollection<DishCategoryDTO>>.Create(result);
        }
    }
}
