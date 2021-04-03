using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gastromio.Core.Application.Ports.Persistence;
using Gastromio.Core.Domain.Model.Orders;
using Gastromio.Core.Domain.Model.Restaurants;
using Moq;
using Moq.Language.Flow;

namespace Gastromio.Domain.TestKit.Application.Ports.Persistence
{
    public class OrderRepositoryMock : Mock<IOrderRepository>
    {
        public OrderRepositoryMock()
        {
        }

        public OrderRepositoryMock(MockBehavior behavior) : base(behavior)
        {
        }

        public ISetup<IOrderRepository, Task<IEnumerable<Order>>> SetupFindByRestaurantIdAsync(
            RestaurantId restaurantId)
        {
            return Setup(m => m.FindByRestaurantIdAsync(restaurantId, It.IsAny<CancellationToken>()));
        }

        public void VerifyFindByRestaurantIdAsync(RestaurantId restaurantId, Func<Times> times)
        {
            Verify(m => m.FindByRestaurantIdAsync(restaurantId, It.IsAny<CancellationToken>()), times);
        }

        public ISetup<IOrderRepository, Task<IEnumerable<Order>>> SetupFindByPendingCustomerNotificationAsync()
        {
            return Setup(m => m.FindByPendingCustomerNotificationAsync(It.IsAny<CancellationToken>()));
        }

        public void VerifyFindByPendingCustomerNotificationAsync(Func<Times> times)
        {
            Verify(m => m.FindByPendingCustomerNotificationAsync(It.IsAny<CancellationToken>()), times);
        }

        public ISetup<IOrderRepository, Task<IEnumerable<Order>>> SetupFindByPendingRestaurantEmailNotificationAsync()
        {
            return Setup(m => m.FindByPendingRestaurantEmailNotificationAsync(It.IsAny<CancellationToken>()));
        }

        public void VerifyFindByPendingRestaurantEmailNotificationAsync(Func<Times> times)
        {
            Verify(m => m.FindByPendingRestaurantEmailNotificationAsync(It.IsAny<CancellationToken>()), times);
        }

        public ISetup<IOrderRepository, Task<IEnumerable<Order>>> SetupFindByPendingRestaurantMobileNotificationAsync()
        {
            return Setup(m => m.FindByPendingRestaurantMobileNotificationAsync(It.IsAny<CancellationToken>()));
        }

        public void VerifyFindByPendingRestaurantMobileNotificationAsync(Func<Times> times)
        {
            Verify(m => m.FindByPendingRestaurantMobileNotificationAsync(It.IsAny<CancellationToken>()), times);
        }

        public ISetup<IOrderRepository, Task<Order>> SetupFindByOrderIdAsync(OrderId orderId)
        {
            return Setup(m => m.FindByOrderIdAsync(orderId, It.IsAny<CancellationToken>()));
        }

        public void VerifyFindByOrderIdAsync(OrderId orderId, Func<Times> times)
        {
            Verify(m => m.FindByOrderIdAsync(orderId, It.IsAny<CancellationToken>()), times);
        }

        public ISetup<IOrderRepository, Task> SetupStoreAsync(Order order)
        {
            return Setup(m => m.StoreAsync(order, It.IsAny<CancellationToken>()));
        }

        public void VerifyStoreAsync(Order order, Func<Times> times)
        {
            Verify(m => m.StoreAsync(order, It.IsAny<CancellationToken>()), times);
        }

        public ISetup<IOrderRepository, Task> SetupStoreAsync()
        {
            return Setup(m => m.StoreAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()));
        }

        public void VerifyStoreAsync(Func<Times> times)
        {
            Verify(m => m.StoreAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), times);
        }

        public ISetup<IOrderRepository, Task> SetupRemoveByRestaurantIdAsync(RestaurantId restaurantId)
        {
            return Setup(m => m.RemoveByRestaurantIdAsync(restaurantId, It.IsAny<CancellationToken>()));
        }

        public void VerifyRemoveByRestaurantIdAsync(RestaurantId restaurantId, Func<Times> times)
        {
            Verify(m => m.RemoveByRestaurantIdAsync(restaurantId, It.IsAny<CancellationToken>()), times);
        }

        public ISetup<IOrderRepository, Task> SetupRemoveAsync(OrderId orderId)
        {
            return Setup(m => m.RemoveAsync(orderId, It.IsAny<CancellationToken>()));
        }

        public void VerifyRemoveAsync(OrderId orderId, Func<Times> times)
        {
            Verify(m => m.RemoveAsync(orderId, It.IsAny<CancellationToken>()), times);
        }
    }
}
