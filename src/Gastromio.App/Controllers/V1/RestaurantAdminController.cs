using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Gastromio.App.Helper;
using Gastromio.App.Models;
using Gastromio.Core.Application.Commands;
using Gastromio.Core.Application.Commands.AddDeviatingOpeningDayToRestaurant;
using Gastromio.Core.Application.Commands.AddDeviatingOpeningPeriodToRestaurant;
using Gastromio.Core.Application.Commands.AddDishCategoryToRestaurant;
using Gastromio.Core.Application.Commands.AddOrChangeDishOfRestaurant;
using Gastromio.Core.Application.Commands.AddOrChangeExternalMenuOfRestaurant;
using Gastromio.Core.Application.Commands.AddPaymentMethodToRestaurant;
using Gastromio.Core.Application.Commands.AddRegularOpeningPeriodToRestaurant;
using Gastromio.Core.Application.Commands.ChangeDeviatingOpeningDayStatusOfRestaurant;
using Gastromio.Core.Application.Commands.ChangeDeviatingOpeningPeriodOfRestaurant;
using Gastromio.Core.Application.Commands.ChangeDishCategoryOfRestaurant;
using Gastromio.Core.Application.Commands.ChangeRegularOpeningPeriodOfRestaurant;
using Gastromio.Core.Application.Commands.ChangeRestaurantAddress;
using Gastromio.Core.Application.Commands.ChangeRestaurantContactInfo;
using Gastromio.Core.Application.Commands.ChangeRestaurantImage;
using Gastromio.Core.Application.Commands.ChangeRestaurantServiceInfo;
using Gastromio.Core.Application.Commands.ChangeSupportedOrderModeOfRestaurant;
using Gastromio.Core.Application.Commands.DecOrderOfDish;
using Gastromio.Core.Application.Commands.DecOrderOfDishCategory;
using Gastromio.Core.Application.Commands.DisableDishCategory;
using Gastromio.Core.Application.Commands.EnableDishCategory;
using Gastromio.Core.Application.Commands.IncOrderOfDish;
using Gastromio.Core.Application.Commands.IncOrderOfDishCategory;
using Gastromio.Core.Application.Commands.RemoveDeviatingOpeningDayFromRestaurant;
using Gastromio.Core.Application.Commands.RemoveDeviatingOpeningPeriodFromRestaurant;
using Gastromio.Core.Application.Commands.RemoveDishCategoryFromRestaurant;
using Gastromio.Core.Application.Commands.RemoveDishFromRestaurant;
using Gastromio.Core.Application.Commands.RemoveExternalMenuFromRestaurant;
using Gastromio.Core.Application.Commands.RemovePaymentMethodFromRestaurant;
using Gastromio.Core.Application.Commands.RemoveRegularOpeningPeriodFromRestaurant;
using Gastromio.Core.Application.DTOs;
using Gastromio.Core.Application.Queries;
using Gastromio.Core.Application.Queries.GetAllCuisines;
using Gastromio.Core.Application.Queries.GetAllPaymentMethods;
using Gastromio.Core.Application.Queries.GetDishesOfRestaurantForAdmin;
using Gastromio.Core.Application.Queries.GetRestaurantById;
using Gastromio.Core.Application.Queries.RestAdminMyRestaurants;
using Gastromio.Core.Application.Services;
using Gastromio.Core.Domain.Model.DishCategories;
using Gastromio.Core.Domain.Model.Dishes;
using Gastromio.Core.Domain.Model.PaymentMethods;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gastromio.App.Controllers.V1
{
    [Route("api/v1/restaurantadmin")]
    [ApiController]
    [Authorize()]
    public class RestaurantAdminController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly IQueryDispatcher queryDispatcher;
        private readonly IFailureMessageService failureMessageService;

        public RestaurantAdminController(ILogger<RestaurantAdminController> logger,
            ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher,
            IFailureMessageService failureMessageService)
        {
            this.logger = logger;
            this.commandDispatcher = commandDispatcher;
            this.queryDispatcher = queryDispatcher;
            this.failureMessageService = failureMessageService;
        }

        [Route("myrestaurants")]
        [HttpGet]
        public async Task<IActionResult> GetMyRestaurantsAsync()
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var queryResult =
                await queryDispatcher.PostAsync<RestAdminMyRestaurantsQuery, ICollection<RestaurantDTO>>(
                    new RestAdminMyRestaurantsQuery(), new UserId(currentUserId));
            return ResultHelper.HandleResult(queryResult, failureMessageService);
        }

        [Route("restaurants/{restaurant}")]
        [HttpGet]
        public async Task<IActionResult> GetRestaurantAsync(string restaurant)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var queryResult =
                await queryDispatcher.PostAsync<GetRestaurantByIdQuery, RestaurantDTO>(
                    new GetRestaurantByIdQuery(restaurant, false), new UserId(currentUserId));

            return ResultHelper.HandleResult(queryResult, failureMessageService);
        }

        [Route("restaurants/{restaurant}/dishes")]
        [HttpGet]
        public async Task<IActionResult> GetDishesOfRestaurantAsync(string restaurant)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var queryResult =
                await queryDispatcher.PostAsync<GetDishesOfRestaurantForAdminQuery, ICollection<DishCategoryDTO>>(
                    new GetDishesOfRestaurantForAdminQuery(restaurant), new UserId(currentUserId));
            return ResultHelper.HandleResult(queryResult, failureMessageService);
        }

        [Route("cuisines")]
        [HttpGet]
        public async Task<IActionResult> GetCuisinesAsync()
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var queryResult =
                await queryDispatcher.PostAsync<GetAllCuisinesQuery, ICollection<CuisineDTO>>(new GetAllCuisinesQuery(),
                    new UserId(currentUserId));
            return ResultHelper.HandleResult(queryResult, failureMessageService);
        }

        [Route("paymentmethods")]
        [HttpGet]
        public async Task<IActionResult> GetPaymentMethodsAsync()
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var queryResult =
                await queryDispatcher.PostAsync<GetAllPaymentMethodsQuery, ICollection<PaymentMethodDTO>>(
                    new GetAllPaymentMethodsQuery(), new UserId(currentUserId));
            return ResultHelper.HandleResult(queryResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/changeimage")]
        [HttpPost]
        public async Task<IActionResult> PostChangeImageAsync(Guid restaurantId,
            [FromBody] ChangeRestaurantImageModel changeRestaurantImageModel)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var image = string.IsNullOrWhiteSpace(changeRestaurantImageModel.Image)
                ? null
                : ImageHelper.ConvertFromImageUrl(changeRestaurantImageModel.Image);

            var command = new ChangeRestaurantImageCommand(new RestaurantId(restaurantId),
                changeRestaurantImageModel.Type, image);

            var commandResult =
                await commandDispatcher.PostAsync<ChangeRestaurantImageCommand, bool>(command,
                    new UserId(currentUserId));

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/changeaddress")]
        [HttpPost]
        public async Task<IActionResult> PostChangeAddressAsync(Guid restaurantId,
            [FromBody] ChangeRestaurantAddressModel changeRestaurantAddressModel)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var command = new ChangeRestaurantAddressCommand(
                new RestaurantId(restaurantId),
                changeRestaurantAddressModel.Street,
                changeRestaurantAddressModel.ZipCode,
                changeRestaurantAddressModel.City
            );

            var commandResult =
                await commandDispatcher.PostAsync<ChangeRestaurantAddressCommand, bool>(command,
                    new UserId(currentUserId));
            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/changecontactinfo")]
        [HttpPost]
        public async Task<IActionResult> PostChangeContactInfoAsync(Guid restaurantId,
            [FromBody] ChangeRestaurantContactInfoModel changeRestaurantContactInfoModel)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var command = new ChangeRestaurantContactInfoCommand(
                new RestaurantId(restaurantId),
                changeRestaurantContactInfoModel.Phone,
                changeRestaurantContactInfoModel.Fax,
                changeRestaurantContactInfoModel.WebSite,
                changeRestaurantContactInfoModel.ResponsiblePerson,
                changeRestaurantContactInfoModel.EmailAddress,
                changeRestaurantContactInfoModel.Mobile,
                changeRestaurantContactInfoModel.OrderNotificationByMobile
            );

            var commandResult =
                await commandDispatcher.PostAsync<ChangeRestaurantContactInfoCommand, bool>(command,
                    new UserId(currentUserId));

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/changeserviceinfo")]
        [HttpPost]
        public async Task<IActionResult> PostChangePickupInfoAsync(Guid restaurantId,
            [FromBody] ChangeRestaurantServiceInfoModel changeRestaurantServiceInfoModel)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<ChangeRestaurantServiceInfoCommand, bool>(
                new ChangeRestaurantServiceInfoCommand(new RestaurantId(restaurantId),
                    new PickupInfo(
                        changeRestaurantServiceInfoModel.PickupEnabled,
                        changeRestaurantServiceInfoModel.PickupAverageTime,
                        changeRestaurantServiceInfoModel.PickupMinimumOrderValue,
                        changeRestaurantServiceInfoModel.PickupMaximumOrderValue
                    ),
                    new DeliveryInfo(
                        changeRestaurantServiceInfoModel.DeliveryEnabled,
                        changeRestaurantServiceInfoModel.DeliveryAverageTime,
                        changeRestaurantServiceInfoModel.DeliveryMinimumOrderValue,
                        changeRestaurantServiceInfoModel.DeliveryMaximumOrderValue,
                        changeRestaurantServiceInfoModel.DeliveryCosts
                    ),
                    new ReservationInfo(
                        changeRestaurantServiceInfoModel.ReservationEnabled,
                        changeRestaurantServiceInfoModel.ReservationSystemUrl
                    ),
                    changeRestaurantServiceInfoModel.HygienicHandling
                ),
                new UserId(currentUserId)
            );

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/addregularopeningperiod")]
        [HttpPost]
        public async Task<IActionResult> PostAddRegularOpeningPeriodAsync(Guid restaurantId,
            [FromBody] AddRegularOpeningPeriodToRestaurantModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var start = TimeSpan.FromMinutes(model.Start);
            var end = TimeSpan.FromMinutes(model.End);

            var commandResult = await commandDispatcher.PostAsync<AddRegularOpeningPeriodToRestaurantCommand, bool>(
                new AddRegularOpeningPeriodToRestaurantCommand(new RestaurantId(restaurantId), model.DayOfWeek, start, end),
                new UserId(currentUserId));

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/changeregularopeningperiod")]
        [HttpPost]
        public async Task<IActionResult> PostChangeRegularOpeningPeriodAsync(Guid restaurantId,
            [FromBody] ChangeRegularOpeningPeriodOfRestaurantModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var oldStart = TimeSpan.FromMinutes(model.OldStart);
            var newStart = TimeSpan.FromMinutes(model.NewStart);
            var newEnd = TimeSpan.FromMinutes(model.NewEnd);

            var commandResult = await commandDispatcher.PostAsync<ChangeRegularOpeningPeriodOfRestaurantCommand, bool>(
                new ChangeRegularOpeningPeriodOfRestaurantCommand(new RestaurantId(restaurantId), model.DayOfWeek, oldStart, newStart, newEnd),
                new UserId(currentUserId));

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/removeregularopeningperiod")]
        [HttpPost]
        public async Task<IActionResult> PostRemoveRegularOpeningPeriodAsync(Guid restaurantId,
            [FromBody] RemoveRegularOpeningPeriodFromRestaurantModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var start = TimeSpan.FromMinutes(model.Start);

            var commandResult = await commandDispatcher.PostAsync<RemoveRegularOpeningPeriodFromRestaurantCommand, bool>(
                new RemoveRegularOpeningPeriodFromRestaurantCommand(new RestaurantId(restaurantId), model.DayOfWeek, start),
                new UserId(currentUserId)
            );

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/adddeviatingopeningday")]
        [HttpPost]
        public async Task<IActionResult> PostAddDeviatingOpeningDayAsync(Guid restaurantId,
            [FromBody] AddDeviatingOpeningDayToRestaurantModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<AddDeviatingOpeningDayToRestaurantCommand, bool>(
                new AddDeviatingOpeningDayToRestaurantCommand(new RestaurantId(restaurantId), model.Date.ToDomain(), model.Status.ToDeviatingOpeningDayStatus()),
                new UserId(currentUserId));

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/changedeviatingopeningdaystatus")]
        [HttpPost]
        public async Task<IActionResult> PostChangeDeviatingOpeningDayStatusAsync(Guid restaurantId,
            [FromBody] ChangeDeviatingOpeningDayStatusOfRestaurantModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<ChangeDeviatingOpeningDayStatusOfRestaurantCommand, bool>(
                new ChangeDeviatingOpeningDayStatusOfRestaurantCommand(new RestaurantId(restaurantId), model.Date.ToDomain(), model.Status.ToDeviatingOpeningDayStatus()),
                new UserId(currentUserId));

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/removedeviatingopeningday")]
        [HttpPost]
        public async Task<IActionResult> PostRemoveDeviatingOpeningDayAsync(Guid restaurantId,
            [FromBody] RemoveDeviatingOpeningDayFromRestaurantModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<RemoveDeviatingOpeningDayFromRestaurantCommand, bool>(
                new RemoveDeviatingOpeningDayFromRestaurantCommand(new RestaurantId(restaurantId), model.Date.ToDomain()),
                new UserId(currentUserId));

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/adddeviatingopeningperiod")]
        [HttpPost]
        public async Task<IActionResult> PostAddDeviatingOpeningPeriodAsync(Guid restaurantId,
            [FromBody] AddDeviatingOpeningPeriodToRestaurantModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var start = TimeSpan.FromMinutes(model.Start);
            var end = TimeSpan.FromMinutes(model.End);

            var commandResult = await commandDispatcher.PostAsync<AddDeviatingOpeningPeriodToRestaurantCommand, bool>(
                new AddDeviatingOpeningPeriodToRestaurantCommand(new RestaurantId(restaurantId), model.Date.ToDomain(), start, end),
                new UserId(currentUserId));

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/changedeviatingopeningperiod")]
        [HttpPost]
        public async Task<IActionResult> PostChangeDeviatingOpeningPeriodAsync(Guid restaurantId,
            [FromBody] ChangeDeviatingOpeningPeriodOfRestaurantModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var oldStart = TimeSpan.FromMinutes(model.OldStart);
            var newStart = TimeSpan.FromMinutes(model.NewStart);
            var newEnd = TimeSpan.FromMinutes(model.NewEnd);

            var commandResult = await commandDispatcher.PostAsync<ChangeDeviatingOpeningPeriodOfRestaurantCommand, bool>(
                new ChangeDeviatingOpeningPeriodOfRestaurantCommand(new RestaurantId(restaurantId), model.Date.ToDomain(), oldStart, newStart, newEnd),
                new UserId(currentUserId));

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/removedeviatingopeningperiod")]
        [HttpPost]
        public async Task<IActionResult> PostRemoveDeviatingOpeningPeriodAsync(Guid restaurantId,
            [FromBody] RemoveDeviatingOpeningPeriodFromRestaurantModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var start = TimeSpan.FromMinutes(model.Start);

            var commandResult = await commandDispatcher.PostAsync<RemoveDeviatingOpeningPeriodFromRestaurantCommand, bool>(
                new RemoveDeviatingOpeningPeriodFromRestaurantCommand(new RestaurantId(restaurantId), model.Date.ToDomain(), start),
                new UserId(currentUserId)
            );

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/addpaymentmethod")]
        [HttpPost]
        public async Task<IActionResult> PostAddPaymentMethodAsync(Guid restaurantId,
            [FromBody] AddPaymentMethodToRestaurantModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<AddPaymentMethodToRestaurantCommand, bool>(
                new AddPaymentMethodToRestaurantCommand(new RestaurantId(restaurantId),
                    new PaymentMethodId(model.PaymentMethodId)),
                new UserId(currentUserId)
            );

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/removepaymentmethod")]
        [HttpPost]
        public async Task<IActionResult> PostRemovePaymentMethodAsync(Guid restaurantId,
            [FromBody] RemovePaymentMethodFromRestaurantModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<RemovePaymentMethodFromRestaurantCommand, bool>(
                new RemovePaymentMethodFromRestaurantCommand(new RestaurantId(restaurantId),
                    new PaymentMethodId(model.PaymentMethodId)),
                new UserId(currentUserId)
            );

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/adddishcategory")]
        [HttpPost]
        public async Task<IActionResult> PostAddDishCategoryAsync(Guid restaurantId,
            [FromBody] AddDishCategoryToRestaurantModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<AddDishCategoryToRestaurantCommand, Guid>(
                new AddDishCategoryToRestaurantCommand(new RestaurantId(restaurantId), model.Name,
                    model.AfterCategoryId.HasValue ? new DishCategoryId(model.AfterCategoryId.Value) : null),
                new UserId(currentUserId)
            );

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/changedishcategory")]
        [HttpPost]
        public async Task<IActionResult> PostChangeDishCategoryAsync(Guid restaurantId,
            [FromBody] ChangeDishCategoryOfRestaurantModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<ChangeDishCategoryOfRestaurantCommand, bool>(
                new ChangeDishCategoryOfRestaurantCommand(new RestaurantId(restaurantId),
                    new DishCategoryId(model.DishCategoryId), model.Name),
                new UserId(currentUserId)
            );

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/incorderofdishcategory")]
        [HttpPost]
        public async Task<IActionResult> PostIncOrderOfDishCategoryAsync(Guid restaurantId,
            [FromBody] IncOrderOfDishCategoryModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<IncOrderOfDishCategoryCommand, bool>(
                new IncOrderOfDishCategoryCommand(new DishCategoryId(model.DishCategoryId)),
                new UserId(currentUserId)
            );

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/decorderofdishcategory")]
        [HttpPost]
        public async Task<IActionResult> PostDecOrderOfDishCategoryAsync(Guid restaurantId,
            [FromBody] DecOrderOfDishCategoryModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<DecOrderOfDishCategoryCommand, bool>(
                new DecOrderOfDishCategoryCommand(new DishCategoryId(model.DishCategoryId)),
                new UserId(currentUserId)
            );

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/enabledishcategory")]
        [HttpPost]
        public async Task<IActionResult> PostEnableDishCategoryAsync(Guid restaurantId,
            [FromBody] EnableDishCategoryModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<EnableDishCategoryCommand, bool>(
                new EnableDishCategoryCommand(new DishCategoryId(model.DishCategoryId)),
                new UserId(currentUserId)
            );

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/disabledishcategory")]
        [HttpPost]
        public async Task<IActionResult> PostDisableDishCategoryAsync(Guid restaurantId,
            [FromBody] DisableDishCategoryModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<DisableDishCategoryCommand, bool>(
                new DisableDishCategoryCommand(new DishCategoryId(model.DishCategoryId)),
                new UserId(currentUserId)
            );

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/removedishcategory")]
        [HttpPost]
        public async Task<IActionResult> PostRemoveDishCategoryAsync(Guid restaurantId,
            [FromBody] RemoveDishCategoryFromRestaurantModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<RemoveDishCategoryFromRestaurantCommand, bool>(
                new RemoveDishCategoryFromRestaurantCommand(new RestaurantId(restaurantId),
                    new DishCategoryId(model.DishCategoryId)),
                new UserId(currentUserId)
            );

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/addoreditdish")]
        [HttpPost]
        public async Task<IActionResult> PostAddOrEditDishAsync(Guid restaurantId,
            [FromBody] AddOrChangeDishOfRestaurantModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<AddOrChangeDishOfRestaurantCommand, Guid>(
                new AddOrChangeDishOfRestaurantCommand(
                    new RestaurantId(restaurantId),
                    new DishCategoryId(model.DishCategoryId),
                    model.Dish.Id,
                    model.Dish.Name,
                    model.Dish.Description,
                    model.Dish.ProductInfo,
                    model.Dish.OrderNo,
                    model.Dish.Variants?.Select(
                        variant => new DishVariant(
                            variant.VariantId,
                            variant.Name,
                            variant.Price
                        )
                    )
                ),
                new UserId(currentUserId)
            );

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/incorderofdish")]
        [HttpPost]
        public async Task<IActionResult> PostIncOrderOfDishAsync(Guid restaurantId,
            [FromBody] IncOrderOfDishModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<IncOrderOfDishCommand, bool>(
                new IncOrderOfDishCommand(new DishId(model.DishId)),
                new UserId(currentUserId)
            );

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/decorderofdish")]
        [HttpPost]
        public async Task<IActionResult> PostDecOrderOfDishAsync(Guid restaurantId,
            [FromBody] DecOrderOfDishModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<DecOrderOfDishCommand, bool>(
                new DecOrderOfDishCommand(new DishId(model.DishId)),
                new UserId(currentUserId)
            );

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/removedish")]
        [HttpPost]
        public async Task<IActionResult> PostRemoveDishAsync(Guid restaurantId,
            [FromBody] RemoveDishFromRestaurantModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<RemoveDishFromRestaurantCommand, bool>(
                new RemoveDishFromRestaurantCommand(new RestaurantId(restaurantId),
                    new DishCategoryId(model.DishCategoryId), new DishId(model.DishId)),
                new UserId(currentUserId)
            );

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/changesupportedordermode")]
        [HttpPost]
        public async Task<IActionResult> PostChangeSupportedOrderModeAsync(Guid restaurantId,
            [FromBody] ChangeSupportedOrderModeOfRestaurantModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var supportedOrderMode = model.SupportedOrderMode.ToSupportedOrderMode();

            var commandResult = await commandDispatcher.PostAsync<ChangeSupportedOrderModeOfRestaurantCommand, bool>(
                new ChangeSupportedOrderModeOfRestaurantCommand(new RestaurantId(restaurantId), supportedOrderMode),
                new UserId(currentUserId));

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/addorchangeexternalmenu")]
        [HttpPost]
        public async Task<IActionResult> PostAddOrChangeExternalMenuAsync(Guid restaurantId,
            [FromBody] AddOrChangeExternalMenuOfRestaurantModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var externalMenu = new ExternalMenu(
                model.ExternalMenuId,
                model.Name,
                model.Description,
                model.Url
            );

            var commandResult = await commandDispatcher.PostAsync<AddOrChangeExternalMenuOfRestaurantCommand, bool>(
                new AddOrChangeExternalMenuOfRestaurantCommand(new RestaurantId(restaurantId), externalMenu),
                new UserId(currentUserId));

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/removeexternalmenu")]
        [HttpPost]
        public async Task<IActionResult> PostRemoveExternalMenuAsync(Guid restaurantId,
            [FromBody] RemoveExternalMenuFromRestaurantModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<RemoveExternalMenuFromRestaurantCommand, bool>(
                new RemoveExternalMenuFromRestaurantCommand(new RestaurantId(restaurantId), model.ExternalMenuId),
                new UserId(currentUserId));

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }
    }
}
