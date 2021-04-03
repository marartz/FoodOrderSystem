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
using Gastromio.Core.Domain.Model.Orders;
using Gastromio.Core.Domain.Model.PaymentMethods;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Gastromio.Core.Application.Commands.Checkout
{
    public class CheckoutCommandHandler : ICommandHandler<CheckoutCommand, OrderDTO>
    {
        private readonly ILogger<CheckoutCommandHandler> logger;
        private readonly IRestaurantRepository restaurantRepository;
        private readonly IDishCategoryRepository dishCategoryRepository;
        private readonly IDishRepository dishRepository;
        private readonly IPaymentMethodRepository paymentMethodRepository;
        private readonly IOrderRepository orderRepository;

        public CheckoutCommandHandler(ILogger<CheckoutCommandHandler> logger,
            IRestaurantRepository restaurantRepository, IDishCategoryRepository dishCategoryRepository,
            IDishRepository dishRepository, IPaymentMethodRepository paymentMethodRepository,
            IOrderRepository orderRepository)
        {
            this.logger = logger;
            this.restaurantRepository = restaurantRepository;
            this.dishCategoryRepository = dishCategoryRepository;
            this.dishRepository = dishRepository;
            this.paymentMethodRepository = paymentMethodRepository;
            this.orderRepository = orderRepository;
        }

        public async Task<Result<OrderDTO>> HandleAsync(CheckoutCommand command, User currentUser,
            CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var newOrderId = new OrderId(Guid.NewGuid());

            var commandJson = JsonConvert.SerializeObject(command);

            logger.LogInformation($"Received order request {newOrderId.Value}: {commandJson}");

            Restaurant restaurant;

            if (Guid.TryParse(command.RestaurantId, out var restaurantId))
            {
                restaurant =
                    await restaurantRepository.FindByRestaurantIdAsync(new RestaurantId(restaurantId),
                        cancellationToken);
            }
            else
            {
                restaurant =
                    (await restaurantRepository.FindByRestaurantNameAsync(command.RestaurantId, cancellationToken))
                    .FirstOrDefault();
            }

            if (restaurant == null)
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: restaurant not found");
                return FailureResult<OrderDTO>.Create(FailureResultCode.OrderIsInvalid);
            }

            if (!restaurant.IsActive)
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: restaurant not active");
                return FailureResult<OrderDTO>.Create(FailureResultCode.OrderIsInvalid);
            }

            var restaurantInfo =
                $"{restaurant.Name} ({restaurant.Address?.Street}, {restaurant.Address?.ZipCode} {restaurant.Address?.City})";

            decimal totalPrice = 0;

            var dishCategoryDict = new Dictionary<DishCategoryId, DishCategory>();

            var dishDict = new Dictionary<Guid, Dish>();
            var variantDict = new Dictionary<Guid, DishVariant>();
            foreach (var cartDish in command.CartDishes)
            {
                var dish = await dishRepository.FindByDishIdAsync(cartDish.DishId, cancellationToken);
                if (dish == null)
                {
                    logger.LogInformation($"Declined order {newOrderId.Value}: dish not found");
                    return FailureResult<OrderDTO>.Create(FailureResultCode.OrderIsInvalid);
                }

                dishDict.Add(cartDish.ItemId, dish);
                if (dish.RestaurantId != restaurant.Id)
                {
                    logger.LogInformation($"Declined order {newOrderId.Value}: dish does not belong to restaurant");
                    return FailureResult<OrderDTO>.Create(FailureResultCode.OrderIsInvalid);
                }

                if (!dishCategoryDict.TryGetValue(dish.CategoryId, out var dishCategory))
                {
                    dishCategory =
                        await dishCategoryRepository.FindByDishCategoryIdAsync(dish.CategoryId, cancellationToken);
                    dishCategoryDict.Add(dish.CategoryId, dishCategory);
                }
                if (dishCategory == null)
                {
                    logger.LogInformation($"Declined order {newOrderId.Value}: dish category not found");
                    return FailureResult<OrderDTO>.Create(FailureResultCode.OrderIsInvalid);
                }
                if (!dishCategory.Enabled)
                {
                    logger.LogInformation($"Declined order {newOrderId.Value}: dish category is disabled");
                    return FailureResult<OrderDTO>.Create(FailureResultCode.OrderIsInvalid);
                }

                var variant = dish.Variants.FirstOrDefault(en => en.VariantId == cartDish.VariantId);
                if (variant == null)
                {
                    logger.LogInformation($"Declined order {newOrderId.Value}: variant does not belong to dish");
                    return FailureResult<OrderDTO>.Create(FailureResultCode.OrderIsInvalid);
                }

                variantDict.Add(cartDish.ItemId, variant);
                if (!string.IsNullOrWhiteSpace(cartDish.Remarks))
                {
                    var remarks = cartDish.Remarks.Trim();
                    if (remarks.Length > 1000)
                    {
                        logger.LogInformation($"Declined order {newOrderId.Value}: remark is longer than 1000 characters");
                        return FailureResult<OrderDTO>.Create(FailureResultCode.OrderIsInvalid);
                    }
                }

                if (cartDish.Count <= 0)
                {
                    logger.LogInformation($"Declined order {newOrderId.Value}: dish count is negative");
                    return FailureResult<OrderDTO>.Create(FailureResultCode.OrderIsInvalid);
                }

                if (cartDish.Count > 100)
                {
                    logger.LogInformation($"Declined order {newOrderId.Value}: dish count is greater than 100");
                    return FailureResult<OrderDTO>.Create(FailureResultCode.OrderIsInvalid);
                }

                totalPrice += cartDish.Count * variant.Price;
            }

            switch (command.OrderType)
            {
                case OrderType.Pickup:
                    if (!ValidateOrderTypePickup(command, newOrderId, restaurant, totalPrice))
                    {
                        logger.LogInformation($"Declined order {newOrderId.Value}: pickup order is not valid");
                        return FailureResult<OrderDTO>.Create(FailureResultCode.OrderIsInvalid);
                    }

                    break;
                case OrderType.Delivery:
                    if (!ValidateOrderTypeDelivery(command, newOrderId, restaurant, totalPrice))
                    {
                        logger.LogInformation($"Declined order {newOrderId.Value}: delivery order is not valid");
                        return FailureResult<OrderDTO>.Create(FailureResultCode.OrderIsInvalid);
                    }

                    break;
                case OrderType.Reservation:
                    if (!ValidateOrderTypeReservation(command, newOrderId, restaurant))
                    {
                        logger.LogInformation($"Declined order {newOrderId.Value}: reservation order is not valid");
                        return FailureResult<OrderDTO>.Create(FailureResultCode.OrderIsInvalid);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var serviceTime = command.ServiceTime ?? DateTimeOffset.Now;
            if (!restaurant.IsOrderPossibleAt(serviceTime))
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: order at this time is not possible");
                return FailureResult<OrderDTO>.Create(FailureResultCode.OrderIsInvalid);
            }

            PaymentMethod paymentMethod = null;
            if (command.OrderType == OrderType.Pickup || command.OrderType == OrderType.Delivery)
            {
                paymentMethod =
                    await paymentMethodRepository.FindByPaymentMethodIdAsync(command.PaymentMethodId,
                        cancellationToken);
                if (paymentMethod == null)
                {
                    logger.LogInformation($"Declined order {newOrderId.Value}: payment method is not known");
                    return FailureResult<OrderDTO>.Create(FailureResultCode.OrderIsInvalid);
                }
            }

            decimal costs = 0;
            if (command.OrderType == OrderType.Delivery && restaurant.DeliveryInfo.Costs.HasValue)
            {
                costs = restaurant.DeliveryInfo.Costs.Value;
            }

            totalPrice += costs;

            var order = new Order(
                newOrderId,
                new CustomerInfo(
                    command.GivenName,
                    command.LastName,
                    command.Street,
                    command.AddAddressInfo,
                    command.ZipCode,
                    command.City,
                    command.Phone,
                    command.Email
                ),
                new CartInfo(
                    command.OrderType,
                    restaurant.Id,
                    restaurant.Name,
                    restaurantInfo,
                    restaurant.ContactInfo.Phone,
                    restaurant.ContactInfo.EmailAddress,
                    restaurant.ContactInfo.Mobile,
                    restaurant.NeedsSupport,
                    command.CartDishes?.Select(en => new OrderedDishInfo(
                        en.ItemId,
                        en.DishId,
                        $"{dishDict[en.ItemId].Name} ({dishDict[en.ItemId].Description})",
                        en.VariantId,
                        dishDict[en.ItemId].Variants.Count > 1 ? variantDict[en.ItemId].Name : "",
                        variantDict[en.ItemId].Price,
                        en.Count,
                        en.Remarks
                    )).ToList()
                ),
                command.Comments,
                command.PaymentMethodId,
                paymentMethod?.Name,
                paymentMethod?.Description,
                costs,
                totalPrice,
                command.ServiceTime,
                null,
                null,
                null,
                DateTimeOffset.UtcNow,
                null,
                null
            );

            var shouldInformByMobile = restaurant.ContactInfo?.OrderNotificationByMobile ?? false;
            if (!shouldInformByMobile)
            {
                order.RegisterRestaurantMobileNotificationAttempt(true, "mobile notification not requested");
            }

            await orderRepository.StoreAsync(order, cancellationToken);

            var viewModel = new OrderDTO(order, paymentMethod);
            logger.LogInformation($"Order is processed successfully: {newOrderId.Value}");
            return SuccessResult<OrderDTO>.Create(viewModel);
        }

        private bool ValidateOrderTypePickup(CheckoutCommand command, OrderId newOrderId, Restaurant restaurant, decimal totalPrice)
        {
            if (restaurant.PickupInfo == null || !restaurant.PickupInfo.Enabled)
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: pickup is not enabled");
                return false;
            }

            if (command.OrderType == OrderType.Pickup && restaurant.PickupInfo.MinimumOrderValue.HasValue &&
                totalPrice < restaurant.PickupInfo.MinimumOrderValue.Value)
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: minimum order value for pickup not met");
                return false;
            }

            if (command.OrderType == OrderType.Pickup && restaurant.PickupInfo.MaximumOrderValue.HasValue &&
                totalPrice > restaurant.PickupInfo.MaximumOrderValue.Value)
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: maximum order value for pickup exceeded");
                return false;
            }

            if (string.IsNullOrWhiteSpace(command.GivenName))
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: given name is empty");
                return false;
            }

            if (string.IsNullOrWhiteSpace(command.LastName))
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: last name is empty");
                return false;
            }

            if (string.IsNullOrWhiteSpace(command.Email))
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: email is empty");
                return false;
            }

            if (string.IsNullOrWhiteSpace(command.Phone))
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: phone is empty");
                return false;
            }

            return true;
        }

        private bool ValidateOrderTypeDelivery(CheckoutCommand command, OrderId newOrderId, Restaurant restaurant, decimal totalPrice)
        {
            if (restaurant.DeliveryInfo == null || !restaurant.DeliveryInfo.Enabled)
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: delivery is not enabled");
                return false;
            }

            if (command.OrderType == OrderType.Delivery && restaurant.DeliveryInfo.MinimumOrderValue.HasValue &&
                totalPrice < restaurant.DeliveryInfo.MinimumOrderValue.Value)
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: minimum order value for delivery is not met");
                return false;
            }

            if (command.OrderType == OrderType.Delivery && restaurant.DeliveryInfo.MaximumOrderValue.HasValue &&
                totalPrice > restaurant.DeliveryInfo.MaximumOrderValue.Value)
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: maximum order value for delivery exceeded");
                return false;
            }

            if (string.IsNullOrWhiteSpace(command.GivenName))
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: given name is empty");
                return false;
            }

            if (string.IsNullOrWhiteSpace(command.LastName))
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: last name is empty");
                return false;
            }

            if (string.IsNullOrWhiteSpace(command.Street))
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: street is empty");
                return false;
            }

            if (string.IsNullOrWhiteSpace(command.ZipCode))
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: zip code is empty");
                return false;
            }

            if (string.IsNullOrWhiteSpace(command.City))
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: city is empty");
                return false;
            }

            if (string.IsNullOrWhiteSpace(command.Email))
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: email is empty");
                return false;
            }

            if (string.IsNullOrWhiteSpace(command.Phone))
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: phone is empty");
                return false;
            }

            return true;
        }

        private bool ValidateOrderTypeReservation(CheckoutCommand command, OrderId newOrderId, Restaurant restaurant)
        {
            if (restaurant.ReservationInfo == null || !restaurant.ReservationInfo.Enabled)
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: reservation is not enabled");
                return false;
            }

            if (string.IsNullOrWhiteSpace(command.GivenName))
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: given name is empty");
                return false;
            }

            if (string.IsNullOrWhiteSpace(command.LastName))
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: last name is empty");
                return false;
            }

            if (string.IsNullOrWhiteSpace(command.Street))
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: street is empty");
                return false;
            }

            if (string.IsNullOrWhiteSpace(command.ZipCode))
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: zip code is empty");
                return false;
            }

            if (string.IsNullOrWhiteSpace(command.City))
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: city is empty");
                return false;
            }

            if (string.IsNullOrWhiteSpace(command.Email))
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: email is empty");
                return false;
            }

            if (string.IsNullOrWhiteSpace(command.Phone))
            {
                logger.LogInformation($"Declined order {newOrderId.Value}: phone is empty");
                return false;
            }

            return true;
        }
    }
}
