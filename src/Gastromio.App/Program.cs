using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using Gastromio.Core.Application.Commands.AddTestData;
using Gastromio.Core.Application.Commands.EnsureAdminUser;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Persistence.MongoDB;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace Gastromio.App
{
    public class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("./logs/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Starting web host");

                var host = CreateHostBuilder(args).Build();

                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;

                    var configuration = services.GetService<IConfiguration>();

                    var dbAdminService = services.GetService<IDbAdminService>();

                    var currentUser = new User(
                        new UserId(Guid.Empty),
                        Role.SystemAdmin,
                        "admin@gastromio.de",
                        null,
                        null,
                        null,
                        null,
                        DateTimeOffset.UtcNow,
                        new UserId(Guid.Empty),
                        DateTimeOffset.UtcNow,
                        new UserId(Guid.Empty)
                    );

                    var ensureAdminUserCommandHandler = services.GetService<EnsureAdminUserCommandHandler>();

                    var seed =
                        string.Equals(configuration["Seed"], "true", StringComparison.InvariantCultureIgnoreCase) ||
                        configuration.GetSection("Seed").GetChildren().Any();
                    if (seed)
                    {
                        dbAdminService.PurgeDatabase();
                        dbAdminService.PrepareDatabase();

                        var result = ensureAdminUserCommandHandler.HandleAsync(new EnsureAdminUserCommand(), currentUser).Result;
                        if (result.IsFailure)
                        {
                            var failureResult = (FailureResult<bool>) result;
                            Log.Logger.Error(string.Join("; ", failureResult.Errors));
                            throw new InvalidOperationException("Error during command EnsureAdminUserCommand");
                        }

                        if (!Int32.TryParse(configuration["Seed:Params:UserCount"], out var userCount))
                            userCount = 20;

                        if (!Int32.TryParse(configuration["Seed:Params:RestCount"], out var restCount))
                            restCount = 15;

                        if (!Int32.TryParse(configuration["Seed:Params:DishCatCount"], out var dishCatCount))
                            dishCatCount = 8;

                        if (!Int32.TryParse(configuration["Seed:Params:DishCount"], out var dishCount))
                            dishCount = 8;

                        var addTestDataCommandHandler = services.GetService<AddTestDataCommandHandler>();

                        result = addTestDataCommandHandler
                            .HandleAsync(new AddTestDataCommand(userCount, restCount, dishCatCount, dishCount),
                                currentUser).Result;
                        if (result.IsFailure)
                        {
                            var failureResult = (FailureResult<bool>) result;
                            Log.Logger.Error(string.Join("; ", failureResult.Errors.Values.SelectMany(en => en)));
                            throw new InvalidOperationException("Error during command AddTestDataCommand");
                        }
                    }
                    else
                    {
                        dbAdminService.PrepareDatabase();

                        var result = ensureAdminUserCommandHandler.HandleAsync(new EnsureAdminUserCommand(), currentUser).Result;
                        if (result.IsFailure)
                        {
                            var failureResult = (FailureResult<bool>) result;
                            Log.Logger.Error(string.Join("; ", failureResult.Errors));
                            throw new InvalidOperationException("Error during command EnsureAdminUserCommand");
                        }
                    }

                    dbAdminService.CorrectRestaurantAliases();
                }

                host.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) => { config.AddCommandLine(args); })
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}
