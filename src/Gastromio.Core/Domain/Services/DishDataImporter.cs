using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Application.Services;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.DishCategories;
using Gastromio.Core.Domain.Model.Dishes;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Domain.Services
{
    public class DishDataImporter : IDishDataImporter
    {
        private readonly IFailureMessageService failureMessageService;
        private readonly IRestaurantRepository restaurantRepository;
        private readonly IDishCategoryFactory dishCategoryFactory;
        private readonly IDishCategoryRepository dishCategoryRepository;
        private readonly IDishFactory dishFactory;
        private readonly IDishRepository dishRepository;

        private readonly Dictionary<string, Restaurant> recentRestaurants = new Dictionary<string, Restaurant>();

        private readonly Dictionary<string, Dictionary<string, DishCategory>> recentDishCategories =
            new Dictionary<string, Dictionary<string, DishCategory>>();

        private readonly Dictionary<string, Dictionary<string, Dish>> recentDishes =
            new Dictionary<string, Dictionary<string, Dish>>();

        public DishDataImporter(
            IFailureMessageService failureMessageService,
            IRestaurantRepository restaurantRepository,
            IDishCategoryFactory dishCategoryFactory,
            IDishCategoryRepository dishCategoryRepository,
            IDishFactory dishFactory,
            IDishRepository dishRepository
        )
        {
            this.failureMessageService = failureMessageService;
            this.restaurantRepository = restaurantRepository;
            this.dishCategoryFactory = dishCategoryFactory;
            this.dishCategoryRepository = dishCategoryRepository;
            this.dishFactory = dishFactory;
            this.dishRepository = dishRepository;
        }

        public async Task ImportDishAsync(ImportLog log, int rowIndex, DishRow dishRow,
            UserId curUserId, bool dryRun, CancellationToken cancellationToken = default)
        {
            if (!recentRestaurants.TryGetValue(dishRow.RestaurantImportId, out var restaurant))
            {
                restaurant = await restaurantRepository.FindByImportIdAsync(dishRow.RestaurantImportId, cancellationToken);
                if (restaurant == null)
                {
                    log.AddLine(ImportLogLineType.Warning, rowIndex,
                        "Restaurant mit Import-Id '{0}' nicht gefunden => Überspringe Datensatz",
                        dishRow.RestaurantImportId);
                    return;
                }
                recentRestaurants.Add(dishRow.RestaurantImportId, restaurant);

                log.AddLine(ImportLogLineType.Information, rowIndex, "Lösche alle Speisen von Restaurant '{0}', wenn vorhanden", restaurant.Name);
                if (!dryRun)
                {
                    await dishRepository.RemoveByRestaurantIdAsync(restaurant.Id, cancellationToken);
                }
                
                log.AddLine(ImportLogLineType.Information, rowIndex, "Lösche alle Kategorien von Restaurant '{0}', wenn vorhanden", restaurant.Name);
                if (!dryRun)
                {
                    await dishCategoryRepository.RemoveByRestaurantIdAsync(restaurant.Id, cancellationToken);
                }
            }

            if (!recentDishCategories.TryGetValue(dishRow.RestaurantImportId, out var dishCategoryDict))
            {
                dishCategoryDict = new Dictionary<string, DishCategory>();
                recentDishCategories.Add(dishRow.RestaurantImportId, dishCategoryDict);
            }

            if (!dishCategoryDict.TryGetValue(dishRow.Category, out var category))
            {
                var orderNo = dishCategoryDict.Count == 0 ? 0 : dishCategoryDict.Max(en => en.Value.OrderNo) + 1;

                var dishCategoryResult = dishCategoryFactory.Create(
                    restaurant.Id,
                    dishRow.Category,
                    orderNo,
                    curUserId
                );

                if (dishCategoryResult.IsFailure)
                {
                    AddFailureMessageToLog(log, rowIndex, dishCategoryResult);
                    return;
                }
                category = dishCategoryResult.Value;

                log.AddLine(ImportLogLineType.Information, rowIndex,
                    "Lege für Restaurant '{0}' eine neue Kategorie mit Namen '{1}' und Sortierung '{2}' an",
                    restaurant.Name, dishRow.Category, orderNo);

                if (!dryRun)
                    await dishCategoryRepository.StoreAsync(category, cancellationToken);

                dishCategoryDict.Add(dishRow.Category, category);
            }

            if (!recentDishes.TryGetValue(dishRow.RestaurantImportId, out var dishDict))
            {
                dishDict = new Dictionary<string, Dish>();
                recentDishes.Add(dishRow.RestaurantImportId, dishDict);
            }

            var dishKey = $"{dishRow.Category}||{dishRow.DishName}||{dishRow.Description ?? ""}";

            if (!dishDict.TryGetValue(dishKey, out var dish))
            {
                var orderNo = dishDict.Count == 0 ? 0 : dishDict.Max(en => en.Value.OrderNo) + 1;

                var variant = new DishVariant(Guid.NewGuid(),
                    string.IsNullOrWhiteSpace(dishRow.VariantName) ? string.Empty : dishRow.VariantName,
                    dishRow.VariantPrice.HasValue ? (decimal) dishRow.VariantPrice.Value : 0);

                var dishResult = dishFactory.Create(
                    restaurant.Id,
                    category.Id,
                    dishRow.DishName,
                    dishRow.Description,
                    dishRow.ProductInfo,
                    orderNo, 
                    new[] {variant},
                    curUserId
                );
                
                if (dishResult.IsFailure)
                {
                    AddFailureMessageToLog(log, rowIndex, dishResult);
                    return;
                }
                    
                dish = dishResult.Value;

                log.AddLine(ImportLogLineType.Information, rowIndex,
                    "Lege für Restaurant '{0}' eine neue Speise mit Namen '{1}', Beschreibung '{2}' und Sortierung '{3}' an",
                    restaurant.Name, dishRow.DishName, dishRow.Description, orderNo);
                
                if (!dryRun)
                    await dishRepository.StoreAsync(dish, cancellationToken);

                dishDict.Add(dishKey, dish);
            }
            else
            {
                var addVariantResult = dish.AddVariant(
                    Guid.NewGuid(),
                    dishRow.VariantName,
                    dishRow.VariantPrice.HasValue ? (decimal)dishRow.VariantPrice.Value : 0,
                    curUserId
                );
                
                if (addVariantResult.IsFailure)
                {
                    AddFailureMessageToLog(log, rowIndex, addVariantResult);
                    return;
                }

                log.AddLine(ImportLogLineType.Information, rowIndex,
                    "Lege für Restaurant '{0}' und Speise '{1}' eine neue Variante '{2}' an",
                    restaurant.Name, dishRow.DishName, dishRow.VariantName);

                if (!dryRun)
                    await dishRepository.StoreAsync(dish, cancellationToken);
            }
        }

        private void AddFailureMessageToLog<T>(ImportLog log, int rowIndex, Result<T> result)
        {
            var message = failureMessageService.GetTranslatedMessage((FailureResult<T>) result);
            log.AddLine(ImportLogLineType.Error, rowIndex, message);
        }
    }
}
