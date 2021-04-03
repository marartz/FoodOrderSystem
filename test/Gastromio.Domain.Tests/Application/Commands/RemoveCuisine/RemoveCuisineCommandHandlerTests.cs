using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.RemoveCuisine;
using Gastromio.Core.Domain.Model.Cuisines;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Domain.Model.Cuisines;
using Gastromio.Domain.TestKit.Domain.Model.Restaurants;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.RemoveCuisine
{
    public class RemoveCuisineCommandHandlerTests : CommandHandlerTestBase<RemoveCuisineCommandHandler,
        RemoveCuisineCommand, bool>
    {
        private readonly Fixture fixture;

        public RemoveCuisineCommandHandlerTests()
        {
            fixture = new Fixture(Role.SystemAdmin);
        }

        [Fact]
        public async Task HandleAsync_AllValid_RemovesCuisineAndReturnsSuccess()
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
                fixture.CuisineRepositoryMock.VerifyRemoveAsync(fixture.Cuisine.Id, Times.Once);
            }
        }

        protected override
            CommandHandlerTestFixtureBase<RemoveCuisineCommandHandler, RemoveCuisineCommand, bool> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<RemoveCuisineCommandHandler,
            RemoveCuisineCommand, bool>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                RestaurantRepositoryMock = new RestaurantRepositoryMock(MockBehavior.Strict);
                CuisineRepositoryMock = new CuisineRepositoryMock(MockBehavior.Strict);
            }

            public Cuisine Cuisine { get; private set; }

            public RestaurantRepositoryMock RestaurantRepositoryMock { get; }

            public CuisineRepositoryMock CuisineRepositoryMock { get; }

            public override RemoveCuisineCommandHandler CreateTestObject()
            {
                return new RemoveCuisineCommandHandler(
                    CuisineRepositoryMock.Object,
                    RestaurantRepositoryMock.Object
                );
            }

            public override RemoveCuisineCommand CreateSuccessfulCommand()
            {
                return new RemoveCuisineCommand(Cuisine.Id);
            }

            public void SetupRandomCuisine()
            {
                Cuisine = new CuisineBuilder()
                    .WithValidConstrains()
                    .Create();
            }

            public void SetupRestaurantRepositoryFindingNoRestaurantsWithCuisine()
            {
                RestaurantRepositoryMock.SetupFindByCuisineIdAsync(Cuisine.Id)
                    .ReturnsAsync(Enumerable.Empty<Restaurant>());
            }

            public void SetupCuisineRepositoryRemovingCuisine()
            {
                CuisineRepositoryMock.SetupRemoveAsync(Cuisine.Id)
                    .Returns(Task.CompletedTask);
            }

            public override void SetupForSuccessfulCommandExecution(Role? role)
            {
                SetupRandomCuisine();
                SetupRestaurantRepositoryFindingNoRestaurantsWithCuisine();
                SetupCuisineRepositoryRemovingCuisine();
            }
        }
    }
}
