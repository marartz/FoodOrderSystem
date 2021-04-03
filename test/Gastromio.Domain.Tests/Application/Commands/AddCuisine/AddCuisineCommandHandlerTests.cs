using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Application.Commands.AddCuisine;
using Gastromio.Core.Application.DTOs;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Cuisines;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Application.Ports.Persistence;
using Gastromio.Domain.TestKit.Domain.Model.Cuisines;
using Moq;
using Xunit;

namespace Gastromio.Domain.Tests.Application.Commands.AddCuisine
{
    public class AddCuisineCommandHandlerTests : CommandHandlerTestBase<AddCuisineCommandHandler,
        AddCuisineCommand, CuisineDTO>
    {
        private readonly Fixture fixture;

        public AddCuisineCommandHandlerTests()
        {
            fixture = new Fixture(Role.SystemAdmin);
        }

        [Fact]
        public async Task HandleAsync_CuisineToCreateNull_ReturnsFailure()
        {
            // Arrange
            fixture.SetupCuisineRepositoryFindingCuisineByName();

            var testObject = fixture.CreateTestObject();
            var command = new AddCuisineCommand(null);

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
        public async Task HandleAsync_CuisineToCreateEmpty_ReturnsFailure()
        {
            // Arrange
            fixture.SetupCuisineRepositoryFindingCuisineByName();

            var testObject = fixture.CreateTestObject();
            var command = new AddCuisineCommand("");

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
        public async Task HandleAsync_CuisineAlreadyKnown_ReturnsFailure()
        {
            // Arrange
            fixture.SetupCuisineRepositoryFindingCuisineByName();

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
        public async Task HandleAsync_AllValid_CreatesCuisineAndReturnsSuccess()
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
                fixture.CuisineRepositoryMock.VerifyStoreAsync(fixture.CreatedCuisine, Times.Once);
            }
        }

        protected override
            CommandHandlerTestFixtureBase<AddCuisineCommandHandler, AddCuisineCommand, CuisineDTO> FixtureBase
        {
            get { return fixture; }
        }

        private sealed class Fixture : CommandHandlerTestFixtureBase<AddCuisineCommandHandler,
            AddCuisineCommand, CuisineDTO>
        {
            public Fixture(Role? minimumRole) : base(minimumRole)
            {
                CuisineFactoryMock = new CuisineFactoryMock(MockBehavior.Strict);
                CuisineRepositoryMock = new CuisineRepositoryMock(MockBehavior.Strict);
            }

            public CuisineFactoryMock CuisineFactoryMock { get; }

            public CuisineRepositoryMock CuisineRepositoryMock { get; }

            public Cuisine CreatedCuisine { get; private set; }

            public override AddCuisineCommandHandler CreateTestObject()
            {
                return new AddCuisineCommandHandler(
                    CuisineFactoryMock.Object,
                    CuisineRepositoryMock.Object
                );
            }

            public override AddCuisineCommand CreateSuccessfulCommand()
            {
                return new AddCuisineCommand("test");
            }

            public void SetupCuisineRepositoryFindingCuisineByName()
            {
                var cuisine = new CuisineBuilder()
                    .WithName("test")
                    .Create();

                CuisineRepositoryMock.SetupFindByNameAsync("test")
                    .ReturnsAsync(cuisine);
            }

            public void SetupCuisineRepositoryNotFindingCuisineByName()
            {
                CuisineRepositoryMock.SetupFindByNameAsync("test")
                    .ReturnsAsync((Cuisine) null);
            }

            public void SetupCuisineRepositoryStoringCuisine()
            {
                CuisineRepositoryMock.Setup(m => m.StoreAsync(It.IsAny<Cuisine>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);
            }

            public void SetupCuisineFactoryCreatingCuisine()
            {
                CreatedCuisine = new CuisineBuilder()
                    .WithName("test")
                    .Create();

                CuisineFactoryMock.SetupCreate("test", UserWithMinimumRole.Id)
                    .Returns(SuccessResult<Cuisine>.Create(CreatedCuisine));
            }

            public override void SetupForSuccessfulCommandExecution(Role? role)
            {
                SetupCuisineRepositoryNotFindingCuisineByName();
                SetupCuisineFactoryCreatingCuisine();
                SetupCuisineRepositoryStoringCuisine();
            }
        }
    }
}
