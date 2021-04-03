using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.SetImportIdOfRestaurant;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Common;
using Gastromio.Domain.TestKit.Domain.Model.Restaurants;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.SetImportIdOfRestaurant
{
    public class SetImportIdOfRestaurantCommandHandlerTests : CommandHandlerTestBase<SetImportIdOfRestaurantCommandHandler,
        SetImportIdOfRestaurantCommand, bool>
    {
        private readonly Fixture fixture;

        public SetImportIdOfRestaurantCommandHandlerTests()
        {
            fixture = new Fixture(Role.SystemAdmin);
        }

        [Fact]
        public async Task HandleAsync_RestaurantNotKnown_ReturnsFailure()
        {
            // Arrange
            fixture.SetupRandomRestaurant(fixture.MinimumRole);
            fixture.SetupRandomImportId();
            fixture.SetupRestaurantRepositoryNotFindingRestaurantById();

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
        public async Task HandleAsync_AllValid_ChangesImportIdOfRestaurantAndReturnsSuccess()
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
                fixture.Restaurant.ImportId.Should().Be(fixture.ImportId);
                fixture.RestaurantRepositoryMock.VerifyStoreAsync(fixture.Restaurant, Times.Once);
            }
        }

        protected override
            CommandHandlerTestFixtureBase<SetImportIdOfRestaurantCommandHandler, SetImportIdOfRestaurantCommand, bool> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<SetImportIdOfRestaurantCommandHandler,
            SetImportIdOfRestaurantCommand, bool>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                RestaurantRepositoryMock = new RestaurantRepositoryMock(MockBehavior.Strict);
            }

            public RestaurantRepositoryMock RestaurantRepositoryMock { get; }

            public Restaurant Restaurant { get; private set; }

            public string ImportId { get; private set; }

            public override SetImportIdOfRestaurantCommandHandler CreateTestObject()
            {
                return new SetImportIdOfRestaurantCommandHandler(
                    RestaurantRepositoryMock.Object
                );
            }

            public override SetImportIdOfRestaurantCommand CreateSuccessfulCommand()
            {
                return new SetImportIdOfRestaurantCommand(Restaurant.Id, ImportId);
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

            public void SetupRandomImportId()
            {
                ImportId = RandomStringBuilder.BuildWithLength(20);
            }

            public void SetupRestaurantRepositoryFindingRestaurantById()
            {
                RestaurantRepositoryMock.SetupFindByRestaurantIdAsync(Restaurant.Id)
                    .ReturnsAsync(Restaurant);
            }

            public void SetupRestaurantRepositoryNotFindingRestaurantById()
            {
                RestaurantRepositoryMock.SetupFindByRestaurantIdAsync(Restaurant.Id)
                    .ReturnsAsync((Restaurant)null);
            }

            public void SetupRestaurantRepositoryStoringRestaurant()
            {
                RestaurantRepositoryMock.SetupStoreAsync(Restaurant)
                    .Returns(Task.CompletedTask);
            }

            public override void SetupForSuccessfulCommandExecution(Role? role)
            {
                SetupRandomRestaurant(role);
                SetupRandomImportId();
                SetupRestaurantRepositoryFindingRestaurantById();
                SetupRestaurantRepositoryStoringRestaurant();
            }
        }
    }
}
