using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Gastromio.Core.Domain.Model.PaymentMethods;
using Xunit;

namespace Gastromio.Domain.Tests.Domain.Model.PaymentMethods
{
    public class PaymentMethodRepositoryTests
    {
        private readonly Fixture fixture;

        public PaymentMethodRepositoryTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public async Task FindAllAsync_ReturnsAllPaymentMethods()
        {
            // Arrange
            var testObject = fixture.CreateTestObject();

            // Act
            var result = (await testObject.FindAllAsync(CancellationToken.None))?.ToList();

            // Assert
            using (new AssertionScope())
            {
                result.Should().HaveCount(4);
                result.Should().Contain(paymentMethod => paymentMethod.Id == PaymentMethodId.Cash);
                result.Should().Contain(paymentMethod => paymentMethod.Id == PaymentMethodId.DebitCard);
                result.Should().Contain(paymentMethod => paymentMethod.Id == PaymentMethodId.CreditCard);
                result.Should().Contain(paymentMethod => paymentMethod.Id == PaymentMethodId.Invoice);
            }
        }

        [Fact]
        public async Task FindByNameAsync_ExactCase_NotIgnoringCase_ReturnsPaymentMethod()
        {
            // Arrange
            var testObject = fixture.CreateTestObject();

            // Act
            var result = await testObject.FindByNameAsync("Bar", false);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.Id.Should().Be(PaymentMethodId.Cash);
            }
        }

        [Fact]
        public async Task FindByNameAsync_ExactCase_IgnoringCase_ReturnsPaymentMethod()
        {
            // Arrange
            var testObject = fixture.CreateTestObject();

            // Act
            var result = await testObject.FindByNameAsync("Bar", true);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.Id.Should().Be(PaymentMethodId.Cash);
            }
        }

        [Fact]
        public async Task FindByNameAsync_LowerCase_NotIgnoringCase_ReturnsNull()
        {
            // Arrange
            var testObject = fixture.CreateTestObject();

            // Act
            var result = await testObject.FindByNameAsync("bar", false);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeNull();
            }
        }

        [Fact]
        public async Task FindByNameAsync_LowerCase_IgnoringCase_ReturnsPaymentMethod()
        {
            // Arrange
            var testObject = fixture.CreateTestObject();

            // Act
            var result = await testObject.FindByNameAsync("bar", true);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.Id.Should().Be(PaymentMethodId.Cash);
            }
        }

        [Fact]
        public async Task FindByPaymentMethodIdAsync_DoesExist_ReturnsPaymentMethod()
        {
            // Arrange
            var testObject = fixture.CreateTestObject();

            // Act
            var result = await testObject.FindByPaymentMethodIdAsync(PaymentMethodId.Cash);

            // Assert
            using (new AssertionScope())
            {
                result.Should().NotBeNull();
                result?.Id.Should().Be(PaymentMethodId.Cash);
            }
        }

        [Fact]
        public async Task FindByPaymentMethodIdAsync_DoesNotExist_ReturnsNull()
        {
            // Arrange
            var testObject = fixture.CreateTestObject();

            // Act
            var result = await testObject.FindByPaymentMethodIdAsync(new PaymentMethodId(Guid.NewGuid()));

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeNull();
            }
        }

        private sealed class Fixture
        {
            public PaymentMethodRepository CreateTestObject()
            {
                return new PaymentMethodRepository();
            }
        }

    }
}
