using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Gastromio.Core.Application.Commands
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IServiceProvider serviceProvider;
        private static readonly IDictionary<Type, Type> CommandHandlerMapping = new Dictionary<Type, Type>();

        public static void Initialize(IServiceCollection services)
        {
            var commandHandlerType = typeof(ICommandHandler<,>);
            var handlerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetInterfaces().Any(intf => intf.IsGenericType && intf.GetGenericTypeDefinition() == commandHandlerType));
            foreach (var handlerType in handlerTypes)
            {
                var commandHandlerIntf = handlerType.GetInterfaces().First(intf => intf.IsGenericType && intf.GetGenericTypeDefinition() == commandHandlerType);
                var commandType = commandHandlerIntf.GetGenericArguments()[0];
                CommandHandlerMapping.Add(commandType, handlerType);
                services.AddTransient(handlerType);
            }
        }

        public CommandDispatcher(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task<Result<TResult>> PostAsync<TCommand, TResult>(TCommand command, UserId currentUserId, CancellationToken cancellationToken = default) where TCommand : ICommand<TResult>
        {
            User currentUser = null;

            if (currentUserId != null)
            {
                var userRepository = serviceProvider.GetService<IUserRepository>();
                currentUser = await userRepository.FindByUserIdAsync(currentUserId, cancellationToken);
            }

            var commandType = typeof(TCommand);
            
            if (!CommandHandlerMapping.TryGetValue(commandType, out var handlerType))
                throw new InvalidOperationException($"could not find handler for command with type {commandType.FullName}");

            var handler = serviceProvider.GetService(handlerType) as ICommandHandler<TCommand, TResult>;

            return await handler.HandleAsync(command, currentUser, cancellationToken);
        }
    }
}
