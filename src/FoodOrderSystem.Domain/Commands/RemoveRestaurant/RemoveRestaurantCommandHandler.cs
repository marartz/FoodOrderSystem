﻿using FoodOrderSystem.Domain.Model;
using FoodOrderSystem.Domain.Model.Dish;
using FoodOrderSystem.Domain.Model.DishCategory;
using FoodOrderSystem.Domain.Model.Restaurant;
using FoodOrderSystem.Domain.Model.User;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FoodOrderSystem.Domain.Commands.RemoveRestaurant
{
    public class RemoveRestaurantCommandHandler : ICommandHandler<RemoveRestaurantCommand, bool>
    {
        private readonly IRestaurantRepository restaurantRepository;
        private readonly IDishCategoryRepository dishCategoryRepository;
        private readonly IDishRepository dishRepository;

        public RemoveRestaurantCommandHandler(IRestaurantRepository restaurantRepository, IDishCategoryRepository dishCategoryRepository, IDishRepository dishRepository)
        {
            this.restaurantRepository = restaurantRepository;
            this.dishCategoryRepository = dishCategoryRepository;
            this.dishRepository = dishRepository;
        }

        public async Task<Result<bool>> HandleAsync(RemoveRestaurantCommand command, User currentUser, CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (currentUser == null)
                return FailureResult<bool>.Unauthorized();

            if (currentUser.Role < Role.SystemAdmin)
                return FailureResult<bool>.Forbidden();

            await dishRepository.RemoveByRestaurantIdAsync(command.RestaurantId, cancellationToken);
            await dishCategoryRepository.RemoveByRestaurantIdAsync(command.RestaurantId, cancellationToken);
            await restaurantRepository.RemoveAsync(command.RestaurantId, cancellationToken);

            return SuccessResult<bool>.Create(true);
        }
    }
}
