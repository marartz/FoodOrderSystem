using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.ChangeRestaurantImage;
using Gastromio.Core.Domain.Model.RestaurantImages;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Domain.Model.RestaurantImages;
using Gastromio.Domain.TestKit.Domain.Model.Restaurants;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.ChangeRestaurantImage
{
    public class ChangeRestaurantImageCommandHandlerTests : CommandHandlerTestBase<
        ChangeRestaurantImageCommandHandler, ChangeRestaurantImageCommand, bool>
    {
        private readonly Fixture fixture;

        private const string ImageDataBase64 =
            "/9j/4AAQSkZJRgABAQEBLAEsAAD//gATQ3JlYXRlZCB3aXRoIEdJTVD/4gKwSUNDX1BST0ZJTEUA" +
            "AQEAAAKgbGNtcwQwAABtbnRyUkdCIFhZWiAH5QACABEACwAaAA1hY3NwQVBQTAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAA9tYAAQAAAADTLWxjbXMAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAA1kZXNjAAABIAAAAEBjcHJ0AAABYAAAADZ3dHB0AAABmAAAABRj" +
            "aGFkAAABrAAAACxyWFlaAAAB2AAAABRiWFlaAAAB7AAAABRnWFlaAAACAAAAABRyVFJDAAACFAAA" +
            "ACBnVFJDAAACFAAAACBiVFJDAAACFAAAACBjaHJtAAACNAAAACRkbW5kAAACWAAAACRkbWRkAAAC" +
            "fAAAACRtbHVjAAAAAAAAAAEAAAAMZW5VUwAAACQAAAAcAEcASQBNAFAAIABiAHUAaQBsAHQALQBp" +
            "AG4AIABzAFIARwBCbWx1YwAAAAAAAAABAAAADGVuVVMAAAAaAAAAHABQAHUAYgBsAGkAYwAgAEQA" +
            "bwBtAGEAaQBuAABYWVogAAAAAAAA9tYAAQAAAADTLXNmMzIAAAAAAAEMQgAABd7///MlAAAHkwAA" +
            "/ZD///uh///9ogAAA9wAAMBuWFlaIAAAAAAAAG+gAAA49QAAA5BYWVogAAAAAAAAJJ8AAA+EAAC2" +
            "xFhZWiAAAAAAAABilwAAt4cAABjZcGFyYQAAAAAAAwAAAAJmZgAA8qcAAA1ZAAAT0AAACltjaHJt" +
            "AAAAAAADAAAAAKPXAABUfAAATM0AAJmaAAAmZwAAD1xtbHVjAAAAAAAAAAEAAAAMZW5VUwAAAAgA" +
            "AAAcAEcASQBNAFBtbHVjAAAAAAAAAAEAAAAMZW5VUwAAAAgAAAAcAHMAUgBHAEL/2wBDAP//////" +
            "////////////////////////////////////////////////////////////////////////////" +
            "////2wBDAf//////////////////////////////////////////////////////////////////" +
            "////////////////////wgARCAABAAEDAREAAhEBAxEB/8QAFAABAAAAAAAAAAAAAAAAAAAAAv/E" +
            "ABQBAQAAAAAAAAAAAAAAAAAAAAD/2gAMAwEAAhADEAAAAUf/xAAUEAEAAAAAAAAAAAAAAAAAAAAA" +
            "/9oACAEBAAEFAn//xAAUEQEAAAAAAAAAAAAAAAAAAAAA/9oACAEDAQE/AX//xAAUEQEAAAAAAAAA" +
            "AAAAAAAAAAAA/9oACAECAQE/AX//xAAUEAEAAAAAAAAAAAAAAAAAAAAA/9oACAEBAAY/An//xAAU" +
            "EAEAAAAAAAAAAAAAAAAAAAAA/9oACAEBAAE/IX//2gAMAwEAAgADAAAAEB//xAAUEQEAAAAAAAAA" +
            "AAAAAAAAAAAA/9oACAEDAQE/EH//xAAUEQEAAAAAAAAAAAAAAAAAAAAA/9oACAECAQE/EH//xAAU" +
            "EAEAAAAAAAAAAAAAAAAAAAAA/9oACAEBAAE/EH//2Q==";

        public ChangeRestaurantImageCommandHandlerTests()
        {
            fixture = new Fixture(Role.RestaurantAdmin);
        }

        [Fact]
        public async Task HandleAsync_RestaurantNotKnown_ReturnsFailure()
        {
            // Arrange
            fixture.SetupRandomRestaurant(fixture.MinimumRole);
            fixture.SetupRandomRestaurantImage();
            fixture.SetupRestaurantRepositoryNotFindingRestaurant();

            var testObject = fixture.CreateTestObject();
            var command = fixture.CreateSuccessfulCommand();

            // Act
            var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public async Task HandleAsync_AllValid_AddsRestaurantImageToRestaurantAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupForSuccessfulCommandExecution(fixture.MinimumRole);

            var testObject = fixture.CreateTestObject();
            var command = fixture.CreateSuccessfulCommand();

            // Act
            var result = await testObject.HandleAsync(command, fixture.UserWithMinimumRole, CancellationToken.None);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                fixture.RestaurantImageRepositoryMock
                    .Verify(m => m.StoreAsync(
                        It.Is<RestaurantImage>(i => i.Type == fixture.RestaurantImage.Type),
                        It.IsAny<CancellationToken>()
                    ), Times.Once);
            }
        }

        protected override
            CommandHandlerTestFixtureBase<ChangeRestaurantImageCommandHandler, ChangeRestaurantImageCommand,
                bool> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<ChangeRestaurantImageCommandHandler,
            ChangeRestaurantImageCommand, bool>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                RestaurantRepositoryMock = new RestaurantRepositoryMock(MockBehavior.Strict);
                RestaurantImageRepositoryMock = new RestaurantImageRepositoryMock(MockBehavior.Strict);
            }

            public RestaurantRepositoryMock RestaurantRepositoryMock { get; }

            public RestaurantImageRepositoryMock RestaurantImageRepositoryMock { get; }

            public Restaurant Restaurant { get; private set; }

            public RestaurantImage RestaurantImage { get; private set; }

            public override ChangeRestaurantImageCommandHandler CreateTestObject()
            {
                return new ChangeRestaurantImageCommandHandler(
                    RestaurantRepositoryMock.Object,
                    RestaurantImageRepositoryMock.Object
                );
            }

            public override ChangeRestaurantImageCommand CreateSuccessfulCommand()
            {
                return new ChangeRestaurantImageCommand(Restaurant.Id, RestaurantImage.Type, RestaurantImage.Data);
            }

            public void SetupRandomRestaurant(Role? role)
            {
                var builder = new RestaurantBuilder();

                if (role == Role.RestaurantAdmin)
                {
                    builder = builder
                        .WithAdministrators(new HashSet<UserId> {UserId});
                }

                Restaurant = builder
                    .WithValidConstrains()
                    .Create();
            }

            public void SetupRandomRestaurantImage()
            {
                var imageData = Convert.FromBase64String(ImageDataBase64);

                RestaurantImage = new RestaurantImageBuilder()
                    .WithRestaurantId(Restaurant.Id)
                    .WithType("logo")
                    .WithData(imageData)
                    .WithCreatedBy(UserId)
                    .WithCreatedOn(DateTimeOffset.Now)
                    .WithUpdatedBy(UserId)
                    .WithUpdatedOn(DateTimeOffset.Now)
                    .Create();
            }

            public void SetupRestaurantRepositoryFindingRestaurant()
            {
                RestaurantRepositoryMock.SetupFindByRestaurantIdAsync(Restaurant.Id)
                    .ReturnsAsync(Restaurant);
            }

            public void SetupRestaurantRepositoryNotFindingRestaurant()
            {
                RestaurantRepositoryMock.SetupFindByRestaurantIdAsync(Restaurant.Id)
                    .ReturnsAsync((Restaurant) null);
            }

            public void SetupRestaurantImageRepositoryFindingNoImagesForRestaurant()
            {
                RestaurantImageRepositoryMock.SetupFindByRestaurantIdAsync(Restaurant.Id)
                    .ReturnsAsync(Enumerable.Empty<RestaurantImage>());
            }

            public void SetupRestaurantImageRepositoryFindingNoTypesForRestaurant()
            {
                RestaurantImageRepositoryMock.SetupFindTypesByRestaurantIdAsync(Restaurant.Id)
                    .ReturnsAsync(Enumerable.Empty<string>());
            }

            public void SetupRestaurantImageRepositoryStoringRestaurantImage()
            {
                RestaurantImageRepositoryMock
                    .Setup(m => m.StoreAsync(
                        It.Is<RestaurantImage>(i => i.Type == RestaurantImage.Type),
                        It.IsAny<CancellationToken>()
                    ))
                    .Returns(Task.CompletedTask);
            }

            public override void SetupForSuccessfulCommandExecution(Role? role)
            {
                SetupRandomRestaurant(role);
                SetupRandomRestaurantImage();
                SetupRestaurantRepositoryFindingRestaurant();
                SetupRestaurantImageRepositoryFindingNoImagesForRestaurant();
                SetupRestaurantImageRepositoryFindingNoTypesForRestaurant();
                SetupRestaurantImageRepositoryStoringRestaurantImage();
            }
        }
    }
}
