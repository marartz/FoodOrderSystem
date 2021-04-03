using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Cuisines;
using Gastromio.Core.Domain.Model.DishCategories;
using Gastromio.Core.Domain.Model.Dishes;
using Gastromio.Core.Domain.Model.PaymentMethods;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Microsoft.Extensions.Logging;

namespace Gastromio.Core.Application.Commands.AddTestData
{
    public class AddTestDataCommandHandler : ICommandHandler<AddTestDataCommand, bool>
    {
        private readonly ILogger<AddTestDataCommandHandler> logger;
        private readonly IUserFactory userFactory;
        private readonly IUserRepository userRepository;
        private readonly ICuisineFactory cuisineFactory;
        private readonly ICuisineRepository cuisineRepository;
        private readonly IPaymentMethodRepository paymentMethodRepository;
        private readonly IRestaurantFactory restaurantFactory;
        private readonly IRestaurantRepository restaurantRepository;
        private readonly IDishCategoryFactory dishCategoryFactory;
        private readonly IDishCategoryRepository dishCategoryRepository;
        private readonly IDishFactory dishFactory;
        private readonly IDishRepository dishRepository;

        private readonly Dictionary<string, User> restAdminDict = new Dictionary<string, User>();
        private readonly List<Cuisine> cuisines = new List<Cuisine>();

        private List<PaymentMethod> paymentMethods;

        public AddTestDataCommandHandler(
            ILogger<AddTestDataCommandHandler> logger,
            IUserFactory userFactory,
            IUserRepository userRepository,
            ICuisineFactory cuisineFactory,
            ICuisineRepository cuisineRepository,
            IPaymentMethodRepository paymentMethodRepository,
            IRestaurantFactory restaurantFactory,
            IRestaurantRepository restaurantRepository,
            IDishCategoryFactory dishCategoryFactory,
            IDishCategoryRepository dishCategoryRepository,
            IDishFactory dishFactory,
            IDishRepository dishRepository
        )
        {
            this.logger = logger;
            this.userFactory = userFactory;
            this.userRepository = userRepository;
            this.cuisineFactory = cuisineFactory;
            this.cuisineRepository = cuisineRepository;
            this.paymentMethodRepository = paymentMethodRepository;
            this.restaurantFactory = restaurantFactory;
            this.restaurantRepository = restaurantRepository;
            this.dishCategoryFactory = dishCategoryFactory;
            this.dishCategoryRepository = dishCategoryRepository;
            this.dishFactory = dishFactory;
            this.dishRepository = dishRepository;
        }

        public async Task<Result<bool>> HandleAsync(AddTestDataCommand command, User currentUser,
            CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (currentUser == null)
                return FailureResult<bool>.Unauthorized();

            if (currentUser.Role < Role.SystemAdmin)
                return FailureResult<bool>.Forbidden();

            paymentMethods = (await paymentMethodRepository.FindAllAsync(cancellationToken)).ToList();

            var tempResult = await CreateUsersAsync(currentUser, command.UserCount, cancellationToken);
            if (tempResult.IsFailure)
                return tempResult;

            tempResult = await CreateCuisinesAsync(currentUser, cancellationToken);
            if (tempResult.IsFailure)
                return tempResult;

            tempResult = await CreateRestaurantsAsync(currentUser, command.RestCount, command.DishCatCount, command.DishCount,
                cancellationToken);
            if (tempResult.IsFailure)
                return tempResult;

            return SuccessResult<bool>.Create(true);
        }

        private async Task<Result<bool>> CreateUsersAsync(User currentUser, int count, CancellationToken cancellationToken)
        {
            logger.LogInformation("creating test users");

            for (var i = 0; i < count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var name = $"sysadmin{(i + 1):D3}";
                var tempResult = userFactory.Create(Role.SystemAdmin, $"{name}@gastromio.de", "Start2020!",
                    true, currentUser.Id);
                if (tempResult.IsFailure)
                    return tempResult.Cast<bool>();

                var user = ((SuccessResult<User>) tempResult).Value;

                await userRepository.StoreAsync(user, cancellationToken);
                logger.LogInformation("    sysadmin user {0} created", name);
            }

            for (var i = 0; i < count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var name = $"restadmin{(i + 1):D3}";
                var tempResult = userFactory.Create(Role.SystemAdmin, $"{name}@gastromio.de", "Start2020!",
                    true, currentUser.Id);
                if (tempResult.IsFailure)
                    return tempResult.Cast<bool>();

                var user = ((SuccessResult<User>) tempResult).Value;

                await userRepository.StoreAsync(user, cancellationToken);
                logger.LogInformation("    restadmin user {0} created", name);

                restAdminDict.Add(name, user);
            }

            logger.LogInformation("test users created");
            return SuccessResult<bool>.Create(true);
        }

        private async Task<Result<bool>> CreateCuisinesAsync(User currentUser, CancellationToken cancellationToken)
        {
            logger.LogInformation("creating test cuisines");

            var tempResult = cuisineFactory.Create("Chinesisch", currentUser.Id);
            if (tempResult.IsFailure)
                return tempResult.Cast<bool>();
            var cuisine = ((SuccessResult<Cuisine>) tempResult).Value;
            await cuisineRepository.StoreAsync(cuisine, cancellationToken);
            cuisines.Add(cuisine);

            tempResult = cuisineFactory.Create("Griechisch", currentUser.Id);
            if (tempResult.IsFailure)
                return tempResult.Cast<bool>();
            cuisine = ((SuccessResult<Cuisine>) tempResult).Value;
            await cuisineRepository.StoreAsync(cuisine, cancellationToken);
            cuisines.Add(cuisine);

            tempResult = cuisineFactory.Create("Italienisch", currentUser.Id);
            if (tempResult.IsFailure)
                return tempResult.Cast<bool>();
            cuisine = ((SuccessResult<Cuisine>) tempResult).Value;
            await cuisineRepository.StoreAsync(cuisine, cancellationToken);
            cuisines.Add(cuisine);

            tempResult = cuisineFactory.Create("Amerikanisch", currentUser.Id);
            if (tempResult.IsFailure)
                return tempResult.Cast<bool>();
            cuisine = ((SuccessResult<Cuisine>) tempResult).Value;
            await cuisineRepository.StoreAsync(cuisine, cancellationToken);
            cuisines.Add(cuisine);

            tempResult = cuisineFactory.Create("Mexikanisch", currentUser.Id);
            if (tempResult.IsFailure)
                return tempResult.Cast<bool>();
            cuisine = ((SuccessResult<Cuisine>) tempResult).Value;
            await cuisineRepository.StoreAsync(cuisine, cancellationToken);
            cuisines.Add(cuisine);

            tempResult = cuisineFactory.Create("Französisch", currentUser.Id);
            if (tempResult.IsFailure)
                return tempResult.Cast<bool>();
            cuisine = ((SuccessResult<Cuisine>) tempResult).Value;
            await cuisineRepository.StoreAsync(cuisine, cancellationToken);
            cuisines.Add(cuisine);

            tempResult = cuisineFactory.Create("Indisch", currentUser.Id);
            if (tempResult.IsFailure)
                return tempResult.Cast<bool>();
            cuisine = ((SuccessResult<Cuisine>) tempResult).Value;
            await cuisineRepository.StoreAsync(cuisine, cancellationToken);
            cuisines.Add(cuisine);

            tempResult = cuisineFactory.Create("Mediterran", currentUser.Id);
            if (tempResult.IsFailure)
                return tempResult.Cast<bool>();
            cuisine = ((SuccessResult<Cuisine>) tempResult).Value;
            await cuisineRepository.StoreAsync(cuisine, cancellationToken);
            cuisines.Add(cuisine);

            tempResult = cuisineFactory.Create("Japnisch", currentUser.Id);
            if (tempResult.IsFailure)
                return tempResult.Cast<bool>();
            cuisine = ((SuccessResult<Cuisine>) tempResult).Value;
            await cuisineRepository.StoreAsync(cuisine, cancellationToken);
            cuisines.Add(cuisine);

            tempResult = cuisineFactory.Create("Regional", currentUser.Id);
            if (tempResult.IsFailure)
                return tempResult.Cast<bool>();
            cuisine = ((SuccessResult<Cuisine>) tempResult).Value;
            await cuisineRepository.StoreAsync(cuisine, cancellationToken);
            cuisines.Add(cuisine);

            tempResult = cuisineFactory.Create("Deutsch (gut bürgerlich)", currentUser.Id);
            if (tempResult.IsFailure)
                return tempResult.Cast<bool>();
            cuisine = ((SuccessResult<Cuisine>) tempResult).Value;
            await cuisineRepository.StoreAsync(cuisine, cancellationToken);
            cuisines.Add(cuisine);

            logger.LogInformation("test cuisines created");
            return SuccessResult<bool>.Create(true);
        }

        private async Task<Result<bool>> CreateRestaurantsAsync(User currentUser, int restCount, int dishCatCount, int dishCount, CancellationToken cancellationToken)
        {
            logger.LogInformation("creating test restaurants");

            for (var i = 0; i < restCount; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                var tempResult = await CreateRestaurantAsync(currentUser, i, dishCatCount, dishCount, cancellationToken);
                if (tempResult.IsFailure)
                    return tempResult;
            }

            logger.LogInformation("test restaurants created");
            return SuccessResult<bool>.Create(true);
        }

        private async Task<Result<bool>> CreateRestaurantAsync(User currentUser, int index, int dishCatCount, int dishCount, CancellationToken cancellationToken)
        {
            var restaurantName = $"Restaurant {(index + 1):D3}";
            var restAdminName = $"restadmin{(index % restAdminDict.Count + 1):D3}";

            logger.LogInformation("    creating test restaurant {0}", restaurantName);

            var restaurantResult = restaurantFactory.CreateWithName(restaurantName, currentUser.Id);
            if (restaurantResult.IsFailure)
                return restaurantResult.Cast<bool>();
            var restaurant = ((SuccessResult<Restaurant>) restaurantResult).Value;

            var boolResult =
                restaurant.ChangeAddress(new Address("Musterstraße 1", "12345", "Musterstadt"), currentUser.Id);
            if (boolResult.IsFailure)
                return boolResult;

            boolResult = restaurant.ChangeContactInfo(new ContactInfo(
                $"02871/1234-{(index + 1):D3}",
                $"02871/1234-5{(index + 1):D3}",
                $"http://www.restaurant{(index + 1):D3}.de",
                $"Verantw. Person {(index + 1):D3}",
                $"order@restaurant{(index + 1):D3}.de",
                "0171/1234-{(index + 1):D3}",
                true
            ), currentUser.Id);
            if (boolResult.IsFailure)
                return boolResult;

            for (var i = 0; i < 7; i++)
            {
                boolResult = restaurant.AddRegularOpeningPeriod(i,
                    new OpeningPeriod(TimeSpan.FromHours(10 + (index % 4) * 0.5),
                        TimeSpan.FromHours(20 + (index % 4) * 0.5)),
                    currentUser.Id);
                if (boolResult.IsFailure)
                    return boolResult;
            }

            boolResult = restaurant.ChangePickupInfo(new PickupInfo(
                true,
                15 + index / 100,
                5 + (decimal) index / 100,
                100 + (decimal) index / 100
            ), currentUser.Id);
            if (boolResult.IsFailure)
                return boolResult;

            if (index % 2 == 0)
            {
                boolResult = restaurant.ChangeDeliveryInfo(new DeliveryInfo(
                    true,
                    15 + index / 100,
                    5 + (decimal) index / 100,
                    100 + (decimal) index / 100,
                    3 + (decimal) index / 100
                ), currentUser.Id);
                if (boolResult.IsFailure)
                    return boolResult;
            }

            if (index % 4 == 0)
            {
                boolResult = restaurant.ChangeReservationInfo(new ReservationInfo(true, null), currentUser.Id);
                if (boolResult.IsFailure)
                    return boolResult;
            }

            restaurant.AddCuisine(cuisines[(index + 0) % cuisines.Count].Id, currentUser.Id);
            restaurant.AddCuisine(cuisines[(index + 1) % cuisines.Count].Id, currentUser.Id);

            restaurant.AddPaymentMethod(paymentMethods[(index + 0) % paymentMethods.Count].Id, currentUser.Id);
            restaurant.AddPaymentMethod(paymentMethods[(index + 1) % paymentMethods.Count].Id, currentUser.Id);

            var restAdmin = restAdminDict[restAdminName];
            restaurant.AddAdministrator(restAdmin.Id, currentUser.Id);

            SupportedOrderMode supportedOrderMode = SupportedOrderMode.OnlyPhone;
            if (index % 3 == 0)
            {
                supportedOrderMode = SupportedOrderMode.OnlyPhone;
            }
            else if (index % 3 == 1)
            {
                supportedOrderMode = SupportedOrderMode.AtNextShift;
            }
            else if (index % 3 == 2)
            {
                supportedOrderMode = SupportedOrderMode.Anytime;
            }

            boolResult = restaurant.ChangeSupportedOrderMode(supportedOrderMode, currentUser.Id);
            if (boolResult.IsFailure)
                return boolResult;

            boolResult = restaurant.Activate(currentUser.Id);
            if (boolResult.IsFailure)
                return boolResult;

            await restaurantRepository.StoreAsync(restaurant, cancellationToken);

            for (var catIndex = 0; catIndex < dishCatCount; catIndex++)
            {
                var dishCategoryName = $"Kategorie{(catIndex + 1):D2}";
                logger.LogInformation("        creating dish category {0}", dishCategoryName);

                var dishCategoryResult = dishCategoryFactory.Create(
                    restaurant.Id,
                    dishCategoryName,
                    catIndex,
                    currentUser.Id
                );

                if (dishCategoryResult.IsFailure)
                    return dishCategoryResult.Cast<bool>();

                var dishCategory = ((SuccessResult<DishCategory>) dishCategoryResult).Value;
                await dishCategoryRepository.StoreAsync(dishCategory, cancellationToken);

                for (var dishIndex = 0; dishIndex < dishCount; dishIndex++)
                {
                    var dishName = $"Gericht{(dishIndex + 1):D2}";

                    var variant = new DishVariant(Guid.NewGuid(), dishName, 5 + (decimal) dishIndex / 10);

                    var dishResult = dishFactory.Create(
                        restaurant.Id,
                        dishCategory.Id,
                        dishName,
                        $"Beschreibung des Gerichts{(dishIndex + 1):D2}",
                        $"Produktinformation des Gerichts{(dishIndex + 1):D2}",
                        dishIndex,
                        new[] {variant},
                        currentUser.Id
                    );
                    if (dishResult.IsFailure)
                        return dishResult.Cast<bool>();
                    var dish = ((SuccessResult<Dish>) dishResult).Value;
                    await dishRepository.StoreAsync(dish, cancellationToken);
                }

                logger.LogInformation("        dish category {0} created", dishCategoryName);
            }

            logger.LogInformation("    test restaurant {0} created", restaurantName);
            return SuccessResult<bool>.Create(true);
        }
    }
}
