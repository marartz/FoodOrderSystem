using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.RestaurantImages;
using Gastromio.Core.Domain.Model.Users;
using SixLabors.ImageSharp;

namespace Gastromio.Core.Application.Commands.ChangeRestaurantImage
{
    public class ChangeRestaurantImageCommandHandler : ICommandHandler<ChangeRestaurantImageCommand, bool>
    {
        private readonly IRestaurantRepository restaurantRepository;
        private readonly IRestaurantImageRepository restaurantImageRepository;

        public ChangeRestaurantImageCommandHandler(
            IRestaurantRepository restaurantRepository,
            IRestaurantImageRepository restaurantImageRepository
        )
        {
            this.restaurantRepository = restaurantRepository;
            this.restaurantImageRepository = restaurantImageRepository;
        }

        public async Task<Result<bool>> HandleAsync(ChangeRestaurantImageCommand command, User currentUser, CancellationToken cancellationToken = default)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (currentUser == null)
                return FailureResult<bool>.Unauthorized();

            if (currentUser.Role < Role.RestaurantAdmin)
                return FailureResult<bool>.Forbidden();

            var restaurant = await restaurantRepository.FindByRestaurantIdAsync(command.RestaurantId, cancellationToken);
            if (restaurant == null)
                return FailureResult<bool>.Create(FailureResultCode.RestaurantDoesNotExist);

            if (currentUser.Role == Role.RestaurantAdmin && !restaurant.HasAdministrator(currentUser.Id))
                return FailureResult<bool>.Forbidden();

            if (string.IsNullOrWhiteSpace(command.Type))
                return FailureResult<bool>.Create(FailureResultCode.RestaurantImageTypeRequired);

            if (command.Image == null)
            {
                await restaurantImageRepository.RemoveByRestaurantIdAndTypeAsync(command.RestaurantId, command.Type,
                    cancellationToken);
                return SuccessResult<bool>.Create(true);
            }

            var types = (await restaurantImageRepository.FindTypesByRestaurantIdAsync(command.RestaurantId,
                cancellationToken))?.ToList();
            if (types != null && types.Contains(command.Type))
            {
                await restaurantImageRepository.RemoveByRestaurantIdAndTypeAsync(command.RestaurantId, command.Type,
                    cancellationToken);
            }

            if (command.Image.Length > 2024 * 2024) // 4 MB
                return FailureResult<bool>.Create(FailureResultCode.RestaurantImageDataTooBig);

            try
            {
                using (var srcMemoryStream = new MemoryStream(command.Image))
                using (var dstMemoryStream = new MemoryStream())
                {
                    var imageObj = Image.Load(srcMemoryStream);
                    if (imageObj == null)
                        return FailureResult<bool>.Create(FailureResultCode.RestaurantImageNotValid);

                    imageObj.SaveAsJpeg(dstMemoryStream);

                    await dstMemoryStream.FlushAsync(cancellationToken);

                    var restaurantImage = new RestaurantImage(
                        new RestaurantImageId(Guid.NewGuid()),
                        command.RestaurantId,
                        command.Type,
                        dstMemoryStream.GetBuffer(),
                        DateTimeOffset.UtcNow,
                        currentUser.Id,
                        DateTimeOffset.UtcNow,
                        currentUser.Id
                    );

                    await restaurantImageRepository.StoreAsync(restaurantImage, cancellationToken);

                    return SuccessResult<bool>.Create(true);
                }
            }
            catch
            {
                // TODO: Log error
                return FailureResult<bool>.Create(FailureResultCode.RestaurantImageNotValid);
            }
        }
    }
}
