using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.ChangeCuisine;
using Gastromio.Core.Domain.Model.Cuisines;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Domain.Model.Cuisines;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.ChangeCuisine
{
    public class ChangeCuisineCommandHandlerTests : CommandHandlerTestBase<ChangeCuisineCommandHandler,
        ChangeCuisineCommand, bool>
    {
        private readonly Fixture fixture;

        public ChangeCuisineCommandHandlerTests()
        {
            fixture = new Fixture(Role.SystemAdmin);
        }

        [Fact]
        public async Task HandleAsync_CuisineNotKnown_ReturnsFailure()
        {
            // Arrange
            fixture.SetupRandomCuisine();
            fixture.SetupCuisineRepositoryNotFindingCuisineById();

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
        public async Task HandleAsync_AllValid_StoresCuisineReturnsSuccess()
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
                fixture.CuisineRepositoryMock.VerifyStoreAsync(fixture.Cuisine, Times.Once);
            }
        }

        protected override
            CommandHandlerTestFixtureBase<ChangeCuisineCommandHandler, ChangeCuisineCommand, bool> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<ChangeCuisineCommandHandler,
            ChangeCuisineCommand, bool>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                CuisineRepositoryMock = new CuisineRepositoryMock(MockBehavior.Strict);
            }

            public Cuisine Cuisine { get; private set; }

            public CuisineRepositoryMock CuisineRepositoryMock { get; }

            public override ChangeCuisineCommandHandler CreateTestObject()
            {
                return new ChangeCuisineCommandHandler(
                    CuisineRepositoryMock.Object
                );
            }

            public override ChangeCuisineCommand CreateSuccessfulCommand()
            {
                return new ChangeCuisineCommand(Cuisine.Id, "test");
            }

            public void SetupRandomCuisine()
            {
                Cuisine = new CuisineBuilder()
                    .WithValidConstrains()
                    .Create();
            }

            public void SetupCuisineRepositoryFindingCuisineById()
            {
                CuisineRepositoryMock.SetupFindByCuisineIdAsync(Cuisine.Id)
                    .ReturnsAsync(Cuisine);
            }

            public void SetupCuisineRepositoryNotFindingCuisineById()
            {
                CuisineRepositoryMock.SetupFindByCuisineIdAsync(Cuisine.Id)
                    .ReturnsAsync((Cuisine)null);
            }

            public void SetupCuisineRepositoryStoringCuisine()
            {
                CuisineRepositoryMock.Setup(m => m.StoreAsync(Cuisine, It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);
            }

            public override void SetupForSuccessfulCommandExecution(Role? role)
            {
                SetupRandomCuisine();
                SetupCuisineRepositoryFindingCuisineById();
                SetupCuisineRepositoryStoringCuisine();
            }
        }
    }
}
