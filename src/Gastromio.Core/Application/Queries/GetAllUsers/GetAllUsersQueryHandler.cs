using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.DTOs;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Users;

namespace Gastromio.Core.Application.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, ICollection<UserDTO>>
    {
        private readonly IUserRepository userRepository;

        public GetAllUsersQueryHandler(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<Result<ICollection<UserDTO>>> HandleAsync(GetAllUsersQuery query, User currentUser, CancellationToken cancellationToken = default)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (currentUser == null)
                return FailureResult<ICollection<UserDTO>>.Unauthorized();

            if (currentUser.Role < Role.SystemAdmin)
                return FailureResult<ICollection<UserDTO>>.Forbidden();

            var users = await userRepository.FindAllAsync(cancellationToken);

            return SuccessResult<ICollection<UserDTO>>.Create(users.Select(user => new UserDTO(user)).ToList());
        }
    }
}
