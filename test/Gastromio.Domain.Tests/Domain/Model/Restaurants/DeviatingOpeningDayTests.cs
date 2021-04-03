using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Domain.TestKit.Domain.Model.Restaurants;
using Xunit;

namespace Gastromio.Domain.Tests.Domain.Model.Restaurants
{
    public class DeviatingOpeningDayTests
    {
        private readonly Fixture fixture;

        public DeviatingOpeningDayTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void FindPeriodAtTime_WithoutPeriods_ReturnsNull()
        {
            // Arrange
            fixture.SetupWithoutPeriods();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.FindPeriodAtTime(TimeSpan.Zero);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void FindPeriodAtTime_LaterPeriod_ReturnsNull()
        {
            // Arrange
            fixture.SetupWithOnePeriod();
            var testObject = fixture.CreateTestObject();

            var time = fixture.OpeningPeriods.First().Start.Add(TimeSpan.FromMinutes(-1));

            // Act
            var result = testObject.FindPeriodAtTime(time);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void FindPeriodAtTime_EarlierPeriod_ReturnsNull()
        {
            // Arrange
            fixture.SetupWithOnePeriod();
            var testObject = fixture.CreateTestObject();

            var time = fixture.OpeningPeriods.First().End.Add(TimeSpan.FromMinutes(1));

            // Act
            var result = testObject.FindPeriodAtTime(time);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void FindPeriodAtTime_MatchingPeriodAtStart_ReturnsPeriod()
        {
            // Arrange
            fixture.SetupWithOnePeriod();
            var testObject = fixture.CreateTestObject();

            var time = fixture.OpeningPeriods.First().Start;

            // Act
            var result = testObject.FindPeriodAtTime(time);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().BeEquivalentTo(fixture.OpeningPeriods.First());
            }
        }

        [Fact]
        public void FindPeriodAtTime_MatchingPeriodAtEnd_ReturnsPeriod()
        {
            // Arrange
            fixture.SetupWithOnePeriod();
            var testObject = fixture.CreateTestObject();

            var time = fixture.OpeningPeriods.First().End;

            // Act
            var result = testObject.FindPeriodAtTime(time);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result.Should().BeEquivalentTo(fixture.OpeningPeriods.First());
            }
        }

        [Fact]
        public void AddPeriod_SamePeriodAlreadyPresent_ChangesNothingAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupWithOnePeriod();
            var testObject = fixture.CreateTestObject();

            var curPeriod = fixture.OpeningPeriods.First();

            var period = new OpeningPeriodBuilder()
                .WithStart(curPeriod.Start)
                .WithEnd(curPeriod.End)
                .Create();

            // Act
            var result = testObject.AddPeriod(period);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                var successResult = (SuccessResult<DeviatingOpeningDay>) result;
                successResult.Should().NotBeNull();
                successResult?.Value.OpeningPeriods.Should().BeEquivalentTo(curPeriod);
            }
        }

        [Fact]
        public void AddPeriod_StartBeforeEarliestOpeningTime_ReturnsFailure()
        {
            // Arrange
            fixture.SetupWithoutPeriods();
            var testObject = fixture.CreateTestObject();

            var period = new OpeningPeriodBuilder()
                .WithStart(TimeSpan.FromHours(OpeningPeriod.EarliestOpeningTime - 1))
                .WithEnd(TimeSpan.FromHours(OpeningPeriod.EarliestOpeningTime + 1))
                .Create();

            // Act
            var result = testObject.AddPeriod(period);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void AddPeriod_StartBeforeEnd_ReturnsFailure()
        {
            // Arrange
            fixture.SetupWithoutPeriods();
            var testObject = fixture.CreateTestObject();

            var period = new OpeningPeriodBuilder()
                .WithStart(TimeSpan.FromHours(OpeningPeriod.EarliestOpeningTime + 2))
                .WithEnd(TimeSpan.FromHours(OpeningPeriod.EarliestOpeningTime + 1))
                .Create();

            // Act
            var result = testObject.AddPeriod(period);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void AddPeriod_NewEndEqualsCurrentStart_ReturnsFailure()
        {
            // Arrange
            fixture.SetupWithOnePeriod();
            var testObject = fixture.CreateTestObject();

            var curPeriod = fixture.OpeningPeriods.First();

            var period = new OpeningPeriodBuilder()
                .WithStart(curPeriod.Start.Add(TimeSpan.FromHours(-1)))
                .WithEnd(curPeriod.Start)
                .Create();

            // Act
            var result = testObject.AddPeriod(period);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void AddPeriod_NewEndIsOverlappingIntoCurrent_ReturnsFailure()
        {
            // Arrange
            fixture.SetupWithOnePeriod();
            var testObject = fixture.CreateTestObject();

            var curPeriod = fixture.OpeningPeriods.First();

            var period = new OpeningPeriodBuilder()
                .WithStart(curPeriod.Start.Add(TimeSpan.FromHours(-1)))
                .WithEnd(curPeriod.Start.Add(TimeSpan.FromHours(+1)))
                .Create();

            // Act
            var result = testObject.AddPeriod(period);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void AddPeriod_NewStartIsOverlappingIntoCurrent_ReturnsFailure()
        {
            // Arrange
            fixture.SetupWithOnePeriod();
            var testObject = fixture.CreateTestObject();

            var curPeriod = fixture.OpeningPeriods.First();

            var period = new OpeningPeriodBuilder()
                .WithStart(curPeriod.End.Add(TimeSpan.FromHours(-1)))
                .WithEnd(curPeriod.End.Add(TimeSpan.FromHours(+1)))
                .Create();

            // Act
            var result = testObject.AddPeriod(period);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void AddPeriod_NewStartEqualsCurrentEnd_ReturnsFailure()
        {
            // Arrange
            fixture.SetupWithOnePeriod();
            var testObject = fixture.CreateTestObject();

            var curPeriod = fixture.OpeningPeriods.First();

            var period = new OpeningPeriodBuilder()
                .WithStart(curPeriod.End)
                .WithEnd(curPeriod.End.Add(TimeSpan.FromHours(+1)))
                .Create();

            // Act
            var result = testObject.AddPeriod(period);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void AddPeriod_NewStartEqualsCurStartButShorter_ReturnsFailure()
        {
            // Arrange
            fixture.SetupWithOnePeriod();
            var testObject = fixture.CreateTestObject();

            var curPeriod = fixture.OpeningPeriods.First();

            var period = new OpeningPeriodBuilder()
                .WithStart(curPeriod.Start)
                .WithEnd(curPeriod.End.Add(TimeSpan.FromHours(-1)))
                .Create();

            // Act
            var result = testObject.AddPeriod(period);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void AddPeriod_NewIsCompletelyInCurrent_ReturnsFailure()
        {
            // Arrange
            fixture.SetupWithOnePeriod();
            var testObject = fixture.CreateTestObject();

            var curPeriod = fixture.OpeningPeriods.First();

            var period = new OpeningPeriodBuilder()
                .WithStart(curPeriod.Start.Add(TimeSpan.FromHours(+1)))
                .WithEnd(curPeriod.End.Add(TimeSpan.FromHours(-1)))
                .Create();

            // Act
            var result = testObject.AddPeriod(period);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void AddPeriod_NewEndEqualsCurEndButShorter_ReturnsFailure()
        {
            // Arrange
            fixture.SetupWithOnePeriod();
            var testObject = fixture.CreateTestObject();

            var curPeriod = fixture.OpeningPeriods.First();

            var period = new OpeningPeriodBuilder()
                .WithStart(curPeriod.Start.Add(TimeSpan.FromHours(+1)))
                .WithEnd(curPeriod.End)
                .Create();

            // Act
            var result = testObject.AddPeriod(period);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void AddPeriod_CurrentIsCompletelyInNew_ReturnsFailure()
        {
            // Arrange
            fixture.SetupWithOnePeriod();
            var testObject = fixture.CreateTestObject();

            var curPeriod = fixture.OpeningPeriods.First();

            var period = new OpeningPeriodBuilder()
                .WithStart(curPeriod.Start.Add(TimeSpan.FromHours(-1)))
                .WithEnd(curPeriod.End.Add(TimeSpan.FromHours(+1)))
                .Create();

            // Act
            var result = testObject.AddPeriod(period);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsFailure.Should().BeTrue();
            }
        }

        [Fact]
        public void AddPeriod_WithoutPeriods_AddsPeriodAndReturnsSuccess()
        {
            // Arrange
            fixture.SetupWithoutPeriods();
            var testObject = fixture.CreateTestObject();

            var period = new OpeningPeriodBuilder()
                .WithStart(TimeSpan.FromHours(8))
                .WithEnd(TimeSpan.FromHours(16))
                .Create();

            // Act
            var result = testObject.AddPeriod(period);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.IsSuccess.Should().BeTrue();
                var successResult = (SuccessResult<DeviatingOpeningDay>) result;
                successResult.Should().NotBeNull();
                successResult?.Value.OpeningPeriods.Should().BeEquivalentTo(period);
            }
        }

        [Fact]
        public void RemovePeriod_WithoutPeriods_DoesNotChangeAnything()
        {
            // Arrange
            fixture.SetupWithoutPeriods();
            var testObject = fixture.CreateTestObject();

            // Act
            var result = testObject.RemovePeriod(TimeSpan.FromHours(8));

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.OpeningPeriods.Should().BeEmpty();
            }
        }

        [Fact]
        public void RemovePeriod_WithNonMatchingPeriods_DoesNotChangeAnything()
        {
            // Arrange
            fixture.SetupWithOnePeriod();
            var testObject = fixture.CreateTestObject();

            var curPeriod = fixture.OpeningPeriods.First();

            // Act
            var result = testObject.RemovePeriod(curPeriod.Start.Add(TimeSpan.FromHours(1)));

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.OpeningPeriods.Should().BeEquivalentTo(curPeriod);
            }
        }

        [Fact]
        public void RemovePeriod_WithMatchingPeriod_RemovesPeriod()
        {
            // Arrange
            fixture.SetupWithOnePeriod();
            var testObject = fixture.CreateTestObject();

            var curPeriod = fixture.OpeningPeriods.First();

            // Act
            var result = testObject.RemovePeriod(curPeriod.Start);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.OpeningPeriods.Should().BeEmpty();
            }
        }

        private sealed class Fixture
        {
            public IList<OpeningPeriod> OpeningPeriods { get; private set; }

            public DeviatingOpeningDay CreateTestObject()
            {
                return new DeviatingOpeningDayBuilder()
                    .WithOpeningPeriods(OpeningPeriods)
                    .Create();
            }

            public void SetupWithoutPeriods()
            {
                OpeningPeriods = new List<OpeningPeriod>();
            }

            public void SetupWithOnePeriod()
            {
                var start = TimeSpan.FromHours(8);
                var end = TimeSpan.FromHours(16);

                OpeningPeriods = new OpeningPeriodBuilder()
                    .WithStart(start)
                    .WithEnd(end)
                    .CreateMany(1)
                    .ToList();
            }
        }
    }
}
