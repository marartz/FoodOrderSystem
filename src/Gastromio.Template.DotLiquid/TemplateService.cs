using System;
using System.Text;
using Gastromio.Core.Application.Ports.Template;
using Gastromio.Core.Domain.Model.Orders;

namespace Gastromio.Template.DotLiquid
{
    public class TemplateService : ITemplateService
    {
        public EmailData GetCustomerEmail(Order order)
        {
            return order.CartInfo.OrderType switch
            {
                OrderType.Pickup => GetCustomerEmailForPickup(order),
                OrderType.Delivery => GetCustomerEmailForDelivery(order),
                OrderType.Reservation => GetCustomerEmailForReservation(order),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static EmailData GetCustomerEmailForPickup(Order order)
        {
            var sb = new StringBuilder();
            AppendCustomerSalutation(order, sb);
            AppendOrderReceptionForCustomer(order, sb);
            AppendGastromioInfoForCustomerOrder(order, sb);
            AppendOrderRestaurantInfoForCustomer(order, sb);
            AppendOrderDetails(sb, order);

            sb.Append("<p>");
            sb.Append("Die gewählte Bestellart ist: Abholung. Bitte hole das Essen beim Restaurant ab.");
            sb.Append("</p>");

            AppendServiceTime(sb, order);
            AppendGreetings(sb);

            var message = sb.ToString();
            return new EmailData
            {
                Subject = "Deine Bestellung bei Gastromio.de",
                TextPart = message
                    .Replace("<br/>", "\r\n")
                    .Replace("<p>", "")
                    .Replace("</p>", "\r\n"),
                HtmlPart = message
            };
        }

        private static EmailData GetCustomerEmailForDelivery(Order order)
        {
            var sb = new StringBuilder();
            AppendCustomerSalutation(order, sb);
            AppendOrderReceptionForCustomer(order, sb);
            AppendGastromioInfoForCustomerOrder(order, sb);
            AppendOrderRestaurantInfoForCustomer(order, sb);
            AppendOrderDetails(sb, order);

            sb.Append("<p>");
            sb.Append("Die gewählte Bestellart ist: Lieferung. Das Essen wird Dir nach Hause geliefert.");
            sb.Append("</p>");

            AppendServiceTime(sb, order);
            AppendGreetings(sb);

            var message = sb.ToString();
            return new EmailData
            {
                Subject = "Deine Bestellung bei Gastromio.de",
                TextPart = message
                    .Replace("<br/>", "\r\n")
                    .Replace("<p>", "")
                    .Replace("</p>", "\r\n"),
                HtmlPart = message
            };
        }

        private static EmailData GetCustomerEmailForReservation(Order order)
        {
            var sb = new StringBuilder();
            AppendCustomerSalutation(order, sb);
            AppendReservationReceptionForCustomer(order, sb);
            AppendServiceTime(sb, order);
            AppendGastromioInfoForCustomerReservation(order, sb);
            AppendGreetings(sb);

            var message = sb.ToString();
            return new EmailData
            {
                Subject = "Deine Reservierungsanfrage bei Gastromio.de",
                TextPart = message
                    .Replace("<br/>", "\r\n")
                    .Replace("<p>", "")
                    .Replace("</p>", "\r\n"),
                HtmlPart = message
            };
        }

        private static void AppendGreetings(StringBuilder sb)
        {
            sb.AppendLine("<br/>");
            sb.Append("<p>");
            sb.AppendLine("Dein Gastromio-Team");
            sb.Append("</p>");
        }

        private static void AppendCustomerSalutation(Order order, StringBuilder sb)
        {
            sb.Append("<p>");
            sb.Append("Liebe/r ");
            sb.Append(order.CustomerInfo.GivenName);
            sb.Append(",");
            sb.Append("</p>");
        }

        private static void AppendOrderReceptionForCustomer(Order order, StringBuilder sb)
        {
            sb.Append("<p>");
            sb.Append("wir haben Deine Bestellung empfangen und an ");
            sb.Append(order.CartInfo.RestaurantInfo);
            sb.Append(" weitergeleitet!");
            sb.Append("</p>");
        }

        private static void AppendReservationReceptionForCustomer(Order order, StringBuilder sb)
        {
            sb.Append("<p>");
            sb.Append("wir haben Deine Reservierungsanfrage empfangen und an ");
            sb.Append(order.CartInfo.RestaurantInfo);
            sb.Append(" weitergeleitet!");
            sb.Append("</p>");
        }

        private static void AppendGastromioInfoForCustomerOrder(Order order, StringBuilder sb)
        {
            sb.Append("<p>");
            sb.AppendLine(
                "Gastromio.de wurde ehrenamtlich erstellt, um Bocholts schönes Gastronomieangebot zu erhalten. Gastromio wurde");
            sb.AppendLine(
                "erst kürzlich in Betrieb genommen. Es ist also möglich, dass der Wirt noch nicht in der Bestellabwicklung über");
            sb.AppendLine(
                "Gastromio geübt ist, oder sich ein Systemfehler eingeschlichen hat, den wir noch nicht kennen. Wir haben daher");
            sb.AppendLine(
                "den Wirt gebeten, seine Vorbestellungen zu Beginn der Schicht kurz per E-Mail zu bestätigen. Passiert das nicht,");
            sb.AppendLine(
                "frag ruhig kurz nach, sei aber nett, es ist für den Wirt genauso neu, wie für Dich. Das Restaurant ist unter der");
            sb.Append("Telefonnummer ");
            sb.Append(order.CartInfo.RestaurantPhone);
            sb.Append(" zu erreichen.");
            sb.Append("</p>");

            sb.Append("<p>");
            sb.AppendLine(
                "Wenn ein Problem aufgetreten ist, das ihr nicht lösen konntet, melde Dich doch gerne unter support@gastromio.de!");
            sb.Append("Wir nehmen Deinen Hinweis gerne auf.");
            sb.Append("</p>");
        }

        private static void AppendGastromioInfoForCustomerReservation(Order order, StringBuilder sb)
        {
            sb.Append("<p>");
            sb.AppendLine(
                "Gastromio.de wurde ehrenamtlich erstellt, um Bocholts schönes Gastronomieangebot zu erhalten. Gastromio wurde");
            sb.AppendLine(
                "erst kürzlich in Betrieb genommen. Es ist also möglich, dass der Wirt noch nicht in der Abwicklung der Reservierungsanfragen über");
            sb.AppendLine(
                "Gastromio geübt ist, oder sich ein Systemfehler eingeschlichen hat, den wir noch nicht kennen. Wir haben daher");
            sb.AppendLine(
                "den Wirt gebeten, seine Reservierungsanfragen zu Beginn der Schicht kurz per E-Mail zu bestätigen. Passiert das nicht,");
            sb.AppendLine(
                "frag ruhig kurz nach, sei aber nett, es ist für den Wirt genauso neu, wie für Dich. Das Restaurant ist unter der");
            sb.Append("Telefonnummer ");
            sb.Append(order.CartInfo.RestaurantPhone);
            sb.Append(" zu erreichen.");
            sb.Append("</p>");

            sb.Append("<p>");
            sb.AppendLine(
                "Wenn ein Problem aufgetreten ist, das ihr nicht lösen konntet, melde Dich doch gerne unter support@gastromio.de!");
            sb.Append("Wir nehmen Deinen Hinweis gerne auf.");
            sb.Append("</p>");
        }

        private static void AppendOrderRestaurantInfoForCustomer(Order order, StringBuilder sb)
        {
            sb.Append("<p>");
            sb.Append("Deine Bestellung bei ");
            sb.Append(order.CartInfo.RestaurantName);
            sb.Append(":");
            sb.Append("</p>");
        }

        public EmailData GetRestaurantEmail(Order order)
        {
            switch (order.CartInfo.OrderType)
            {
                case OrderType.Pickup:
                    return GetRestaurantEmailForPickup(order);
                case OrderType.Delivery:
                    return GetRestaurantEmailForDelivery(order);
                case OrderType.Reservation:
                    return GetRestaurantEmailForReservation(order);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private static EmailData GetRestaurantEmailForPickup(Order order)
        {
            var sb = new StringBuilder();
            AppendRestaurantSalutation(order, sb);
            AppendOrderReceptionForRestaurant(sb);
            AppendOrderDetails(sb, order);

            sb.Append("<p>");
            sb.Append("Die gewählte Bestellart ist: Abholung. Der Besteller holt die Bestellung ab.");
            sb.Append("</p>");

            AppendServiceTime(sb, order);
            AppendGastromioInfoForRestaurantOrder(order, sb);
            AppendGreetings(sb);

            var customerInfo =
                $"({order.CustomerInfo.GivenName} {order.CustomerInfo.LastName}, {order.CustomerInfo.Email}, {order.CustomerInfo.Phone})";
            var message = sb.ToString();
            return new EmailData
            {
                Subject = $"Gastromio.de - Neue Bestellung von {customerInfo}",
                TextPart = message
                    .Replace("<br/>", "\r\n")
                    .Replace("<p>", "")
                    .Replace("</p>", "\r\n"),
                HtmlPart = message
            };
        }

        private static EmailData GetRestaurantEmailForDelivery(Order order)
        {
            var sb = new StringBuilder();
            AppendRestaurantSalutation(order, sb);
            AppendOrderReceptionForRestaurant(sb);
            AppendOrderDetails(sb, order);

            sb.Append("<p>");
            sb.Append("Die gewählte Bestellart ist: Lieferung. Bitte dem Besteller die Bestellung zur gewünschten Adresse liefern.");
            sb.Append("</p>");

            AppendServiceTime(sb, order);
            AppendGastromioInfoForRestaurantOrder(order, sb);
            AppendGreetings(sb);

            var customerInfo =
                $"({order.CustomerInfo.GivenName} {order.CustomerInfo.LastName} {order.CustomerInfo.Street}, {order.CustomerInfo.ZipCode} {order.CustomerInfo.City})";
            var message = sb.ToString();
            return new EmailData
            {
                Subject = $"Gastromio.de - Neue Bestellung von {customerInfo}",
                TextPart = message
                    .Replace("<br/>", "\r\n")
                    .Replace("<p>", "")
                    .Replace("</p>", "\r\n"),
                HtmlPart = message
            };
        }

        private static EmailData GetRestaurantEmailForReservation(Order order)
        {
            var sb = new StringBuilder();
            AppendRestaurantSalutation(order, sb);
            AppendReservationReceptionForRestaurant(sb);
            AppendServiceTime(sb, order);

            sb.Append("<p>");
            sb.Append("Reservierungsanfrage:");
            sb.AppendLine("<br/>");

            sb.Append(order.CustomerInfo.GivenName);
            sb.Append(" ");
            sb.Append(order.CustomerInfo.LastName);
            sb.AppendLine("<br/>");

            sb.AppendLine(order.CustomerInfo.Street);
            sb.Append(order.CustomerInfo.ZipCode);
            sb.Append(" ");
            sb.Append(order.CustomerInfo.City);
            sb.AppendLine("<br/>");

            sb.Append("Telefonnummer: ");
            sb.Append(order.CustomerInfo.Phone);
            sb.AppendLine("<br/>");

            sb.Append("E-Mail-Adresse: ");
            sb.Append(order.CustomerInfo.Email);

            if (!string.IsNullOrWhiteSpace(order.CustomerInfo.AddAddressInfo))
            {
                sb.AppendLine("<br/>");
                sb.Append("Zusatzinformationen: ");
                sb.Append(order.CustomerInfo.AddAddressInfo);
            }

            sb.Append("</p>");

            AppendGastromioInfoForRestaurantReservation(order, sb);
            AppendGreetings(sb);

            var customerInfo =
                $"({order.CustomerInfo.GivenName} {order.CustomerInfo.LastName} {order.CustomerInfo.Street}, {order.CustomerInfo.ZipCode} {order.CustomerInfo.City})";
            var message = sb.ToString();
            return new EmailData
            {
                Subject = $"Gastromio.de - Neue Reservierungsanfrage von {customerInfo}",
                TextPart = message
                    .Replace("<br/>", "\r\n")
                    .Replace("<p>", "")
                    .Replace("</p>", "\r\n"),
                HtmlPart = message
            };
        }

        private static void AppendRestaurantSalutation(Order order, StringBuilder sb)
        {
            sb.Append("<p>");
            sb.Append("Liebes Restaurant ");
            sb.Append(order.CartInfo.RestaurantName);
            sb.Append(",");
            sb.Append("</p>");
        }

        private static void AppendOrderReceptionForRestaurant(StringBuilder sb)
        {
            sb.Append("<p>");
            sb.Append("Wir haben eine neue Bestellung für Dich über Gastromio.de entgegengenommen:");
            sb.Append("</p>");
        }

        private static void AppendReservationReceptionForRestaurant(StringBuilder sb)
        {
            sb.Append("<p>");
            sb.Append("Wir haben eine neue Reservierungsanfrage für Dich über Gastromio.de entgegengenommen:");
            sb.Append("</p>");
        }

        private static void AppendGastromioInfoForRestaurantOrder(Order order, StringBuilder sb)
        {
            sb.Append("<p>");
            sb.AppendLine("Noch ein wichtiger Hinweis:");
            sb.Append("</p>");

            sb.Append("<p>");
            sb.AppendLine(
                "Gastromio.de wurde ehrenamtlich von Helfern der Coronahilfe-Bocholt erstellt, um Deine Existenz als Gastronom in dieser");
            sb.AppendLine(
                "schweren Zeit zu erhalten. Weder Du noch Dein Kunde haben durch Gastromio.de irgendwelche Kosten. Gastromio.de wurde erst");
            sb.AppendLine(
                "kürzlich in Betrieb genommen. Es ist also möglich, dass der Kunde noch unsicher ist, ob die Bestellabwicklung über Gastromio");
            sb.AppendLine(
                "funktioniert. Wir haben ihm daher zugesagt, dass Du die Bestellung zu Beginn Deiner Schicht kurz per E-Mail bestätigen würdest.");
            sb.Append(
                "Falls er etwas bestellt hat, das aktuell nicht lieferbar ist, rufe ihn bitte an, er ist unter Telefonnummer ");
            sb.AppendLine(order.CustomerInfo.Phone);
            sb.Append("bzw. unter der E-Mail-Adresse ");
            sb.Append(order.CustomerInfo.Email);
            sb.Append(" zu erreichen.");
            sb.Append("</p>");

            sb.Append("<p>");
            sb.AppendLine(
                "Wenn ein Problem aufgetreten ist, das Ihr nicht lösen konntet, melde es doch gerne unter support@gastromio.de! Wir nehmen Deinen");
            sb.Append("Hinweis gerne auf.");
            sb.Append("</p>");

            sb.Append("<p>");
            sb.AppendLine(
                "Solltest Du Deine Öffnungszeiten ändern oder Deine Speisekarte anpassen wollen und dabei Schwierigkeiten haben, kannst Du auch");
            sb.AppendLine(
                "gerne bei unserer Hotline nachfragen: 02871-287381 oder aber eine E-Mail an hotline@coronahilfe-bocholt.de schicken");
            sb.Append("</p>");

            sb.Append("<p>");
            sb.AppendLine(
                "Denke bitte daran: Alle Helfer, die Dein Angebot über Gastromio.de im Internet und auf verschiedenen Sozialen Medien");
            sb.AppendLine(
                "kommunizieren, tun das ehrenamtlich nach bestem Wissen und Gewissen, und völlig ohne Gegenleistung. Oft sind es Stammkunden,");
            sb.AppendLine(
                "die möchten, dass Du weiter existierst, aber vielleicht macht auch einer mal einen Fehler dabei. Bitte sei also nett, wenn Du");
            sb.AppendLine(
                "etwas zu kritisieren hast und erkläre uns einfach, was Du Dir anders wünschst, sie alle tun ihr Bestes.");
            sb.Append("</p>");
        }

        private static void AppendGastromioInfoForRestaurantReservation(Order order, StringBuilder sb)
        {
            sb.Append("<p>");
            sb.AppendLine("Noch ein wichtiger Hinweis:");
            sb.Append("</p>");

            sb.Append("<p>");
            sb.AppendLine(
                "Gastromio.de wurde ehrenamtlich von Helfern der Coronahilfe-Bocholt erstellt, um Deine Existenz als Gastronom in dieser");
            sb.AppendLine(
                "schweren Zeit zu erhalten. Weder Du noch Dein Kunde haben durch Gastromio.de irgendwelche Kosten. Gastromio.de wurde erst");
            sb.AppendLine(
                "kürzlich in Betrieb genommen. Es ist also möglich, dass der Kunde noch unsicher ist, ob die Abwicklung von Reservierungsanfragen über Gastromio");
            sb.AppendLine(
                "funktioniert. Wir haben ihm daher zugesagt, dass Du die Reservierungsanfrage zu Beginn Deiner Schicht kurz per E-Mail bestätigen würdest.");
            sb.Append(
                "Falls es Gesprächsbedarf zu seiner Reservierungsanfrage gibt, rufe ihn bitte an, er ist unter Telefonnummer ");
            sb.AppendLine(order.CustomerInfo.Phone);
            sb.Append("bzw. unter der E-Mail-Adresse ");
            sb.Append(order.CustomerInfo.Email);
            sb.Append(" zu erreichen.");
            sb.Append("</p>");

            sb.Append("<p>");
            sb.AppendLine(
                "Wenn ein Problem aufgetreten ist, das Ihr nicht lösen konntet, melde es doch gerne unter support@gastromio.de! Wir nehmen Deinen");
            sb.Append("Hinweis gerne auf.");
            sb.Append("</p>");

            sb.Append("<p>");
            sb.AppendLine(
                "Solltest Du Deine Öffnungszeiten ändern oder Deine Speisekarte anpassen wollen und dabei Schwierigkeiten haben, kannst Du auch");
            sb.AppendLine(
                "gerne bei unserer Hotline nachfragen: 02871-287381 oder aber eine E-Mail an hotline@coronahilfe-bocholt.de schicken");
            sb.Append("</p>");

            sb.Append("<p>");
            sb.AppendLine(
                "Denke bitte daran: Alle Helfer, die Dein Angebot über Gastromio.de im Internet und auf verschiedenen Sozialen Medien");
            sb.AppendLine(
                "kommunizieren, tun das ehrenamtlich nach bestem Wissen und Gewissen, und völlig ohne Gegenleistung. Oft sind es Stammkunden,");
            sb.AppendLine(
                "die möchten, dass Du weiter existierst, aber vielleicht macht auch einer mal einen Fehler dabei. Bitte sei also nett, wenn Du");
            sb.AppendLine(
                "etwas zu kritisieren hast und erkläre uns einfach, was Du Dir anders wünschst, sie alle tun ihr Bestes.");
            sb.Append("</p>");
        }

        public EmailData GetRequestPasswordChangeEmail(string email, string url)
        {
            var sb = new StringBuilder();

            sb.Append("<p>");
            sb.Append("Hallo ");
            sb.Append(email);
            sb.Append(",");
            sb.Append("</p>");

            sb.Append("<p>");
            sb.Append("wir haben über Gastromio die Aufforderung erhalten, Dein Passwort zurückzusetzen!");
            sb.Append("<br/>");
            sb.Append("Wenn das ein Versehen war oder nicht von Dir veranlasst worden ist, kannst Du diese E-Mail einfach ignorieren. Dein Passwort wurde nicht geändert!");
            sb.Append("</p>");

            sb.Append("<p>");
            sb.AppendLine("Falls Du doch Dein Passwort ändern möchtest, klicke bitte auf den folgenden Link und folge den dort genannten Anweisungen:");
            sb.Append("</p>");

            sb.Append("<a href='");
            sb.Append(url);
            sb.Append("'>");
            sb.Append(url);
            sb.Append("</a>");

            sb.AppendLine("<br/>");

            sb.Append("<p>");
            sb.AppendLine("Dein Gastromio-Team");
            sb.Append("</p>");

            var message = sb.ToString();

            return new EmailData
            {
                Subject = $"Gastromio.de - Anforderung zur Änderung Deines Passworts",
                TextPart = message
                    .Replace("<br/>", "\r\n")
                    .Replace("<p>", "")
                    .Replace("</p>", "\r\n"),
                HtmlPart = message
            };
        }

        public string GetRestaurantMobileMessage(Order order)
        {
            var sb = new StringBuilder();
            sb.Append("Liebes Restaurant ");
            sb.Append(order.CartInfo.RestaurantName);
            sb.Append(", wir haben eine ");
            if (order.CartInfo.OrderType == OrderType.Reservation)
            {
                sb.Append("Reservierungsanfrage");
            }
            else
            {
                sb.Append("Bestellung");
            }
            sb.Append(" von ");
            sb.Append(order.CustomerInfo.GivenName);
            sb.Append(" ");
            sb.Append(order.CustomerInfo.LastName);
            sb.Append(" für Dich. Details in Deinen Emails.");
            return sb.ToString();
        }

        private static void AppendOrderDetails(StringBuilder sb, Order order)
        {
            sb.Append("<p>");
            foreach (var orderedDish in order.CartInfo.OrderedDishes)
            {
                sb.Append(orderedDish.Count);
                sb.Append("x ");

                sb.Append(orderedDish.DishName);

                if (!string.IsNullOrEmpty(orderedDish.VariantName))
                {
                    sb.Append(" (");
                    sb.Append(orderedDish.VariantName);
                    sb.Append(")");
                }

                sb.Append(": ");

                sb.Append(orderedDish.VariantPrice.ToString("0.00"));
                sb.Append("€");

                if (!string.IsNullOrEmpty(orderedDish.Remarks))
                {
                    sb.Append(" (");
                    sb.Append(orderedDish.Remarks);
                    sb.Append(")");
                }

                sb.AppendLine("<br/>");
            }

            sb.Append("</p>");

            sb.Append("<p>");
            if (order.Costs > 0)
            {
                sb.Append("Lieferkosten: ");
                sb.Append(order.Costs.ToString("0.00"));
                sb.Append("€");
                sb.AppendLine("<br/>");
            }

            sb.Append("Gesamtpreis: ");
            sb.Append(order.TotalPrice.ToString("0.00"));
            sb.Append("€");
            sb.AppendLine("<br/>");

            sb.Append("Zahlungsmethode: ");
            sb.Append(order.PaymentMethodName);
            sb.Append("</p>");

            sb.Append("<p>");
            sb.Append("Bestelldetails:");
            sb.AppendLine("<br/>");

            sb.Append(order.CustomerInfo.GivenName);
            sb.Append(" ");
            sb.Append(order.CustomerInfo.LastName);
            sb.AppendLine("<br/>");

            if (order.CartInfo.OrderType == OrderType.Delivery)
            {
                sb.AppendLine(order.CustomerInfo.Street);
                sb.Append(order.CustomerInfo.ZipCode);
                sb.Append(" ");
                sb.Append(order.CustomerInfo.City);
                sb.AppendLine("<br/>");
            }

            sb.Append("Telefonnummer: ");
            sb.Append(order.CustomerInfo.Phone);
            sb.AppendLine("<br/>");

            sb.Append("E-Mail-Adresse: ");
            sb.Append(order.CustomerInfo.Email);

            if (!string.IsNullOrWhiteSpace(order.CustomerInfo.AddAddressInfo))
            {
                sb.AppendLine("<br/>");
                sb.Append("Zusatzinformationen: ");
                sb.Append(order.CustomerInfo.AddAddressInfo);
            }

            sb.Append("</p>");
        }

        private static void AppendServiceTime(StringBuilder sb, Order order)
        {
            sb.Append("<p>");
            sb.Append("Gewünschtes Datum / Uhrzeit: ");
            if (order.ServiceTime.HasValue)
            {
                var localServiceTime = order.ServiceTime.Value.LocalDateTime;
                sb.Append(localServiceTime.ToLongDateString());
                sb.Append(" ");
                sb.Append(localServiceTime.ToLongTimeString());
            }
            else
            {
                sb.Append("Schnellstmöglich");
            }

            sb.Append("</p>");
        }
    }
}
