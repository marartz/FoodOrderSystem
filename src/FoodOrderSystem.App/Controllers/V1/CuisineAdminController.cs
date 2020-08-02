﻿using FoodOrderSystem.App.Helper;
using FoodOrderSystem.App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FoodOrderSystem.Core.Application.Commands;
using FoodOrderSystem.Core.Application.Commands.AddCuisine;
using FoodOrderSystem.Core.Application.Commands.ChangeCuisine;
using FoodOrderSystem.Core.Application.Commands.RemoveCuisine;
using FoodOrderSystem.Core.Application.DTOs;
using FoodOrderSystem.Core.Application.Queries;
using FoodOrderSystem.Core.Application.Queries.GetAllCuisines;
using FoodOrderSystem.Core.Application.Services;
using FoodOrderSystem.Core.Domain.Model.Cuisine;
using FoodOrderSystem.Core.Domain.Model.User;

namespace FoodOrderSystem.App.Controllers.V1
{
    [Route("api/v1/systemadmin")]
    [ApiController]
    [Authorize()]
    public class CuisineAdminController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly IQueryDispatcher queryDispatcher;
        private readonly IFailureMessageService failureMessageService;

        public CuisineAdminController(ILogger<CuisineAdminController> logger, ICommandDispatcher commandDispatcher,
            IQueryDispatcher queryDispatcher, IFailureMessageService failureMessageService)
        {
            this.logger = logger;
            this.commandDispatcher = commandDispatcher;
            this.queryDispatcher = queryDispatcher;
            this.failureMessageService = failureMessageService;
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

        [Route("cuisines")]
        [HttpPost]
        public async Task<IActionResult> PostCuisinesAsync([FromBody] AddCuisineModel addCuisineModel)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult =
                await commandDispatcher.PostAsync<AddCuisineCommand, CuisineDTO>(
                    new AddCuisineCommand(addCuisineModel.Name), new UserId(currentUserId));
            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("cuisines/{cuisineId}/change")]
        [HttpPost]
        public async Task<IActionResult> PostChangeAsync(Guid cuisineId,
            [FromBody] ChangeCuisineModel changeCuisineModel)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult = await commandDispatcher.PostAsync<ChangeCuisineCommand, bool>(
                new ChangeCuisineCommand(new CuisineId(cuisineId), changeCuisineModel.Name), new UserId(currentUserId));
            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("cuisines/{cuisineId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteCuisineAsync(Guid cuisineId)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var commandResult =
                await commandDispatcher.PostAsync<RemoveCuisineCommand, bool>(
                    new RemoveCuisineCommand(new CuisineId(cuisineId)), new UserId(currentUserId));
            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }
    }
}