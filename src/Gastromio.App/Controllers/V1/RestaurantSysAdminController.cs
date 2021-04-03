using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Gastromio.App.Helper;
using Gastromio.App.Models;
using Gastromio.Core.Application.Commands;
using Gastromio.Core.Application.Commands.ActivateRestaurant;
using Gastromio.Core.Application.Commands.AddAdminToRestaurant;
using Gastromio.Core.Application.Commands.AddCuisineToRestaurant;
using Gastromio.Core.Application.Commands.AddRestaurant;
using Gastromio.Core.Application.Commands.ChangeRestaurantName;
using Gastromio.Core.Application.Commands.DeactivateRestaurant;
using Gastromio.Core.Application.Commands.DisableSupportForRestaurant;
using Gastromio.Core.Application.Commands.EnableSupportForRestaurant;
using Gastromio.Core.Application.Commands.ImportDishData;
using Gastromio.Core.Application.Commands.ImportRestaurantData;
using Gastromio.Core.Application.Commands.RemoveAdminFromRestaurant;
using Gastromio.Core.Application.Commands.RemoveCuisineFromRestaurant;
using Gastromio.Core.Application.Commands.RemoveRestaurant;
using Gastromio.Core.Application.Commands.SetImportIdOfRestaurant;
using Gastromio.Core.Application.DTOs;
using Gastromio.Core.Application.Queries;
using Gastromio.Core.Application.Queries.SysAdminSearchForRestaurants;
using Gastromio.Core.Application.Services;
using Gastromio.Core.Domain.Model.Cuisines;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Core.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gastromio.App.Controllers.V1
{
    [Route("api/v1/systemadmin")]
    [ApiController]
    [Authorize()]
    public class RestaurantSysAdminController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly IQueryDispatcher queryDispatcher;
        private readonly IFailureMessageService failureMessageService;

        public RestaurantSysAdminController(ILogger<RestaurantSysAdminController> logger,
            ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher,
            IFailureMessageService failureMessageService)
        {
            this.logger = logger;
            this.commandDispatcher = commandDispatcher;
            this.queryDispatcher = queryDispatcher;
            this.failureMessageService = failureMessageService;
        }

        [Route("restaurants")]
        [HttpGet]
        public async Task<IActionResult> GetRestaurantsAsync(string search, int skip = 0, int take = -1)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var query = new SysAdminSearchForRestaurantsQuery(search, skip, take);

            var queryResult =
                await queryDispatcher
                    .PostAsync<SysAdminSearchForRestaurantsQuery, PagingDTO<RestaurantDTO>>(query,
                        new UserId(currentUserId));

            return ResultHelper.HandleResult(queryResult, failureMessageService);
        }

        [Route("restaurants")]
        [HttpPost]
        public async Task<IActionResult> PostRestaurantsAsync([FromBody] AddRestaurantModel addRestaurantModel)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult =
                await commandDispatcher.PostAsync<AddRestaurantCommand, RestaurantDTO>(
                    new AddRestaurantCommand(addRestaurantModel.Name), new UserId(currentUserId));
            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/changename")]
        [HttpPost]
        public async Task<IActionResult> PostChangeNameAsync(Guid restaurantId,
            [FromBody] ChangeRestaurantNameModel changeRestaurantNameModel)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<ChangeRestaurantNameCommand, bool>(
                new ChangeRestaurantNameCommand(new RestaurantId(restaurantId), changeRestaurantNameModel.Name),
                new UserId(currentUserId));
            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/setimportid")]
        [HttpPost]
        public async Task<IActionResult> PostSetImportIdAsync(Guid restaurantId,
            [FromBody] SetImportIdOfRestaurantModel setImportIdOfRestaurantModel)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<SetImportIdOfRestaurantCommand, bool>(
                new SetImportIdOfRestaurantCommand(new RestaurantId(restaurantId), setImportIdOfRestaurantModel.ImportId),
                new UserId(currentUserId));
            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/addcuisine")]
        [HttpPost]
        public async Task<IActionResult> PostAddCuisineAsync(Guid restaurantId,
            [FromBody] AddCuisineToRestaurantModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<AddCuisineToRestaurantCommand, bool>(
                new AddCuisineToRestaurantCommand(new RestaurantId(restaurantId), new CuisineId(model.CuisineId)),
                new UserId(currentUserId)
            );

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/removecuisine")]
        [HttpPost]
        public async Task<IActionResult> PostRemoveCuisineAsync(Guid restaurantId,
            [FromBody] RemoveCuisineFromRestaurantModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<RemoveCuisineFromRestaurantCommand, bool>(
                new RemoveCuisineFromRestaurantCommand(new RestaurantId(restaurantId), new CuisineId(model.CuisineId)),
                new UserId(currentUserId)
            );

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/addadmin")]
        [HttpPost]
        public async Task<IActionResult> PostAddAdminAsync(Guid restaurantId,
            [FromBody] AddAdminToRestaurantModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<AddAdminToRestaurantCommand, bool>(
                new AddAdminToRestaurantCommand(new RestaurantId(restaurantId), new UserId(model.UserId)),
                new UserId(currentUserId)
            );

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/removeadmin")]
        [HttpPost]
        public async Task<IActionResult> PostRemoveAdminAsync(Guid restaurantId,
            [FromBody] RemoveAdminFromRestaurantModel model)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<RemoveAdminFromRestaurantCommand, bool>(
                new RemoveAdminFromRestaurantCommand(new RestaurantId(restaurantId), new UserId(model.UserId)),
                new UserId(currentUserId)
            );

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/activate")]
        [HttpPost]
        public async Task<IActionResult> PostActivateAsync(Guid restaurantId)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<ActivateRestaurantCommand, bool>(
                new ActivateRestaurantCommand(new RestaurantId(restaurantId)),
                new UserId(currentUserId)
            );

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/deactivate")]
        [HttpPost]
        public async Task<IActionResult> PostDeactivateAsync(Guid restaurantId)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<DeactivateRestaurantCommand, bool>(
                new DeactivateRestaurantCommand(new RestaurantId(restaurantId)),
                new UserId(currentUserId)
            );

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/enablesupport")]
        [HttpPost]
        public async Task<IActionResult> PostEnableSupportAsync(Guid restaurantId)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult =
                await commandDispatcher.PostAsync<EnableSupportForRestaurantCommand, bool>(
                    new EnableSupportForRestaurantCommand(new RestaurantId(restaurantId)), new UserId(currentUserId));

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}/disablesupport")]
        [HttpPost]
        public async Task<IActionResult> PostDisableSupportAsync(Guid restaurantId)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult =
                await commandDispatcher.PostAsync<DisableSupportForRestaurantCommand, bool>(
                    new DisableSupportForRestaurantCommand(new RestaurantId(restaurantId)), new UserId(currentUserId));

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/{restaurantId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteRestaurantAsync(Guid restaurantId)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<RemoveRestaurantCommand, bool>(
                new RemoveRestaurantCommand(new RestaurantId(restaurantId)), new UserId(currentUserId));
            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("restaurants/import")]
        [HttpPost]
        public async Task<IActionResult> PostRestaurantImportAsync(string dryRun)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var file = Request.Form.Files[0];

            await using var stream = file.OpenReadStream();

            var commandResult =
                await commandDispatcher.PostAsync<ImportRestaurantDataCommand, ImportLog>(
                    new ImportRestaurantDataCommand(stream, dryRun != null), new UserId(currentUserId));

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("dishes/import")]
        [HttpPost]
        public async Task<IActionResult> PostDishImportAsync(string dryRun)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var file = Request.Form.Files[0];

            await using var stream = file.OpenReadStream();

            var commandResult =
                await commandDispatcher.PostAsync<ImportDishDataCommand, ImportLog>(
                    new ImportDishDataCommand(stream, dryRun != null), new UserId(currentUserId));

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }
   }
}
