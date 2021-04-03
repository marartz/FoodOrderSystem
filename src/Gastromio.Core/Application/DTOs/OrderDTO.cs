using System;
using Gastromio.Core.Domain.Model.Orders;
using Gastromio.Core.Domain.Model.PaymentMethods;

namespace Gastromio.Core.Application.DTOs
{
    public class OrderDTO
    {
        public OrderDTO(Guid id, CustomerInfo customerInfo, CartInfo cartInfo, string comments,
            Guid paymentMethodId, PaymentMethodDTO paymentMethod, double valueOfOrder, double costs, double totalPrice,
            DateTimeOffset createdOn, DateTimeOffset? updatedOn, Guid? updatedBy)
        {
            Id = id;
            CustomerInfo = customerInfo;
            CartInfo = cartInfo;
            Comments = comments;
            PaymentMethodId = paymentMethodId;
            PaymentMethod = paymentMethod;
            ValueOfOrder = valueOfOrder;
            Costs = costs;
            TotalPrice = totalPrice;
            CreatedOn = createdOn;
            UpdatedOn = updatedOn;
            UpdatedBy = updatedBy;
        }

        internal OrderDTO(Order order, PaymentMethod paymentMethod)
        {
            Id = order.Id.Value;
            CustomerInfo = order.CustomerInfo;
            CartInfo = order.CartInfo;
            Comments = order.Comments;
            PaymentMethodId = order.PaymentMethodId.Value;
            PaymentMethod = paymentMethod != null ? new PaymentMethodDTO(paymentMethod) : null;
            ValueOfOrder = (double) order.GetValueOfOrder();
            Costs = (double) order.Costs;
            TotalPrice = (double) order.GetTotalPrice();
            CreatedOn = order.CreatedOn;
            UpdatedOn = order.UpdatedOn;
            UpdatedBy = order.UpdatedBy?.Value;
        }

        public Guid Id { get; }

        public CustomerInfo CustomerInfo { get; }

        public CartInfo CartInfo { get; }

        public string Comments { get; }

        public Guid PaymentMethodId { get; }

        public PaymentMethodDTO PaymentMethod { get; }

        public double ValueOfOrder { get; }

        public double Costs { get; }

        public double TotalPrice { get; }

        public DateTimeOffset CreatedOn { get; }

        public DateTimeOffset? UpdatedOn { get; }

        public Guid? UpdatedBy { get; }
    }
}
