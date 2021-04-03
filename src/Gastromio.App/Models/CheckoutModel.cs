using System;
using System.Collections.Generic;

namespace Gastromio.App.Models
{
    public class CheckoutModel
    {
        public string GivenName { get; set; }

        public string LastName { get; set; }

        public string Street { get; set; }

        public string AddAddressInfo { get; set; }

        public string ZipCode { get; set; }

        public string City { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string OrderType { get; set; }

        public string RestaurantId { get; set; }

        public List<CartDishInfoModel> CartDishes { get; set; }

        public string Comments { get; set; }

        public Guid PaymentMethodId { get; set; }

        public DateTimeOffset? ServiceTime { get; set; }
    }
}
