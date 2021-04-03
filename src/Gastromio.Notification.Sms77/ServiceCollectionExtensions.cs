using System.Runtime.CompilerServices;
using Gastromio.Core.Application.Ports.Notification;
using Microsoft.Extensions.DependencyInjection;

[assembly:InternalsVisibleTo("Gastromio.Notification.Sms77.Tests")]

namespace Gastromio.Notification.Sms77
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMobileNotificationViaSms77(this IServiceCollection services)
        {
            services.AddTransient<IMobileNotificationService, Sms77MobileNotificationService>();
        }
    }
}
