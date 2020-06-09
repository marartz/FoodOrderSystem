﻿using FoodOrderSystem.Domain.Model.Restaurant;

namespace FoodOrderSystem.Domain.ViewModels
{
    public class ReservationInfoViewModel
    {
        public string HygienicHandling { get; set; }

        public static ReservationInfoViewModel FromReservationInfo(ReservationInfo reservationInfo)
        {
            return new ReservationInfoViewModel
            {
                HygienicHandling = reservationInfo.HygienicHandling
            };
        }
    }
}