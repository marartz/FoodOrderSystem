using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Gastromio.App.BackgroundServices;
using Gastromio.Core;
using Gastromio.Notification.Sms77;
using Gastromio.Notification.Smtp;
using Gastromio.Persistence.MongoDB;
using Gastromio.Template.DotLiquid;
using Serilog;
using Microsoft.AspNetCore.Mvc;

namespace Gastromio.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCore();

            var connectionString = Configuration.GetConnectionString("MongoDB");
            Log.Logger.Information("Using connection string: {0}", connectionString);

            var databaseName = Configuration.GetValue("DatabaseName", Constants.DatabaseName);
            Log.Logger.Information("Using database name: {0}", databaseName);

            services.AddMongoDB(connectionString, databaseName);

            var configurationProvider = new ConfigurationProvider
            {
                IsTestSystem = Configuration.GetValue("IsTestSystem", true),
                EmailRecipientForTest = Configuration.GetValue("EmailRecipientForTest", "artz.marco@gmx.net"),
                MobileRecipientForTest = Configuration.GetValue("MobileRecipientForTest", "+4915165119020")
            };

            Log.Information($"IsTestSystem: {configurationProvider.IsTestSystem}");
            if (configurationProvider.IsTestSystem)
            {
                Log.Information($"EmailRecipientForTest: {configurationProvider.EmailRecipientForTest}");
            }

            services.AddSingleton<Gastromio.Core.Application.Ports.IConfigurationProvider>(configurationProvider);

            var smtpConfiguration = new SmtpEmailConfiguration
            {
                ServerName = Configuration.GetValue<string>("Smtp:ServerName"),
                Port = Configuration.GetValue<int>("Smtp:Port"),
                UserName = Configuration.GetValue<string>("Smtp:UserName"),
                Password = Configuration.GetValue<string>("Smtp:Password")
            };
            services.AddSingleton(smtpConfiguration);
            services.AddEmailNotificationViaSmtp();

            var sms77MobileConfiguration = new Sms77MobileConfiguration
            {
                ApiToken = Configuration.GetValue<string>("SMS77:ApiToken")
            };
            services.AddSingleton(sms77MobileConfiguration);
            services.AddMobileNotificationViaSms77();

            services.AddDotLiquid();

            services.AddHostedService<NotificationBackgroundService>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                });

            services.AddControllersWithViews();
            services.AddRazorPages();

            // Errors caused by request body deserialization or data annotations on DTOs (e.g. '[Required]') are handled automatically.
            // In this case controller endpoints are not reached and BadRequest is returned (client validation should catch these cases earlier).
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    return new BadRequestResult();
                };
            });

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                }
            });
        }
    }
}
