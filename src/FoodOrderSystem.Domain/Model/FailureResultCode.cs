﻿namespace FoodOrderSystem.Domain.Model
{
    public enum FailureResultCode
    {
        SessionExpired,
        Forbidden,
        InternalServerError,
        WrongCredentials,
        RequiredFieldEmpty,
        FieldValueTooLong,
        FieldValueInvalid,
        ValueMustNotBeNegative,
        UserDoesNotExist,
        UserAlreadyExists,
        CannotRemoveCurrentUser,
        CuisineDoesNotExist,
        CuisineAlreadyExists,
        PaymentMethodDoesNotExist,
        PaymentMethodAlreadyExists,
        RestaurantImageDataTooBig,
        RestaurantImageNotValid,
        NoRestaurantPickupInfosSpecified,
        RestaurantAveragePickupTimeTooLow,
        RestaurantAveragePickupTimeTooHigh,
        RestaurantMinimumPickupOrderValueTooLow,
        RestaurantMinimumPickupOrderValueTooHigh,
        RestaurantMaximumPickupOrderValueTooLow,
        NoRestaurantDeliveryInfosSpecified,
        RestaurantAverageDeliveryTimeTooLow,
        RestaurantAverageDeliveryTimeTooHigh,
        RestaurantMinimumDeliveryOrderValueTooLow,
        RestaurantMinimumDeliveryOrderValueTooHigh,
        RestaurantMaximumDeliveryOrderValueTooLow,
        RestaurantDeliveryCostsTooLow,
        RestaurantDeliveryCostsTooHigh,
        NoRestaurantReservationInfosSpecified,
        RestaurantOpeningPeriodBeginsTooEarly,
        RestaurantOpeningPeriodEndsBeforeStart,
        RestaurantOpeningPeriodIntersects,
        RestaurantDoesNotExist,
        DishCategoryDoesNotBelongToRestaurant,
        DishCategoryAlreadyExists,
        DishCategoryDoesNotExist,
        DishCategoryInvalidOrderNo,
        DishDoesNotBelongToDishCategory,
        DishDoesNotExist,
        DishInvalidOrderNo,
        DishVariantPriceIsNegativeOrZero,
        DishVariantPriceIsTooBig,
        CannotRemoveCurrentUserFromRestaurantAdmins,
        OrderIsInvalid
    }
}
