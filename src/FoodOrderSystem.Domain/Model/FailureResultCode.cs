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
        RestaurantMinimumOrderValueTooHigh,
        RestaurantDeliveryCostsTooHigh,
        RestaurantDeliveryTimeBeginTooEarly,
        RestaurantDeliveryTimeEndBeforeStart,
        RestaurantDeliveryTimeIntersects,
        RestaurantDoesNotExist,
        RestaurantContainsDishCategories,
        RestaurantContainsDishes,
        DishCategoryDoesNotBelongToRestaurant,
        DishCategoryContainsDishes,
        DishDoesNotBelongToDishCategory,
        DishVariantPriceIsNegativeOrZero,
        DishVariantPriceIsTooBig,
        CannotRemoveCurrentUserFromRestaurantAdmins
    }
}
