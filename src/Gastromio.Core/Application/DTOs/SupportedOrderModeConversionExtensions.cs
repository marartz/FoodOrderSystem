using System;
using Gastromio.Core.Domain.Model.Restaurants;

namespace Gastromio.Core.Application.DTOs
{
    public static class SupportedOrderModeConversionExtensions
    {
        public static string ToModel(this SupportedOrderMode supportedOrderMode)
        {
            switch (supportedOrderMode)
            {
                case SupportedOrderMode.OnlyPhone:
                    return "phone";
                case SupportedOrderMode.AtNextShift:
                    return "shift";
                case SupportedOrderMode.Anytime:
                    return "anytime";
                default:
                    throw new ArgumentOutOfRangeException(nameof(supportedOrderMode), supportedOrderMode, null);
            }
        }

        public static SupportedOrderMode ToSupportedOrderMode(this string supportedOrderMode)
        {
            switch (supportedOrderMode)
            {
                case "phone":
                    return SupportedOrderMode.OnlyPhone;
                case "shift":
                    return SupportedOrderMode.AtNextShift;
                case "anytime":
                    return SupportedOrderMode.Anytime;
                default:
                    throw new ArgumentOutOfRangeException(nameof(supportedOrderMode), supportedOrderMode, null);
            }
        }
    }
}
