﻿using FoodOrderSystem.App.Helper;
using FoodOrderSystem.App.Models;
using FoodOrderSystem.Domain.Commands;
using FoodOrderSystem.Domain.Commands.AddCuisine;
using FoodOrderSystem.Domain.Commands.ChangeCuisine;
using FoodOrderSystem.Domain.Commands.RemoveCuisine;
using FoodOrderSystem.Domain.Model.Cuisine;
using FoodOrderSystem.Domain.Model.User;
using FoodOrderSystem.Domain.Queries;
using FoodOrderSystem.Domain.Queries.GetAllCuisines;
using FoodOrderSystem.Domain.Services;
using FoodOrderSystem.Domain.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FoodOrderSystem.App.Controllers.V1
{
    [Route("api/v1/systemadmin")]
    [ApiController]
    [Authorize()]
    public class CuisineAdminController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly IUserRepository userRepository;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly IQueryDispatcher queryDispatcher;
        private readonly IFailureMessageService failureMessageService;

        public CuisineAdminController(ILogger<CuisineAdminController> logger, IUserRepository userRepository, ICommandDispatcher commandDispatcher,
            IQueryDispatcher queryDispatcher, IFailureMessageService failureMessageService)
        {
            this.logger = logger;
            this.userRepository = userRepository;
            this.commandDispatcher = commandDispatcher;
            this.queryDispatcher = queryDispatcher;
            this.failureMessageService = failureMessageService;
        }

        [Route("cuisines")]
        [HttpGet]
        public async Task<IActionResult> GetCuisinesAsync()
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims.FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();
            var currentUser = await userRepository.FindByUserIdAsync(new UserId(currentUserId));

            var queryResult = await queryDispatcher.PostAsync<GetAllCuisinesQuery, ICollection<CuisineViewModel>>(new GetAllCuisinesQuery(), currentUser);
            return ResultHelper.HandleResult(queryResult, failureMessageService);
        }

        [Route("cuisines")]
        [HttpPost]
        public async Task<IActionResult> PostCuisinesAsync([FromBody] AddCuisineModel addCuisineModel)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims.FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();
            var currentUser = await userRepository.FindByUserIdAsync(new UserId(currentUserId));

            var commandResult = await commandDispatcher.PostAsync<AddCuisineCommand, CuisineViewModel>(new AddCuisineCommand(addCuisineModel.Name), currentUser);
            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("cuisines/{cuisineId}/change")]
        [HttpPost]
        public async Task<IActionResult> PostChangeAsync(Guid cuisineId, [FromBody] ChangeCuisineModel changeCuisineModel)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims.FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();
            var currentUser = await userRepository.FindByUserIdAsync(new UserId(currentUserId));

            var commandResult = await commandDispatcher.PostAsync<ChangeCuisineCommand, bool>(new ChangeCuisineCommand(new CuisineId(cuisineId), changeCuisineModel.Name), currentUser);
            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("cuisines/{cuisineId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteCuisineAsync(Guid cuisineId)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims.FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();
            var currentUser = await userRepository.FindByUserIdAsync(new UserId(currentUserId));

            var commandResult = await commandDispatcher.PostAsync<RemoveCuisineCommand, bool>(new RemoveCuisineCommand(new CuisineId(cuisineId)), currentUser);
            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }
    }
}