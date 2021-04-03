using System;
using System.Globalization;
using Gastromio.Core.Application.Commands;
using Gastromio.Core.Application.Queries;
using Gastromio.Core.Application.Services;
using Gastromio.Core.Common;
using Gastromio.Core.Domain.Model.Cuisines;
using Gastromio.Core.Domain.Model.DishCategories;
using Gastromio.Core.Domain.Model.Dishes;
using Gastromio.Core.Domain.Model.PaymentMethods;
using Gastromio.Core.Domain.Model.Restaurants;
using Gastromio.Core.Domain.Model.Users;
using Gastromio.Core.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Gastromio.Core
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCore(this IServiceCollection services)
        {
            // Register model classes
            services.AddTransient<IUserFactory, UserFactory>();
            services.AddTransient<ICuisineFactory, CuisineFactory>();
            services.AddTransient<IPaymentMethodRepository, PaymentMethodRepository>();
            services.AddTransient<IRestaurantFactory, RestaurantFactory>();
            services.AddTransient<IDishCategoryFactory, DishCategoryFactory>();
            services.AddTransient<IDishFactory, DishFactory>();

            // Register command classes
            services.AddTransient<ICommandDispatcher, CommandDispatcher>();
            CommandDispatcher.Initialize(services);

            // Register query classes
            services.AddTransient<IQueryDispatcher, QueryDispatcher>();
            QueryDispatcher.Initialize(services);

            // Initialize failure messages
            var failureMessageService = new FailureMessageService();
            services.AddSingleton<IFailureMessageService>(failureMessageService);

            var deDeCultureInfo = new CultureInfo("de-DE");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.SessionExpired, "Sie sind nicht angemeldet");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.Forbidden, "Sie sind nicht berechtigt, diese Aktion auszuführen");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.InternalServerError, "Es ist ein technischer Fehler aufgetreten. Bitte versuchen Sie es erneut bzw. kontaktieren Sie uns, wenn das Problem anhält.");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.LoginEmailRequired, "Die E-Mail-Addresse wird für den Login benötigt");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.LoginPasswordRequired, "Das Passwort wird für den Login benötigt");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.WrongCredentials, "Emailadresse und/oder Passwort ist nicht korrekt");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.PasswordResetCodeIsInvalid, "Dieser Link ist leider nicht (mehr) gültig, bitte fordere nochmals die Änderung Deines Passworts an");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.UserDoesNotExist, "Benutzer existiert nicht");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.UserAlreadyExists, "Benutzer existiert bereits");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.PasswordIsNotValid, "Das Passwort ist nicht komplex genug (mind. ein Kleinbuchstabe, ein Großbuchstabe, eine Ziffer und ein Zeichen aus '!@#$%^&')");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.CannotRemoveCurrentUser, "Sie können nicht den gerade angemeldeten Benutzer löschen");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.UserIsRestaurantAdmin, "Der Benutzer kann nicht gelöscht werden, da er noch als Restaurantadministrator eingetragen ist (Restaurant(s): {0})");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.CuisineNameIsRequired, "Name der Cuisine ist ein Pflichtfeld");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.CuisineNameTooLong, "Der Name der Cuisine ist zu lang (maximum {0} Zeichen)");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.CuisineDoesNotExist, "Cuisine existiert nicht");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.CuisineAlreadyExists, "Cuisine existiert bereits");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.PaymentMethodDoesNotExist, "Zahlungsmethode existiert nicht");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.PaymentMethodAlreadyExists, "Zahlungsmethode existiert bereits");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantAlreadyExists, "Gleichnamiges Restaurant existiert bereits");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantNameRequired, "Name des Restaurants ist ein Pflichtfeld");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantNameTooLong, "Der Name des Restaurants ist zu lang (maximum {0} Zeichen)");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantAddressRequired, "Die Addresse des Restaurants wird benötigt");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantStreetRequired, "Die Straße des Restaurants wird benötigt");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantStreetTooLong, "Die Straße des Restaurants ist zu lang (maximum {0} Zeichen)");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantStreetInvalid, "Die Straße des Restaurants ist nicht gültig: {0}");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantZipCodeRequired, "Die Postleitzahl des Restaurants wird benötigt");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantZipCodeInvalid, "Die Postleitzahl des Restaurants ist nicht gültig: {0}");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantCityRequired, "Die Stadt des Restaurants wird benötigt");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantCityTooLong, "Die Stadt des Restaurants ist zu lang (maximum {0} Zeichen)");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantPhoneRequired, "Die Telefonnummer des Restaurants wird benötigt");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantPhoneInvalid, "Die Telefonnummer des Restaurants ist nicht gültig: {0}");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantFaxInvalid, "Die Faxnummer des Restaurants ist nicht gültig: {0}");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantWebSiteInvalid, "Die Webseite des Restaurants ist nicht gültig: {0}");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantResponsiblePersonRequired, "Die verantwortliche Person des Restaurants wird benötigt");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantEmailRequired, "Die E-Mail-Addresse des Restaurants wird benötigt");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantEmailInvalid, "Die E-Mail-Addresse des Restaurants ist nicht gültig: {0}");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantMobileInvalid, "Die Mobilnummer des Restaurants ist nicht gültig: {0}");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantImageTypeRequired, "Der Type der Bilddatei wird benötigt");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantImageDataTooBig, "Die Bilddatei ist zu groß (max. 4MB)");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantImageNotValid, "Die angegebene Bilddatei ist nicht gültig");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.NoRestaurantPickupInfosSpecified, "Keine Informationen über die Abholung spezifiziert");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantAveragePickupTimeTooLow, "Die durchschnittliche Zeit bis zur Abholung ist zu klein (mind. 5 Minuten)");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantAveragePickupTimeTooHigh, "Die durchschnittliche Zeit bis zur Abholung ist zu groß (max. 120 Minuten)");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantMinimumPickupOrderValueTooLow, "Der Mindestbestellwert für Abholung ist negativ");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantMinimumPickupOrderValueTooHigh, "Der Mindestbestellwert für Abholung ist zu groß (max. 50€)");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantMaximumPickupOrderValueTooLow, "Der Höchstbestellwert für Abholung ist negativ");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.NoRestaurantDeliveryInfosSpecified, "Keine Informationen über die Lieferung spezifiziert");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantAverageDeliveryTimeTooLow, "Die durchschnittliche Zeit bis zur Lieferung ist zu klein (mind. 5 Minuten)");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantAverageDeliveryTimeTooHigh, "Die durchschnittliche Zeit bis zur Lieferung ist zu groß (max. 120 Minuten)");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantMinimumDeliveryOrderValueTooLow, "Der Mindestbestellwert für Lieferung ist negativ");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantMinimumDeliveryOrderValueTooHigh, "Der Mindestbestellwert für Lieferung ist zu groß (max. 50€)");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantMaximumDeliveryOrderValueTooLow, "Der Höchstbestellwert für Lieferung ist negativ");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantDeliveryCostsTooLow, "Die Lieferkosten sind negativ");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantDeliveryCostsTooHigh, "Die Lieferkosten sind zu groß (max. 10€)");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.NoRestaurantReservationInfosSpecified, "Keine Informationen über die Reservierung spezifiziert");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantWithoutCashPaymentNotAllowed, "Das Deaktivieren von Barzahlung ist nicht möglich");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantOpeningPeriodIntersects, "Die Öffnungsperiode überschneidet sich mit einer bereits eingetragenen Öffnungsperiode");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantDeviatingOpeningDayDoesNotExist, "Der abweichende Öffnungstag ist nicht bekannt");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantDeviatingOpeningDayHasStillOpenPeriods, "Der abweichende Öffnungstag hat noch registrierte Öffnungszeiten");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantOpeningPeriodBeginsTooEarly, "Die Öffnungsperiode darf nicht vor 4 Uhr morgens beginnen");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantOpeningPeriodEndsBeforeStart, "Das Ende der Öffnungsperiode muss nach dem Start liegen");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantDoesNotExist, "Restaurant existiert nicht");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.RestaurantNotActive, "Restaurant ist nicht aktiv");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.DishCategoryDoesNotBelongToRestaurant, "Gerichtkategorie gehört nicht zum Restaurant");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.DishCategoryAlreadyExists, "Es gibt bereits eine Gerichtkategorie mit gleichem Namen");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.DishCategoryDoesNotExist, "Die Gerichtkategorie ist nicht vorhanden");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.DishCategoryNameRequired, "Name der Kategorie ist ein Pflichtfeld");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.DishCategoryNameTooLong, "Der Name der Kategorie ist zu lang (maximum {0} Zeichen)");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.DishCategoryInvalidOrderNo, "Gerichtkategorie hat eine ungültige Reihenfolgenkennzahl");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.DishDoesNotBelongToDishCategory, "Gericht gehört nicht zur Gerichtkategorie");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.DishDoesNotExist, "Dish does not exists");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.DishRestaurantIdRequired, "Restaurant-ID des Gerichts ist ein Pflichtfeld");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.DishCategoryIdRequired, "Kategorie-ID des Gerichts ist ein Pflichtfeld");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.DishNameRequired, "Name des Gerichts ist ein Pflichtfeld");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.DishNameTooLong, "Der Name des Gerichts ist zu lang (maximum {0} Zeichen)");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.DishDescriptionTooLong, "Die Beschreibung des Gerichts ist zu lang (maximum {0} Zeichen)");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.DishProductInfoTooLong, "Die Produktinformation des Gerichts ist zu lang (maximum {0} Zeichen)");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.DishInvalidOrderNo, "Gericht hat eine ungültige Reihenfolgenkennzahl");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.DishVariantNameTooLong, "Der Name der Variante ist zu lang (maximum {0} Zeichen)");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.DishVariantPriceIsNegativeOrZero, "Das Gericht / die Variante muss einen Preis > 0 besitzen");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.DishVariantPriceIsTooBig, "Das Gericht / die Variante muss einen Preis <= 200 besitzen");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.CannotRemoveCurrentUserFromRestaurantAdmins, "Sie können nicht den gerade angemeldeten Benutzer aus den Administratoren des Restaurants löschen");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.OrderIsInvalid, "Die Bestelldaten sind nicht gültig");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.ImportOpeningPeriodIsInvalid, "Die angegebenen Öffnungszeiten sind ungültig: {0}");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.ImportOrderTypeIsInvalid, "Die angegebene Bestellart ist ungültig: {0}");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.ImportCuisinesNotFound, "Keine Cuisine angegeben");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.ImportPaymentMethodsNotFound, "Keine Zahlungsmethoden angegeben");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.ImportPaymentMethodNotFound, "Die angegebene Zahlungsmethode ist nicht bekannt: {0}");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.ImportUnknownSupportedOrderMode, "Der angegebene unterstützte Bestellmodus ist nicht bekannt: {0}");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.ExternalMenuDoesNotExist, "Die externe Speisekarte existiert nicht");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.ExternalMenuHasNoName, "Für die externe Speisekarte ist kein Name angegeben");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.ExternalMenuHasNoDescription, "Für die externe Speisekarte ist keine Beschreibung angegeben");
            failureMessageService.RegisterMessage(deDeCultureInfo, FailureResultCode.ExternalMenuHasNoUrl, "Für die externe Speisekarte ist keine Url angegeben");

            if (!failureMessageService.AreAllCodesRegisteredForCulture(deDeCultureInfo))
                throw new InvalidOperationException($"Not all messages for culture {deDeCultureInfo} are registered");
            
            // Import
            services.AddTransient<IRestaurantDataImporter, RestaurantDataImporter>();
            services.AddTransient<IDishDataImporter, DishDataImporter>();
        }
    }
}
