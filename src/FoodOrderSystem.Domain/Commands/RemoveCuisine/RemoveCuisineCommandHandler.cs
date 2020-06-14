﻿using FoodOrderSystem.Domain.Model;
using FoodOrderSystem.Domain.Model.Cuisine;
using FoodOrderSystem.Domain.Model.User;
using System;
using System.Threading;
using System.Threading.Tasks;
using FoodOrderSystem.Domain.Model.Restaurant;

namespace FoodOrderSystem.Domain.Commands.RemoveCuisine
{
    public class RemoveCuisineCommandHandler : ICommandHandler<RemoveCuisineCommand, bool>
    {
        private readonly ICuisineRepository cuisineRepository;
        private readonly IRestaurantRepository restaurantRepository;

        public RemoveCuisineCommandHandler(ICuisineRepository cuisineRepository, IRestaurantRepository restaurantRepository)
        {
            this.cuisineRepository = cuisineRepository;
            this.restaurantRepository = restaurantRepository;
        }

        public async Task<Result<bool>> HandleAsync(RemoveCuisineCommand command, User currentUser, CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (currentUser == null)
                return FailureResult<bool>.Unauthorized();

            if (currentUser.Role < Role.SystemAdmin)
                return FailureResult<bool>.Forbidden();

            var restaurants = await restaurantRepository.FindByCuisineIdAsync(command.CuisineId, cancellationToken);
            foreach (var restaurant in restaurants)
            {
                restaurant.RemoveCuisine(command.CuisineId, currentUser.Id);
                await restaurantRepository.StoreAsync(restaurant, cancellationToken);
            }
            
            await cuisineRepository.RemoveAsync(command.CuisineId, cancellationToken);

            return SuccessResult<bool>.Create(true);
        }
    }
}
