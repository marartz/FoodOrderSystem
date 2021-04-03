using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Gastromio.Core.Application.Queries
{
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IServiceProvider serviceProvider;
        private static readonly IDictionary<Type, Type> QueryHandlerMapping = new Dictionary<Type, Type>();

        public static void Initialize(IServiceCollection services)
        {
            var queryHandlerType = typeof(IQueryHandler<,>);
            var handlerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetInterfaces().Any(intf => intf.IsGenericType && intf.GetGenericTypeDefinition() == queryHandlerType));
            foreach (var handlerType in handlerTypes)
            {
                var queryHandlerIntf = handlerType.GetInterfaces().First(intf => intf.IsGenericType && intf.GetGenericTypeDefinition() == queryHandlerType);
                var queryType = queryHandlerIntf.GetGenericArguments()[0];
                QueryHandlerMapping.Add(queryType, handlerType);
                services.AddTransient(handlerType);
            }
        }

        public QueryDispatcher(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task<Result<TResult>> PostAsync<TQuery, TResult>(TQuery query, UserId currentUserId, CancellationToken cancellationToken = default) where TQuery : IQuery<TResult>
        {
            User currentUser = null;

            if (currentUserId != null)
            {
                var userRepository = serviceProvider.GetService<IUserRepository>();
                currentUser = await userRepository.FindByUserIdAsync(currentUserId, cancellationToken);
            }

            var queryType = typeof(TQuery);

            if (!QueryHandlerMapping.TryGetValue(queryType, out var handlerType))
                throw new InvalidOperationException($"could not find handler for query with type {queryType.FullName}");

            var handler = serviceProvider.GetService(handlerType) as IQueryHandler<TQuery, TResult>;

            return await handler.HandleAsync(query, currentUser, cancellationToken);
        }
    }
}
