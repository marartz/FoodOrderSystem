using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Cuisines;
using Gastromio.Core.Domain.Model.PaymentMethods;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Common;
using Gastromio.Domain.TestKit.Domain.Model.Restaurants;
using Gastromio.Domain.TestKit.Domain.Model.Users;
using Xunit;

namespace Gastromio.Domain.Tests.Domain.Model.Restaurants
{
    public class RestaurantFactoryTests
    {
        private readonly Fixture fixture;

        public RestaurantFactoryTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void Create_ValidParameters_CreatesRestaurant()
        {
            // Arrange
            fixture.SetupValidParameters();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = CallCreate(testObject);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                result?.Value.Should().BeOfType<Restaurant>();
                result?.Value?.Name.Should().Be(fixture.Name);
                result?.Value?.Address.Should().BeEquivalentTo(fixture.Address);
                result?.Value?.ContactInfo.Should().BeEquivalentTo(fixture.ContactInfo);
                result?.Value?.RegularOpeningDays.Select(en => en.Value)
                    .Should().BeEquivalentTo(fixture.RegularOpeningDays);
                result?.Value?.DeviatingOpeningDays.Select(en => en.Value)
                    .Should().BeEquivalentTo(fixture.DeviatingOpeningDays);
                result?.Value?.PickupInfo.Should().BeEquivalentTo(fixture.PickupInfo);
                result?.Value?.DeliveryInfo.Should().BeEquivalentTo(fixture.DeliveryInfo);
                result?.Value?.ReservationInfo.Should().BeEquivalentTo(fixture.ReservationInfo);
                result?.Value?.HygienicHandling.Should().Be(fixture.HygienicHandling);
                result?.Value?.Cuisines.Should().BeEquivalentTo(fixture.Cuisines);
                result?.Value?.PaymentMethods.Should().BeEquivalentTo(fixture.PaymentMethods);
                result?.Value?.Administrators.Should().BeEquivalentTo(fixture.Administrators);
                result?.Value?.CreatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                result?.Value?.CreatedBy.Should().Be(fixture.CreatedBy);
                result?.Value?.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                result?.Value?.UpdatedBy.Should().Be(fixture.CreatedBy);
            }
        }

        [Fact]
        public void Create_ValidParametersExceptName_ReturnsFailure()
        {
            // Arrange
            fixture.SetupValidParametersExceptName();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = CallCreate(testObject);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void Create_ValidParametersExceptAddress_ReturnsFailure()
        {
            // Arrange
            fixture.SetupValidParametersExceptAddress();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = CallCreate(testObject);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void Create_ValidParametersExceptContactInfo_ReturnsFailure()
        {
            // Arrange
            fixture.SetupValidParametersExceptContactInfo();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = CallCreate(testObject);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void Create_ValidParametersExceptRegularOpeningDays_ReturnsFailure()
        {
            // Arrange
            fixture.SetupValidParametersExceptRegularOpeningDays();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = CallCreate(testObject);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void Create_ValidParametersExceptDeviatingOpeningDays_ReturnsFailure()
        {
            // Arrange
            fixture.SetupValidParametersExceptDeviatingOpeningDays();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = CallCreate(testObject);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void Create_ValidParametersExceptPickupInfo_ReturnsFailure()
        {
            // Arrange
            fixture.SetupValidParametersExceptPickupInfo();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = CallCreate(testObject);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void Create_ValidParametersExceptDeliveryInfo_ReturnsFailure()
        {
            // Arrange
            fixture.SetupValidParametersExceptDeliveryInfo();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = CallCreate(testObject);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        private Result<Restaurant> CallCreate(IRestaurantFactory testObject)
        {
            return testObject.Create(
                fixture.Name,
                fixture.Address,
                fixture.ContactInfo,
                fixture.RegularOpeningDays,
                fixture.DeviatingOpeningDays,
                fixture.PickupInfo,
                fixture.DeliveryInfo,
                fixture.ReservationInfo,
                fixture.HygienicHandling,
                fixture.Cuisines,
                fixture.PaymentMethods,
                fixture.Administrators,
                fixture.CreatedBy
            );
        }

        private sealed class Fixture
        {
            public string Name { get; private set; }

            public Address Address { get; private set; }

            public ContactInfo ContactInfo { get; private set; }

            public List<RegularOpeningDay> RegularOpeningDays { get; private set; }

            public List<DeviatingOpeningDay> DeviatingOpeningDays { get; private set; }

            public PickupInfo PickupInfo { get; private set; }

            public DeliveryInfo DeliveryInfo { get; private set; }

            public ReservationInfo ReservationInfo { get; private set; }

            public string HygienicHandling { get; private set; }

            public HashSet<CuisineId> Cuisines { get; private set; }

            public HashSet<PaymentMethodId> PaymentMethods { get; private set; }

            public HashSet<UserId> Administrators { get; private set; }

            public UserId CreatedBy { get; private set; }

            public void SetupValidParameters()
            {
                Name = RandomStringBuilder.BuildWithLength(20);
                Address = new AddressBuilder()
                    .WithValidConstrains()
                    .Create();
                ContactInfo = new ContactInfoBuilder()
                    .WithValidConstrains()
                    .Create();
                RegularOpeningDays = new RegularOpeningDayBuilder()
                    .WithDayOfWeek(0)
                    .WithOpeningPeriods(new []
                    {
                        new OpeningPeriod(TimeSpan.FromHours(16), TimeSpan.FromHours(22))
                    })
                    .CreateMany(1).ToList();
                DeviatingOpeningDays = new DeviatingOpeningDayBuilder()
                    .WithDate(Date.Today.AddDays(1))
                    .WithStatus(DeviatingOpeningDayStatus.Open)
                    .WithOpeningPeriods(new []
                    {
                        new OpeningPeriod(TimeSpan.FromHours(16), TimeSpan.FromHours(22))
                    })
                    .CreateMany(1).ToList();
                PickupInfo = new PickupInfoBuilder()
                    .WithValidConstrains()
                    .Create();
                DeliveryInfo = new DeliveryInfoBuilder()
                    .WithValidConstrains()
                    .Create();
                ReservationInfo = new ReservationInfoBuilder()
                    .Create();
                HygienicHandling = RandomStringBuilder.BuildWithLength(20);
                Cuisines = new HashSet<CuisineId> {new CuisineId(Guid.NewGuid())};
                PaymentMethods = new HashSet<PaymentMethodId> {new PaymentMethodId(Guid.NewGuid())};
                Administrators = new HashSet<UserId> {new UserId(Guid.NewGuid())};
                CreatedBy = new UserIdBuilder().Create();
            }

            public void SetupValidParametersExceptName()
            {
                SetupValidParameters();
                Name = RandomStringBuilder.BuildWithLength(101);
            }

            public void SetupValidParametersExceptAddress()
            {
                SetupValidParameters();
                Address = new AddressBuilder()
                    .WithZipCode("abcde")
                    .WithValidConstrains()
                    .Create();
            }

            public void SetupValidParametersExceptContactInfo()
            {
                SetupValidParameters();
                ContactInfo = new ContactInfoBuilder()
                    .WithPhone("abcdef")
                    .WithValidConstrains()
                    .Create();
            }

            public void SetupValidParametersExceptRegularOpeningDays()
            {
                SetupValidParameters();
                RegularOpeningDays = new List<RegularOpeningDay>
                {
                    new RegularOpeningDayBuilder()
                        .WithDayOfWeek(0)
                        .WithOpeningPeriods(new[]
                        {
                            new OpeningPeriod(TimeSpan.FromHours(2), TimeSpan.FromHours(3))
                        })
                        .Create()
                };
            }

            public void SetupValidParametersExceptDeviatingOpeningDays()
            {
                SetupValidParameters();
                DeviatingOpeningDays = new List<DeviatingOpeningDay>
                {
                    new DeviatingOpeningDayBuilder()
                        .WithDate(Date.Today.AddDays(1))
                        .WithStatus(DeviatingOpeningDayStatus.Open)
                        .WithOpeningPeriods(new[]
                        {
                            new OpeningPeriod(TimeSpan.FromHours(2), TimeSpan.FromHours(3))
                        })
                        .Create()
                };
            }

            public void SetupValidParametersExceptPickupInfo()
            {
                SetupValidParameters();
                PickupInfo = new PickupInfoBuilder()
                    .WithAverageTime(1)
                    .WithValidConstrains()
                    .Create();
            }

            public void SetupValidParametersExceptDeliveryInfo()
            {
                SetupValidParameters();
                DeliveryInfo = new DeliveryInfoBuilder()
                    .WithAverageTime(1)
                    .WithValidConstrains()
                    .Create();
            }

            public RestaurantFactory CreateTestObject()
            {
                return new RestaurantFactory();
            }
        }
    }
}
