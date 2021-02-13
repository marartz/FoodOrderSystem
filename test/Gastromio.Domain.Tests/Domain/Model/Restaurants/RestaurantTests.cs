using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Domain.TestKit.Common;
using Gastromio.Domain.TestKit.Domain.Model.Restaurants;
using Gastromio.Domain.TestKit.Domain.Model.Users;
using Xunit;

namespace Gastromio.Domain.Tests.Domain.Model.Restaurants
{
    public class RestaurantTests
    {
        private readonly Fixture fixture;

        public RestaurantTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void ChangeName_NameNull_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ChangeName(null, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeName_NameEmpty_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ChangeName(string.Empty, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeName_NameLength101_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var name = RandomStringBuilder.BuildWithLength(101);

            // Act
            var result = testObject.ChangeName(name, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeName_NameLength100_ChangesNameAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var name = RandomStringBuilder.BuildWithLength(100);

            // Act
            var result = testObject.ChangeName(name, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.Name.Should().Be(name);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ChangeAddress_StreetNull_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var address = new AddressBuilder()
                .WithStreet(null)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeAddress(address, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeAddress_StreetEmpty_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var address = new AddressBuilder()
                .WithStreet("")
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeAddress(address, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeAddress_StreetLength101_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var randomStreet = RandomStringBuilder.BuildWithLength(80, "ABCDEFGHIJKLM".ToCharArray());
            var randomHouseNumber = RandomStringBuilder.BuildWithLength(20, "0123456789".ToCharArray());

            var address = new AddressBuilder()
                .WithStreet($"{randomStreet} {randomHouseNumber}")
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeAddress(address, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeAddress_StreetInvalid_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var invalidStreet = RandomStringBuilder.BuildWithLength(100, "ABCDEFGHIJKLM".ToCharArray());

            var address = new AddressBuilder()
                .WithStreet(invalidStreet)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeAddress(address, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeAddress_ZipCodeNull_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var address = new AddressBuilder()
                .WithZipCode(null)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeAddress(address, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeAddress_ZipCodeEmpty_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var address = new AddressBuilder()
                .WithZipCode("")
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeAddress(address, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeAddress_ZipCodeLength6_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var address = new AddressBuilder()
                .WithZipCode("123456")
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeAddress(address, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeAddress_ZipCodeInvalid_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var address = new AddressBuilder()
                .WithZipCode("1234A")
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeAddress(address, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeAddress_AllValid_ChangesAddressAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var address = new AddressBuilder()
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeAddress(address, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.Address.Should().BeEquivalentTo(address);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ChangeContactInfo_PhoneNull_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var contactInfo = new ContactInfoBuilder()
                .WithPhone(null)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeContactInfo(contactInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeContactInfo_PhoneEmpty_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var contactInfo = new ContactInfoBuilder()
                .WithPhone("")
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeContactInfo(contactInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeContactInfo_PhoneInvalid_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var contactInfo = new ContactInfoBuilder()
                .WithPhone("abcd")
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeContactInfo(contactInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeContactInfo_FaxNull_ChangesContactInfoAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var contactInfo = new ContactInfoBuilder()
                .WithFax(null)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeContactInfo(contactInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.ContactInfo.Should().BeEquivalentTo(contactInfo);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ChangeContactInfo_FaxEmpty_ChangesContactInfoAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var contactInfo = new ContactInfoBuilder()
                .WithFax("")
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeContactInfo(contactInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.ContactInfo.Should().BeEquivalentTo(contactInfo);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ChangeContactInfo_FaxInvalid_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var contactInfo = new ContactInfoBuilder()
                .WithFax("abcd")
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeContactInfo(contactInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeContactInfo_WebSiteNull_ChangesContactInfoAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var contactInfo = new ContactInfoBuilder()
                .WithWebSite(null)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeContactInfo(contactInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.ContactInfo.Should().BeEquivalentTo(contactInfo);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ChangeContactInfo_WebSiteEmpty_ChangesContactInfoAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var contactInfo = new ContactInfoBuilder()
                .WithWebSite("")
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeContactInfo(contactInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.ContactInfo.Should().BeEquivalentTo(contactInfo);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ChangeContactInfo_WebSiteInvalid_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var contactInfo = new ContactInfoBuilder()
                .WithWebSite("abcd")
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeContactInfo(contactInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeContactInfo_EmailAddressNull_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var contactInfo = new ContactInfoBuilder()
                .WithEmailAddress(null)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeContactInfo(contactInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeContactInfo_EmailAddressEmpty_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var contactInfo = new ContactInfoBuilder()
                .WithEmailAddress("")
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeContactInfo(contactInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeContactInfo_EmailAddressInvalid_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var contactInfo = new ContactInfoBuilder()
                .WithEmailAddress("abcd")
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeContactInfo(contactInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeContactInfo_MobileNull_ChangesContactInfoAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var contactInfo = new ContactInfoBuilder()
                .WithMobile(null)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeContactInfo(contactInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.ContactInfo.Should().BeEquivalentTo(contactInfo);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ChangeContactInfo_MobileEmpty_ChangesContactInfoAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var contactInfo = new ContactInfoBuilder()
                .WithMobile("")
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeContactInfo(contactInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.ContactInfo.Should().BeEquivalentTo(contactInfo);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ChangeContactInfo_MobileInvalid_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var contactInfo = new ContactInfoBuilder()
                .WithMobile("abcd")
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeContactInfo(contactInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeContactInfo_AllValid_ChangesContactInfoAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var contactInfo = new ContactInfoBuilder()
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeContactInfo(contactInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.ContactInfo.Should().BeEquivalentTo(contactInfo);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void AddRegularOpeningPeriod_DayNotKnown_AddsDayAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupEmptyRegularOpeningDays();
            var testObject = fixture.CreateTestObject();

            var openingPeriod = new OpeningPeriodBuilder()
                .WithValidConstrains()
                .Create();

            // Act
            var result =
                testObject.AddRegularOpeningPeriod(0, openingPeriod, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.RegularOpeningDays.TryGetValue(0, out var openingDay).Should()
                    .BeTrue();
                openingDay.Should().NotBeNull();
                openingDay?.DayOfWeek.Should().Be(0);
                openingDay?.OpeningPeriods.Should().BeEquivalentTo(openingPeriod);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void AddRegularOpeningPeriod_OtherDayKnown_AddsDayAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRegularOpeningDayNextMondayWithOneDefaultPeriod();
            var testObject = fixture.CreateTestObject();

            var openingPeriod = new OpeningPeriodBuilder()
                .WithValidConstrains()
                .Create();

            // Act
            var result =
                testObject.AddRegularOpeningPeriod(1, openingPeriod, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.RegularOpeningDays.TryGetValue(0, out var openingDayMonday)
                    .Should().BeTrue();
                openingDayMonday.Should().NotBeNull();
                openingDayMonday?.DayOfWeek.Should().Be(0);
                openingDayMonday?.OpeningPeriods.Should().HaveCount(1);
                testObject.RegularOpeningDays.TryGetValue(1, out var openingDayTuesday)
                    .Should().BeTrue();
                openingDayTuesday.Should().NotBeNull();
                openingDayTuesday?.DayOfWeek.Should().Be(1);
                openingDayTuesday?.OpeningPeriods.Should().BeEquivalentTo(openingPeriod);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void AddRegularOpeningPeriod_DayKnownAndPeriodOverlaps_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRegularOpeningDayNextMondayWithOneDefaultPeriod();
            var testObject = fixture.CreateTestObject();

            var openingPeriod = new OpeningPeriodBuilder()
                .WithStart(TimeSpan.FromHours(6))
                .WithEnd(TimeSpan.FromHours(23))
                .Create();

            // Act
            var result =
                testObject.AddRegularOpeningPeriod(0, openingPeriod, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void AddRegularOpeningPeriod_DayKnownAndPeriodDoesNotOverlap_AddsPeriodReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRegularOpeningDayNextMondayWithOneDefaultPeriod();
            var testObject = fixture.CreateTestObject();

            var openingPeriod = new OpeningPeriodBuilder()
                .WithStart(TimeSpan.FromHours(6))
                .WithEnd(TimeSpan.FromHours(7))
                .Create();

            var expectedPeriods = testObject.RegularOpeningDays.First().Value.OpeningPeriods
                .ToList();

            expectedPeriods.Add(openingPeriod);

            // Act
            var result =
                testObject.AddRegularOpeningPeriod(0, openingPeriod, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.RegularOpeningDays.TryGetValue(0, out var openingDay).Should()
                    .BeTrue();
                openingDay.Should().NotBeNull();
                openingDay?.DayOfWeek.Should().Be(0);
                openingDay?.OpeningPeriods.Should().BeEquivalentTo(expectedPeriods);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void RemoveRegularOpeningPeriod_DayNotKnown_DoesNothingAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupEmptyRegularOpeningDays();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.RemoveRegularOpeningPeriod(0, TimeSpan.FromHours(8),
                fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.UpdatedOn.Should().NotBeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().NotBe(fixture.ChangedBy);
            }
        }

        [Fact]
        public void RemoveRegularOpeningPeriod_OtherDayKnownWithOnePeriod_DoesNothingAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupEmptyRegularOpeningDays();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.RemoveRegularOpeningPeriod(1, TimeSpan.FromHours(8),
                fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.UpdatedOn.Should().NotBeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().NotBe(fixture.ChangedBy);
            }
        }

        [Fact]
        public void RemoveRegularOpeningPeriod_DayKnownWithOnePeriod_RemovesDayAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRegularOpeningDayNextMondayWithOneDefaultPeriod();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.RemoveRegularOpeningPeriod(0, TimeSpan.FromHours(16.5),
                fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.RegularOpeningDays.Should().HaveCount(0);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void RemoveRegularOpeningPeriod_DayKnownWithTwoPeriods_RemovesPeriodAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRegularOpeningDayNextMondayWithTwoPeriods();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.RemoveRegularOpeningPeriod(0, TimeSpan.FromHours(12),
                fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.RegularOpeningDays.TryGetValue(0, out var openingDay).Should()
                    .BeTrue();
                openingDay?.DayOfWeek.Should().Be(0);
                openingDay?.OpeningPeriods.Should()
                    .BeEquivalentTo(new OpeningPeriod(TimeSpan.FromHours(16), TimeSpan.FromHours(22)));
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void AddDeviatingOpeningDay_DayNotKnown_AddsDayAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupDeviatingDayDateOnMonday();
            fixture.SetupEmptyDeviatingOpeningDays();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.AddDeviatingOpeningDay(fixture.DeviatingDayDate, DeviatingOpeningDayStatus.Open,
                fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.DeviatingOpeningDays.TryGetValue(fixture.DeviatingDayDate, out var openingDay).Should()
                    .BeTrue();
                openingDay?.Date.Should().Be(fixture.DeviatingDayDate);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void AddDeviatingOpeningDay_DayKnown_DoesNotChangeAnythingAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupDeviatingDayDateOnMonday();
            fixture.SetupDeviatingOpeningDayWithOneDefaultPeriod();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.AddDeviatingOpeningDay(fixture.DeviatingDayDate, DeviatingOpeningDayStatus.Open,
                fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.DeviatingOpeningDays.TryGetValue(fixture.DeviatingDayDate, out var openingDay).Should()
                    .BeTrue();
                openingDay?.Date.Should().Be(fixture.DeviatingDayDate);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ChangeDeviatingOpeningDayStatus_DayNotKnown_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupDeviatingDayDateOnMonday();
            fixture.SetupEmptyDeviatingOpeningDays();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ChangeDeviatingOpeningDayStatus(fixture.DeviatingDayDate,
                DeviatingOpeningDayStatus.Open,
                fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeDeviatingOpeningDayStatus_DayKnownAndHasPeriods_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupDeviatingDayDateOnMonday();
            fixture.SetupDeviatingOpeningDayWithOneDefaultPeriod();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ChangeDeviatingOpeningDayStatus(fixture.DeviatingDayDate,
                DeviatingOpeningDayStatus.Open,
                fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeDeviatingOpeningDayStatus_DayKnown_ChangesStatusAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupDeviatingDayDateOnMonday();
            fixture.SetupDeviatingOpeningDayWithoutPeriods();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.ChangeDeviatingOpeningDayStatus(fixture.DeviatingDayDate,
                DeviatingOpeningDayStatus.FullyBooked,
                fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.DeviatingOpeningDays.TryGetValue(fixture.DeviatingDayDate, out var openingDay).Should()
                    .BeTrue();
                openingDay?.Date.Should().Be(fixture.DeviatingDayDate);
                openingDay?.Status.Should().Be(DeviatingOpeningDayStatus.FullyBooked);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void RemoveDeviatingOpeningDay_DayNotKnown_DoesNotChangeAnythingAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupDeviatingDayDateOnMonday();
            fixture.SetupEmptyDeviatingOpeningDays();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.RemoveDeviatingOpeningDay(fixture.DeviatingDayDate, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.DeviatingOpeningDays.Should().BeEmpty();
                testObject.UpdatedOn.Should().NotBeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().NotBe(fixture.ChangedBy);
            }
        }

        [Fact]
        public void RemoveDeviatingOpeningDay_DayKnown_RemovesDayAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupDeviatingDayDateOnMonday();
            fixture.SetupDeviatingOpeningDayWithOneDefaultPeriod();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.RemoveDeviatingOpeningDay(fixture.DeviatingDayDate, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.DeviatingOpeningDays.Should().BeEmpty();
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void AddDeviatingOpeningPeriod_DayNotKnown_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupDeviatingDayDateOnMonday();
            fixture.SetupEmptyDeviatingOpeningDays();
            var testObject = fixture.CreateTestObject();

            var openingPeriod = new OpeningPeriodBuilder()
                .WithValidConstrains()
                .Create();

            // Act
            var result =
                testObject.AddDeviatingOpeningPeriod(fixture.DeviatingDayDate, openingPeriod, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void AddDeviatingOpeningPeriod_DayKnownAndPeriodOverlaps_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupDeviatingDayDateOnMonday();
            fixture.SetupDeviatingOpeningDayWithOneDefaultPeriod();
            var testObject = fixture.CreateTestObject();

            var openingPeriod = new OpeningPeriodBuilder()
                .WithStart(TimeSpan.FromHours(6))
                .WithEnd(TimeSpan.FromHours(23))
                .Create();

            // Act
            var result =
                testObject.AddDeviatingOpeningPeriod(fixture.DeviatingDayDate, openingPeriod, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void AddDeviatingOpeningPeriod_DayKnownAndPeriodDoesNotOverlap_AddsPeriodReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupDeviatingDayDateOnMonday();
            fixture.SetupDeviatingOpeningDayWithOneDefaultPeriod();
            var testObject = fixture.CreateTestObject();

            var openingPeriod = new OpeningPeriodBuilder()
                .WithStart(TimeSpan.FromHours(6))
                .WithEnd(TimeSpan.FromHours(7))
                .Create();

            var expectedPeriods = testObject.DeviatingOpeningDays.First().Value.OpeningPeriods
                .ToList();

            expectedPeriods.Add(openingPeriod);

            // Act
            var result =
                testObject.AddDeviatingOpeningPeriod(fixture.DeviatingDayDate, openingPeriod, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.DeviatingOpeningDays.TryGetValue(fixture.DeviatingDayDate, out var openingDay).Should()
                    .BeTrue();
                openingDay.Should().NotBeNull();
                openingDay?.Date.Should().Be(fixture.DeviatingDayDate);
                openingDay?.OpeningPeriods.Should().BeEquivalentTo(expectedPeriods);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void RemoveDeviatingOpeningPeriod_DayNotKnown_DoesNothingAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupDeviatingDayDateOnMonday();
            fixture.SetupEmptyDeviatingOpeningDays();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.RemoveDeviatingOpeningPeriod(fixture.DeviatingDayDate, TimeSpan.FromHours(8),
                fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.UpdatedOn.Should().NotBeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().NotBe(fixture.ChangedBy);
            }
        }

        [Fact]
        public void RemoveDeviatingOpeningPeriod_OtherDayKnownWithOnePeriod_DoesNothingAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupDeviatingDayDateOnMonday();
            fixture.SetupEmptyDeviatingOpeningDays();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.RemoveDeviatingOpeningPeriod(fixture.DeviatingDayDate.AddDays(1),
                TimeSpan.FromHours(8), fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.UpdatedOn.Should().NotBeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().NotBe(fixture.ChangedBy);
            }
        }

        [Fact]
        public void RemoveDeviatingOpeningPeriod_DayKnownWithOnePeriod_RemovesDayAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupDeviatingDayDateOnMonday();
            fixture.SetupDeviatingOpeningDayWithOneDefaultPeriod();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.RemoveDeviatingOpeningPeriod(fixture.DeviatingDayDate, TimeSpan.FromHours(16.5),
                fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.DeviatingOpeningDays.TryGetValue(fixture.DeviatingDayDate, out var openingDay).Should()
                    .BeTrue();
                openingDay.Should().NotBeNull();
                openingDay?.Status.Should().Be(DeviatingOpeningDayStatus.Closed);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void RemoveDeviatingOpeningPeriod_DayKnownWithTwoPeriods_RemovesPeriodAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupDeviatingDayDateOnMonday();
            fixture.SetupDeviatingOpeningDayWithTwoPeriods();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.RemoveDeviatingOpeningPeriod(fixture.DeviatingDayDate, TimeSpan.FromHours(12),
                fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.DeviatingOpeningDays.TryGetValue(fixture.DeviatingDayDate, out var openingDay).Should()
                    .BeTrue();
                openingDay?.Date.Should().Be(fixture.DeviatingDayDate);
                openingDay?.OpeningPeriods.Should()
                    .BeEquivalentTo(new OpeningPeriod(TimeSpan.FromHours(16), TimeSpan.FromHours(22)));
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void RemoveAllOpeningDays_RemovesAllOpeningDays()
        {
            // Arrange
            fixture.SetupChangedBy();
            fixture.SetupRegularOpeningDayNextMondayWithTwoPeriods();
            fixture.SetupDeviatingDayDateOnMonday();
            fixture.SetupDeviatingOpeningDayWithTwoPeriods();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.RemoveAllOpeningDays(fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.RegularOpeningDays.Should().BeEmpty();
                testObject.DeviatingOpeningDays.Should().BeEmpty();
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void IsOrderPossible_OnlyPhone_ReturnsFalse()
        {
            // Arrange
            fixture.SetupOnlyPhoneOrderMode();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.IsOrderPossibleAt(DateTimeOffset.Now);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsOrderPossible_AtNextShift_NoRegular_NoDeviating_ReturnsFalse()
        {
            // Arrange
            fixture.SetupAtNextShiftOrderMode();
            fixture.SetupEmptyRegularOpeningDays();
            fixture.SetupEmptyDeviatingOpeningDays();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.IsOrderPossibleAt(DateTimeOffset.Now);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsOrderPossible_AtNextShift_OutsideEarlyRegular_OnDeviatingWithoutPeriods_ReturnsFalse()
        {
            // Arrange
            fixture.SetupAtNextShiftOrderMode();
            fixture.SetupRegularOpeningDayNextMondayWithOneEarlyPeriod();
            fixture.SetupDeviatingDayDateOnMonday();
            fixture.SetupDeviatingOpeningDayWithoutPeriods();
            var testObject = fixture.CreateTestObject();

            var orderDateTime = fixture.NextMonday.ToLocalDateTimeOffset().AddHours(18);

            // Act
            var result = testObject.IsOrderPossibleAt(orderDateTime);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsOrderPossible_AtNextShift_OutsideEarlyRegular_OnDeviatingJustBeforePeriodStart_ReturnsFalse()
        {
            // Arrange
            fixture.SetupAtNextShiftOrderMode();
            fixture.SetupRegularOpeningDayNextMondayWithOneEarlyPeriod();
            fixture.SetupDeviatingDayDateOnMonday();
            fixture.SetupDeviatingOpeningDayWithOneDefaultPeriod();
            var testObject = fixture.CreateTestObject();

            var orderDateTime = fixture.NextMonday.ToLocalDateTimeOffset().AddHours(16).AddMinutes(29);

            // Act
            var result = testObject.IsOrderPossibleAt(orderDateTime);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsOrderPossible_AtNextShift_OutsideEarlyRegular_OnDeviatingJustAfterPeriodEnd_ReturnsFalse()
        {
            // Arrange
            fixture.SetupAtNextShiftOrderMode();
            fixture.SetupRegularOpeningDayNextMondayWithOneEarlyPeriod();
            fixture.SetupDeviatingDayDateOnMonday();
            fixture.SetupDeviatingOpeningDayWithOneDefaultPeriod();
            var testObject = fixture.CreateTestObject();

            var orderDateTime = fixture.DeviatingDayDate.ToLocalDateTimeOffset().AddHours(22).AddMinutes(31);

            // Act
            var result = testObject.IsOrderPossibleAt(orderDateTime);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsOrderPossible_AtNextShift_InsideEarlyRegular_OnDeviatingDayButOutsidePeriod_ReturnsFalse()
        {
            // Arrange
            fixture.SetupAtNextShiftOrderMode();
            fixture.SetupRegularOpeningDayNextMondayWithOneEarlyPeriod();
            fixture.SetupDeviatingDayDateOnMonday();
            fixture.SetupDeviatingOpeningDayWithOneDefaultPeriod();
            var testObject = fixture.CreateTestObject();

            var orderDateTime = fixture.NextMonday.ToLocalDateTimeOffset().AddHours(6).AddMinutes(30);

            // Act
            var result = testObject.IsOrderPossibleAt(orderDateTime);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsOrderPossible_AtNextShift_JustBeforeRegularStart_NotOnDeviatingDay_ReturnsFalse()
        {
            // Arrange
            fixture.SetupAtNextShiftOrderMode();
            fixture.SetupRegularOpeningDayNextMondayWithOneEarlyPeriod();
            fixture.SetupDeviatingDayDateOnTuesday();
            fixture.SetupDeviatingOpeningDayWithOneDefaultPeriod();
            var testObject = fixture.CreateTestObject();

            var orderDateTime = fixture.NextMonday.ToLocalDateTimeOffset().AddHours(5).AddMinutes(59);

            // Act
            var result = testObject.IsOrderPossibleAt(orderDateTime);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsOrderPossible_AtNextShift_JustAfterRegularEnd_NotOnDeviatingDay_ReturnsFalse()
        {
            // Arrange
            fixture.SetupAtNextShiftOrderMode();
            fixture.SetupRegularOpeningDayNextMondayWithOneEarlyPeriod();
            fixture.SetupDeviatingDayDateOnTuesday();
            fixture.SetupDeviatingOpeningDayWithOneDefaultPeriod();
            var testObject = fixture.CreateTestObject();

            var orderDateTime = fixture.NextMonday.ToLocalDateTimeOffset().AddHours(7).AddMinutes(1);

            // Act
            var result = testObject.IsOrderPossibleAt(orderDateTime);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsOrderPossible_AtNextShift_NowClosed_OpenedInAnHour_ReturnsTrue()
        {
            // Arrange
            fixture.SetupAtNextShiftOrderMode();
            fixture.SetupRegularOpeningDayTodayWithPeriodInAnHourForRestOfDay();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.IsOrderPossibleAt(DateTimeOffset.Now.AddHours(1).AddMinutes(1));

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsOrderPossible_AtNextShift_NowOpenedFor2Min_OpenedInAnHour_ReturnsTrue()
        {
            // Arrange
            fixture.SetupAtNextShiftOrderMode();
            fixture.SetupRegularOpeningDayTodayWithPeriodNowFor2MinutesAndInAnHourForRestOfDay();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.IsOrderPossibleAt(DateTimeOffset.Now.AddHours(1).AddMinutes(1));

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsOrderPossible_AtNextShift_NowOpenedRestOfDay_ReturnsFalse()
        {
            // Arrange
            fixture.SetupAtNextShiftOrderMode();
            fixture.SetupRegularOpeningDayTodayWithPeriodNowForRestOfDay();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.IsOrderPossibleAt(DateTimeOffset.Now.AddHours(1).AddMinutes(1));

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsOrderPossible_Anytime_NoRegular_NoDeviating_ReturnsFalse()
        {
            // Arrange
            fixture.SetupAnytimeOrderMode();
            fixture.SetupEmptyRegularOpeningDays();
            fixture.SetupEmptyDeviatingOpeningDays();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.IsOrderPossibleAt(DateTimeOffset.Now);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsOrderPossible_Anytime_OutsideEarlyRegular_OnDeviatingWithoutPeriods_ReturnsFalse()
        {
            // Arrange
            fixture.SetupAnytimeOrderMode();
            fixture.SetupRegularOpeningDayNextMondayWithOneEarlyPeriod();
            fixture.SetupDeviatingDayDateOnMonday();
            fixture.SetupDeviatingOpeningDayWithoutPeriods();
            var testObject = fixture.CreateTestObject();

            var orderDateTime = fixture.NextMonday.ToLocalDateTimeOffset().AddHours(18);

            // Act
            var result = testObject.IsOrderPossibleAt(orderDateTime);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsOrderPossible_Anytime_OutsideEarlyRegular_OnDeviatingJustBeforePeriodStart_ReturnsFalse()
        {
            // Arrange
            fixture.SetupAnytimeOrderMode();
            fixture.SetupRegularOpeningDayNextMondayWithOneEarlyPeriod();
            fixture.SetupDeviatingDayDateOnMonday();
            fixture.SetupDeviatingOpeningDayWithOneDefaultPeriod();
            var testObject = fixture.CreateTestObject();

            var orderDateTime = fixture.NextMonday.ToLocalDateTimeOffset().AddHours(16).AddMinutes(29);

            // Act
            var result = testObject.IsOrderPossibleAt(orderDateTime);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsOrderPossible_Anytime_OutsideEarlyRegular_OnDeviatingAtPeriodStart_ReturnsTrue()
        {
            // Arrange
            fixture.SetupAnytimeOrderMode();
            fixture.SetupRegularOpeningDayNextMondayWithOneEarlyPeriod();
            fixture.SetupDeviatingDayDateOnMonday();
            fixture.SetupDeviatingOpeningDayWithOneDefaultPeriod();
            var testObject = fixture.CreateTestObject();

            var orderDateTime = fixture.NextMonday.ToLocalDateTimeOffset().AddHours(16).AddMinutes(30);

            // Act
            var result = testObject.IsOrderPossibleAt(orderDateTime);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsOrderPossible_Anytime_OutsideEarlyRegular_OnDeviatingAtPeriodEnd_ReturnsTrue()
        {
            // Arrange
            fixture.SetupAnytimeOrderMode();
            fixture.SetupRegularOpeningDayNextMondayWithOneEarlyPeriod();
            fixture.SetupDeviatingDayDateOnMonday();
            fixture.SetupDeviatingOpeningDayWithOneDefaultPeriod();
            var testObject = fixture.CreateTestObject();

            var orderDateTime = fixture.NextMonday.ToLocalDateTimeOffset().AddHours(22).AddMinutes(30);

            // Act
            var result = testObject.IsOrderPossibleAt(orderDateTime);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsOrderPossible_Anytime_OutsideEarlyRegular_OnDeviatingJustAfterPeriodEnd_ReturnsFalse()
        {
            // Arrange
            fixture.SetupAnytimeOrderMode();
            fixture.SetupRegularOpeningDayNextMondayWithOneEarlyPeriod();
            fixture.SetupDeviatingDayDateOnMonday();
            fixture.SetupDeviatingOpeningDayWithOneDefaultPeriod();
            var testObject = fixture.CreateTestObject();

            var orderDateTime = fixture.NextMonday.ToLocalDateTimeOffset().AddHours(22).AddMinutes(31);

            // Act
            var result = testObject.IsOrderPossibleAt(orderDateTime);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsOrderPossible_Anytime_InsideEarlyRegular_OnDeviatingDayButOutsidePeriod_ReturnsFalse()
        {
            // Arrange
            fixture.SetupAnytimeOrderMode();
            fixture.SetupRegularOpeningDayNextMondayWithOneEarlyPeriod();
            fixture.SetupDeviatingDayDateOnMonday();
            fixture.SetupDeviatingOpeningDayWithOneDefaultPeriod();
            var testObject = fixture.CreateTestObject();

            var orderDateTime = fixture.NextMonday.ToLocalDateTimeOffset().AddHours(6).AddMinutes(30);

            // Act
            var result = testObject.IsOrderPossibleAt(orderDateTime);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsOrderPossible_Anytime_JustBeforeRegularStart_NotOnDeviatingDay_ReturnsFalse()
        {
            // Arrange
            fixture.SetupAnytimeOrderMode();
            fixture.SetupRegularOpeningDayNextMondayWithOneEarlyPeriod();
            fixture.SetupDeviatingDayDateOnTuesday();
            fixture.SetupDeviatingOpeningDayWithOneDefaultPeriod();
            var testObject = fixture.CreateTestObject();

            var orderDateTime = fixture.NextMonday.ToLocalDateTimeOffset().AddHours(5).AddMinutes(59);

            // Act
            var result = testObject.IsOrderPossibleAt(orderDateTime);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsOrderPossible_Anytime_AtRegularStart_NotOnDeviatingDay_ReturnsTrue()
        {
            // Arrange
            fixture.SetupAnytimeOrderMode();
            fixture.SetupRegularOpeningDayNextMondayWithOneEarlyPeriod();
            fixture.SetupDeviatingDayDateOnTuesday();
            fixture.SetupDeviatingOpeningDayWithOneDefaultPeriod();
            var testObject = fixture.CreateTestObject();

            var orderDateTime = fixture.NextMonday.ToLocalDateTimeOffset().AddHours(6);

            // Act
            var result = testObject.IsOrderPossibleAt(orderDateTime);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsOrderPossible_Anytime_AtRegularEnd_NotOnDeviatingDay_ReturnsTrue()
        {
            // Arrange
            fixture.SetupAnytimeOrderMode();
            fixture.SetupRegularOpeningDayNextMondayWithOneEarlyPeriod();
            fixture.SetupDeviatingDayDateOnTuesday();
            fixture.SetupDeviatingOpeningDayWithOneDefaultPeriod();
            var testObject = fixture.CreateTestObject();

            var orderDateTime = fixture.NextMonday.ToLocalDateTimeOffset().AddHours(7);

            // Act
            var result = testObject.IsOrderPossibleAt(orderDateTime);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsOrderPossible_Anytime_JustAfterRegularEnd_NotOnDeviatingDay_ReturnsFalse()
        {
            // Arrange
            fixture.SetupAnytimeOrderMode();
            fixture.SetupRegularOpeningDayNextMondayWithOneEarlyPeriod();
            fixture.SetupDeviatingDayDateOnTuesday();
            fixture.SetupDeviatingOpeningDayWithOneDefaultPeriod();
            var testObject = fixture.CreateTestObject();

            var orderDateTime = fixture.NextMonday.ToLocalDateTimeOffset().AddHours(7).AddMinutes(1);

            // Act
            var result = testObject.IsOrderPossibleAt(orderDateTime);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ChangePickupInfo_NotEnabled_SavesPickupInfoAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var pickupInfo = new PickupInfoBuilder()
                .WithEnabled(false)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangePickupInfo(pickupInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.PickupInfo.Should().BeEquivalentTo(pickupInfo);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ChangePickupInfo_Enabled_AverageTimeNoValue_SavesPickupInfoAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var pickupInfo = new PickupInfoBuilder()
                .WithEnabled(true)
                .WithAverageTime(null)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangePickupInfo(pickupInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.PickupInfo.Should().BeEquivalentTo(pickupInfo);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ChangePickupInfo_Enabled_AverageTimeBelowMin_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var pickupInfo = new PickupInfoBuilder()
                .WithEnabled(true)
                .WithAverageTime(4)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangePickupInfo(pickupInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangePickupInfo_Enabled_AverageTimeAboveMax_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var pickupInfo = new PickupInfoBuilder()
                .WithEnabled(true)
                .WithAverageTime(121)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangePickupInfo(pickupInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangePickupInfo_Enabled_MinimumOrderValueNoValue_SavesPickupInfoAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var pickupInfo = new PickupInfoBuilder()
                .WithEnabled(true)
                .WithMinimumOrderValue(null)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangePickupInfo(pickupInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.PickupInfo.Should().BeEquivalentTo(pickupInfo);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ChangePickupInfo_Enabled_MinimumOrderValueBelowMin_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var pickupInfo = new PickupInfoBuilder()
                .WithEnabled(true)
                .WithMinimumOrderValue(-0.01m)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangePickupInfo(pickupInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangePickupInfo_Enabled_MinimumOrderValueAboveMax_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var pickupInfo = new PickupInfoBuilder()
                .WithEnabled(true)
                .WithMinimumOrderValue(50.01m)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangePickupInfo(pickupInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangePickupInfo_Enabled_MaximumOrderValueNoValue_SavesPickupInfoAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var pickupInfo = new PickupInfoBuilder()
                .WithEnabled(true)
                .WithMaximumOrderValue(null)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangePickupInfo(pickupInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.PickupInfo.Should().BeEquivalentTo(pickupInfo);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ChangePickupInfo_Enabled_MaximumOrderValueBelowMin_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var pickupInfo = new PickupInfoBuilder()
                .WithEnabled(true)
                .WithMaximumOrderValue(-0.01m)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangePickupInfo(pickupInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangePickupInfo_Enabled_AllValid_SavesPickupInfoAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var pickupInfo = new PickupInfoBuilder()
                .WithEnabled(true)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangePickupInfo(pickupInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.PickupInfo.Should().BeEquivalentTo(pickupInfo);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ChangeDeliveryInfo_NotEnabled_SavesDeliveryInfoAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var deliveryInfo = new DeliveryInfoBuilder()
                .WithEnabled(false)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeDeliveryInfo(deliveryInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.DeliveryInfo.Should().BeEquivalentTo(deliveryInfo);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ChangeDeliveryInfo_Enabled_AverageTimeNoValue_SavesDeliveryInfoAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var deliveryInfo = new DeliveryInfoBuilder()
                .WithEnabled(true)
                .WithAverageTime(null)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeDeliveryInfo(deliveryInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.DeliveryInfo.Should().BeEquivalentTo(deliveryInfo);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ChangeDeliveryInfo_Enabled_AverageTimeBelowMin_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var deliveryInfo = new DeliveryInfoBuilder()
                .WithEnabled(true)
                .WithAverageTime(4)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeDeliveryInfo(deliveryInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeDeliveryInfo_Enabled_AverageTimeAboveMax_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var deliveryInfo = new DeliveryInfoBuilder()
                .WithEnabled(true)
                .WithAverageTime(121)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeDeliveryInfo(deliveryInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeDeliveryInfo_Enabled_MinimumOrderValueNoValue_SavesDeliveryInfoAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var deliveryInfo = new DeliveryInfoBuilder()
                .WithEnabled(true)
                .WithMinimumOrderValue(null)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeDeliveryInfo(deliveryInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.DeliveryInfo.Should().BeEquivalentTo(deliveryInfo);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ChangeDeliveryInfo_Enabled_MinimumOrderValueBelowMin_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var deliveryInfo = new DeliveryInfoBuilder()
                .WithEnabled(true)
                .WithMinimumOrderValue(-0.01m)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeDeliveryInfo(deliveryInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeDeliveryInfo_Enabled_MinimumOrderValueAboveMax_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var deliveryInfo = new DeliveryInfoBuilder()
                .WithEnabled(true)
                .WithMinimumOrderValue(50.01m)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeDeliveryInfo(deliveryInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeDeliveryInfo_Enabled_MaximumOrderValueNoValue_SavesDeliveryInfoAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var deliveryInfo = new DeliveryInfoBuilder()
                .WithEnabled(true)
                .WithMaximumOrderValue(null)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeDeliveryInfo(deliveryInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.DeliveryInfo.Should().BeEquivalentTo(deliveryInfo);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ChangeDeliveryInfo_Enabled_MaximumOrderValueBelowMin_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var deliveryInfo = new DeliveryInfoBuilder()
                .WithEnabled(true)
                .WithMaximumOrderValue(-0.01m)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeDeliveryInfo(deliveryInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeDeliveryInfo_Enabled_AllValid_SavesDeliveryInfoAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var deliveryInfo = new DeliveryInfoBuilder()
                .WithEnabled(true)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeDeliveryInfo(deliveryInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.DeliveryInfo.Should().BeEquivalentTo(deliveryInfo);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ChangeDeliveryInfo_Enabled_CostsNoValue_SavesDeliveryInfoAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var deliveryInfo = new DeliveryInfoBuilder()
                .WithEnabled(true)
                .WithCosts(null)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeDeliveryInfo(deliveryInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                testObject.DeliveryInfo.Should().BeEquivalentTo(deliveryInfo);
                testObject.UpdatedOn.Should().BeCloseTo(DateTimeOffset.UtcNow, 1000);
                testObject.UpdatedBy.Should().Be(fixture.ChangedBy);
            }
        }

        [Fact]
        public void ChangeDeliveryInfo_Enabled_CostsBelowMin_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var deliveryInfo = new DeliveryInfoBuilder()
                .WithEnabled(true)
                .WithCosts(-0.01m)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeDeliveryInfo(deliveryInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void ChangeDeliveryInfo_Enabled_CostsAboveMax_ReturnsFailure()
        {
            // Arrange
            fixture.SetupChangedBy();
            var testObject = fixture.CreateTestObject();

            var deliveryInfo = new DeliveryInfoBuilder()
                .WithEnabled(true)
                .WithCosts(10.01m)
                .WithValidConstrains()
                .Create();

            // Act
            var result = testObject.ChangeDeliveryInfo(deliveryInfo, fixture.ChangedBy);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        private sealed class Fixture
        {
            public Fixture()
            {
                NextMonday = CalculateNextMonday();
                NextTuesday = NextMonday.AddDays(1);
                Today = Date.Today;
                TodayDayOfWeekIndex = CalculateDayOfWeekIndex(Today.DayOfWeek);
            }

            public Date NextMonday { get; }

            public Date NextTuesday { get; }

            public Date Today { get; }

            public int TodayDayOfWeekIndex { get; }

            public UserId ChangedBy { get; private set; }

            public SupportedOrderMode? SupportedOrderMode { get; private set; }

            public List<RegularOpeningDay> RegularOpeningDays { get; private set; }

            public Date DeviatingDayDate { get; private set; }

            public List<DeviatingOpeningDay> DeviatingOpeningDays { get; private set; }

            public void SetupChangedBy()
            {
                ChangedBy = new UserIdBuilder().Create();
            }

            public void SetupOnlyPhoneOrderMode()
            {
                SupportedOrderMode = Core.Domain.Model.Restaurants.SupportedOrderMode.OnlyPhone;
            }

            public void SetupAtNextShiftOrderMode()
            {
                SupportedOrderMode = Core.Domain.Model.Restaurants.SupportedOrderMode.AtNextShift;
            }

            public void SetupAnytimeOrderMode()
            {
                SupportedOrderMode = Core.Domain.Model.Restaurants.SupportedOrderMode.Anytime;
            }

            public void SetupEmptyRegularOpeningDays()
            {
                RegularOpeningDays = new List<RegularOpeningDay>();
            }

            public void SetupRegularOpeningDayNextMondayWithOneDefaultPeriod()
            {
                RegularOpeningDays = new List<RegularOpeningDay>
                {
                    new RegularOpeningDayBuilder()
                        .WithDayOfWeek(CalculateDayOfWeekIndex(NextMonday.DayOfWeek))
                        .WithOpeningPeriods(new OpeningPeriodBuilder().WithValidConstrains().CreateMany(1))
                        .Create()
                };
            }

            public void SetupRegularOpeningDayNextMondayWithOneEarlyPeriod()
            {
                RegularOpeningDays = new List<RegularOpeningDay>
                {
                    new RegularOpeningDayBuilder()
                        .WithDayOfWeek(CalculateDayOfWeekIndex(NextMonday.DayOfWeek))
                        .WithOpeningPeriods(new []
                        {
                            new OpeningPeriod(TimeSpan.FromHours(6), TimeSpan.FromHours(7))
                        })
                        .Create()
                };
            }

            public void SetupRegularOpeningDayNextMondayWithTwoPeriods()
            {
                RegularOpeningDays = new List<RegularOpeningDay>
                {
                    new RegularOpeningDayBuilder()
                        .WithDayOfWeek(CalculateDayOfWeekIndex(NextMonday.DayOfWeek))
                        .WithOpeningPeriods(new[]
                        {
                            new OpeningPeriod(TimeSpan.FromHours(12), TimeSpan.FromHours(14)),
                            new OpeningPeriod(TimeSpan.FromHours(16), TimeSpan.FromHours(22))
                        })
                        .Create()
                };
            }

            public void SetupRegularOpeningDayTodayWithPeriodInAnHourForRestOfDay()
            {
                var currentTime = DateTimeOffset.Now.TimeOfDay.Add(TimeSpan.FromMinutes(-1));

                RegularOpeningDays = new List<RegularOpeningDay>
                {
                    new RegularOpeningDayBuilder()
                        .WithDayOfWeek(TodayDayOfWeekIndex)
                        .WithOpeningPeriods(new[]
                        {
                            new OpeningPeriod(currentTime.Add(TimeSpan.FromHours(1)), TimeSpan.FromHours(28))
                        })
                        .Create()
                };
            }

            public void SetupRegularOpeningDayTodayWithPeriodNowFor2MinutesAndInAnHourForRestOfDay()
            {
                var currentTime = DateTimeOffset.Now.TimeOfDay.Add(TimeSpan.FromMinutes(-1));

                RegularOpeningDays = new List<RegularOpeningDay>
                {
                    new RegularOpeningDayBuilder()
                        .WithDayOfWeek(TodayDayOfWeekIndex)
                        .WithOpeningPeriods(new[]
                        {
                            new OpeningPeriod(currentTime, currentTime.Add(TimeSpan.FromMinutes(2))),
                            new OpeningPeriod(currentTime.Add(TimeSpan.FromHours(1)), TimeSpan.FromHours(28))
                        })
                        .Create()
                };
            }

            public void SetupRegularOpeningDayTodayWithPeriodNowForRestOfDay()
            {
                var currentTime = DateTimeOffset.Now.TimeOfDay.Add(TimeSpan.FromMinutes(-1));

                RegularOpeningDays = new List<RegularOpeningDay>
                {
                    new RegularOpeningDayBuilder()
                        .WithDayOfWeek(TodayDayOfWeekIndex)
                        .WithOpeningPeriods(new[]
                        {
                            new OpeningPeriod(currentTime, TimeSpan.FromHours(28))
                        })
                        .Create()
                };
            }

            public void SetupEmptyDeviatingOpeningDays()
            {
                DeviatingOpeningDays = new List<DeviatingOpeningDay>();
            }

            public void SetupDeviatingDayDateOnMonday()
            {
                DeviatingDayDate = NextMonday;
            }

            public void SetupDeviatingDayDateOnTuesday()
            {
                DeviatingDayDate = NextTuesday;
            }

            public void SetupDeviatingOpeningDayWithoutPeriods()
            {
                DeviatingOpeningDays = new List<DeviatingOpeningDay>
                {
                    new DeviatingOpeningDayBuilder()
                        .WithDate(DeviatingDayDate)
                        .WithStatus(DeviatingOpeningDayStatus.Open)
                        .WithoutOpeningPeriods()
                        .Create()
                };
            }

            public void SetupDeviatingOpeningDayWithOneDefaultPeriod()
            {
                DeviatingOpeningDays = new List<DeviatingOpeningDay>
                {
                    new DeviatingOpeningDayBuilder()
                        .WithDate(DeviatingDayDate)
                        .WithStatus(DeviatingOpeningDayStatus.Open)
                        .WithOpeningPeriods(new OpeningPeriodBuilder().WithValidConstrains().CreateMany(1))
                        .Create()
                };
            }

            public void SetupDeviatingOpeningDayWithTwoPeriods()
            {
                DeviatingOpeningDays = new List<DeviatingOpeningDay>
                {
                    new DeviatingOpeningDayBuilder()
                        .WithDate(DeviatingDayDate)
                        .WithOpeningPeriods(new[]
                        {
                            new OpeningPeriod(TimeSpan.FromHours(12), TimeSpan.FromHours(14)),
                            new OpeningPeriod(TimeSpan.FromHours(16), TimeSpan.FromHours(22))
                        })
                        .Create()
                };
            }

            public Restaurant CreateTestObject()
            {
                var builder = new RestaurantBuilder();

                if (SupportedOrderMode != null)
                    builder = builder.WithSupportedOrderMode(SupportedOrderMode.Value);

                if (RegularOpeningDays != null)
                    builder = builder.WithRegularOpeningDays(RegularOpeningDays);

                if (DeviatingOpeningDays != null)
                    builder = builder.WithDeviatingOpeningDays(DeviatingOpeningDays);

                return builder
                    .WithValidConstrains()
                    .Create();
            }

            private static Date CalculateNextMonday()
            {
                var date = Date.Today.AddDays(1);
                while (date.DayOfWeek != DayOfWeek.Monday)
                    date = date.AddDays(1);
                return date;
            }

            private static int CalculateDayOfWeekIndex(DayOfWeek dayOfWeek)
            {
                var dayOfWeekIndex = ((int) dayOfWeek - 1) % 7;

                if (dayOfWeekIndex < 0)
                {
                    dayOfWeekIndex += 7;
                }

                return dayOfWeekIndex;
            }
        }
    }
}
