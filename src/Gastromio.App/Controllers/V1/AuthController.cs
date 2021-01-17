using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Gastromio.App.Helper;
using Gastromio.App.Models;
using Gastromio.Core.Application.Commands;
using Gastromio.Core.Application.Commands.ChangePasswordWithResetCode;
using Gastromio.Core.Application.Commands.ChangeUserPassword;
using Gastromio.Core.Application.Commands.Login;
using Gastromio.Core.Application.Commands.RequestPasswordChange;
using Gastromio.Core.Application.Commands.ValidatePasswordResetCode;
using Gastromio.Core.Application.DTOs;
using Gastromio.Core.Application.Services;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Gastromio.App.Controllers.V1
{
    [Route("api/v1/auth")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly IConfiguration config;
        private readonly ICommandDispatcher commandDispatcher;
        private readonly IFailureMessageService failureMessageService;
        private readonly IMemoryCache memoryCache;

        public AuthController(
            ILogger<AuthController> logger,
            IConfiguration config,
            ICommandDispatcher commandDispatcher,
            IFailureMessageService failureMessageService,
            IMemoryCache memoryCache
        )
        {
            this.logger = logger;
            this.config = config;
            this.commandDispatcher = commandDispatcher;
            this.failureMessageService = failureMessageService;
            this.memoryCache = memoryCache;
        }

        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> PostLoginAsync([FromBody]LoginModel loginModel)
        {
            var cacheKey = GetFailureCountCacheKey(Request.Host.Value);
            if (memoryCache.TryGetValue<int>(cacheKey, out var delay))
            {
                logger.LogInformation($"Waiting {delay} ms");
                await Task.Delay(delay);
            }

            var commandResult = await commandDispatcher.PostAsync<LoginCommand, UserDTO>(new LoginCommand(loginModel.Email, loginModel.Password), null);
            if (commandResult is SuccessResult<UserDTO> successResult)
            {
                memoryCache.Remove(cacheKey);

                var tokenString = GenerateJSONWebToken(successResult.Value);
                return Ok(new { token = tokenString, user = successResult.Value });
            }
            else
            {
                if (delay == 0)
                {
                    delay = 100;
                }
                else
                {
                    delay = (int)(delay * 1.2);
                    if (delay > 1000)
                    {
                        delay = 1000;
                    }
                }

                memoryCache.Set(cacheKey, delay);
            }

            return ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [AllowAnonymous]
        [Route("requestpasswordchange")]
        [HttpPost]
        public async Task<IActionResult> PostRequestPasswordChangeAsync(
            [FromBody] RequestPasswordChangeModel requestPasswordChangeModel)
        {
            var commandResult =
                await commandDispatcher.PostAsync<RequestPasswordChangeCommand, bool>(
                    new RequestPasswordChangeCommand(requestPasswordChangeModel.UserEmail), null);

            return commandResult is SuccessResult<bool>
                ? Ok()
                : ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [AllowAnonymous]
        [Route("validatepasswordresetcode")]
        [HttpPost]
        public async Task<IActionResult> PostValidatePasswordResetCodeAsync(
            [FromBody] ValidatePasswordResetCodeModel validatePasswordResetCodeModel)
        {
            byte[] passwordResetCode;

            try
            {
                passwordResetCode = Convert.FromBase64String(validatePasswordResetCodeModel.PasswordResetCode);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            var commandResult = await commandDispatcher.PostAsync<ValidatePasswordResetCodeCommand, bool>(
                new ValidatePasswordResetCodeCommand(new UserId(validatePasswordResetCodeModel.UserId),
                    passwordResetCode), null);

            return commandResult is SuccessResult<bool>
                ? Ok()
                : ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [AllowAnonymous]
        [Route("changepasswordwithresetcode")]
        [HttpPost]
        public async Task<IActionResult> PostChangePasswordWithResetCodeAsync(
            [FromBody] ChangePasswordWithResetCodeModel changePasswordWithResetCodeModel)
        {
            byte[] passwordResetCode;

            try
            {
                passwordResetCode = Convert.FromBase64String(changePasswordWithResetCodeModel.PasswordResetCode);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            var commandResult = await commandDispatcher.PostAsync<ChangePasswordWithResetCodeCommand, bool>(
                new ChangePasswordWithResetCodeCommand(new UserId(changePasswordWithResetCodeModel.UserId),
                    passwordResetCode, changePasswordWithResetCodeModel.Password), null);

            return commandResult is SuccessResult<bool>
                ? Ok()
                : ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("changepassword")]
        [HttpPost]
        public async Task<IActionResult> PostChangePasswordAsync(
            [FromBody] ChangeUserPasswordModel changeUserPasswordModel)
        {
            var identityName = (User.Identity as ClaimsIdentity).Claims
                .FirstOrDefault(en => en.Type == ClaimTypes.NameIdentifier)?.Value;
            if (identityName == null || !Guid.TryParse(identityName, out var currentUserId))
                return Unauthorized();

            var curUserId = new UserId(currentUserId);

            var commandResult = await commandDispatcher.PostAsync<ChangeUserPasswordCommand, bool>(
                new ChangeUserPasswordCommand(curUserId, changeUserPasswordModel.Password), curUserId);

            return commandResult is SuccessResult<bool>
                ? Ok()
                : ResultHelper.HandleResult(commandResult, failureMessageService);
        }

        [Route("ping")]
        [HttpGet]
        public IActionResult Ping()
        {
            return Ok("pong");
        }

        private string GetFailureCountCacheKey(string userHostAddress)
        {
            return $"FailureCount:{userHostAddress}";
        }

        private string GenerateJSONWebToken(UserDTO user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                config["Jwt:Issuer"],
                config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
