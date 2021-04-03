import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';

import {CheckoutComponent} from './components/checkout/checkout.component';
import {OrderHomeComponent} from './components/order-home/order-home.component';
import {OrderRestaurantsComponent} from './components/order-restaurants/order-restaurants.component';
import {OrderRestaurantComponent} from './components/order-restaurant/order-restaurant.component';
import {OrderSummaryComponent} from './components/order-summary/order-summary.component';
import {ReservationComponent} from "./components/reservation/reservation.component";

const routes: Routes = [
  {path: '', component: OrderHomeComponent},
  {path: 'restaurants', component: OrderRestaurantsComponent},
  {path: 'restaurants/:restaurantId', component: OrderRestaurantComponent},
  {path: 'checkout', component: CheckoutComponent},
  {path: 'order-summary', component: OrderSummaryComponent},
  {path: 'restaurants/:restaurantId/reservation', component: ReservationComponent},
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OrderUiRoutingModule {
}
