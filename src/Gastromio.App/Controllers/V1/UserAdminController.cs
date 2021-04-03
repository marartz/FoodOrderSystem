using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Gastromio.App.Helper;
using Gastromio.App.Models;
using Gastromio.Core.Application.Commands;
using Gastromio.Core.Application.Commands.AddUser;
using Gastromio.Core.Application.Commands.ChangeUserDetails;
using Gastromio.Core.Application.Commands.ChangeUserPassword;
using Gastromio.Core.Application.Commands.RemoveUser;
using Gastromio.Core.Application.DTOs;
using Gastromio.Core.Application.Queries;
using Gastromio.Core.Application.Queries.SearchForUsers;
using Gastromio.Core.Application.Services;
using Gastromio.Core.Domain.Model.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gastromio.App.Controllers.V1
{
    [Route("api/v1/systemadmin")]
    [ApiController]
    [Authorize()]
    public class UserAdminController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly IQueryDispatcher queryDispatcher;
        private readonly IFailureMessageService failureMessageService;

        public UserAdminController(ILogger<UserAdminController> logger, ICommandDispatcher commandDispatcher,
            IQueryDispatcher queryDispatcher, IFailureMessageService failureMessageService)
        {
            this.logger = logger;
            this.commandDispatcher = commandDispatcher;
            this.queryDispatcher = queryDispatcher;
            this.failureMessageService = failureMessageService;
        }

        [Route("users")]
        [HttpGet]
        public async Task<IActionResult> GetUsersAsync(string search, int skip = 0, int take = -1)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var query = new SearchForUsersQuery(search, null, skip, take);

            var queryResult =
                await queryDispatcher
                    .PostAsync<SearchForUsersQuery, PagingDTO<UserDTO>>(query, new UserId(currentUserId));

            return ResultHelper.HandleResult(queryResult, failureMessageService);
        }

        [Route("users")]
        [HttpPost]
        public async Task<IActionResult> PostUsersAsync([FromBody] AddUserModel addUserModel)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var role = (Role) Enum.Parse(typeof(Role), addUserModel.Role);

            var commandResult = await commandDispatcher.PostAsync<AddUserCommand, UserDTO>(
                new AddUserCommand(role, addUserModel.Email, addUserModel.Password), new UserId(currentUserId));
            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("users/{userId}/changedetails")]
        [HttpPost]
        public async Task<IActionResult> PostChangeDetailsAsync(Guid userId,
            [FromBody] ChangeUserDetailsModel changeUserDetailsModel)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var role = (Role) Enum.Parse(typeof(Role), changeUserDetailsModel.Role);

            var commandResult = await commandDispatcher.PostAsync<ChangeUserDetailsCommand, bool>(
                new ChangeUserDetailsCommand(new UserId(userId), role, changeUserDetailsModel.Email),
                new UserId(currentUserId)
            );

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("users/{userId}/changepassword")]
        [HttpPost]
        public async Task<IActionResult> PostChangePasswordAsync(Guid userId,
            [FromBody] ChangeUserPasswordModel changeUserPasswordModel)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<ChangeUserPasswordCommand, bool>(
                new ChangeUserPasswordCommand(new UserId(userId), changeUserPasswordModel.Password),
                new UserId(currentUserId));
            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("users/{userId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteUserAsync(Guid userId)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult =
                await commandDispatcher.PostAsync<RemoveUserCommand, bool>(new RemoveUserCommand(new UserId(userId)),
                    new UserId(currentUserId));
            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }
    }
}
