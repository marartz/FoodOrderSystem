import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';

import {NgbModule} from '@ng-bootstrap/ng-bootstrap';

import {BlockUIModule} from 'ng-block-ui';

import {OrderUiRoutingModule} from './order-ui.routing.module';

import {AddDishToCartComponent} from './components/add-dish-to-cart/add-dish-to-cart.component';
import {CheckoutComponent} from './components/checkout/checkout.component';
import {EditCartDishComponent} from './components/edit-cart-dish/edit-cart-dish.component';
import {OpeningHourFilterComponent} from './components/opening-hour-filter/opening-hour-filter.component';
import {OrderHomeComponent} from './components/order-home/order-home.component';
import {OrderRestaurantComponent} from './components/order-restaurant/order-restaurant.component';
import {OrderRestaurantImprintComponent} from './components/order-restaurant-imprint/order-restaurant-imprint.component';
import {OrderRestaurantsComponent} from './components/order-restaurants/order-restaurants.component';
import {OrderRestaurantsRowComponent} from './components/order-restaurants-row/order-restaurants-row.component';
import {OrderRestaurantOpeningHoursComponent} from './components/order-restaurant-opening-hours/order-restaurant-opening-hours.component';
import {OrderSummaryComponent} from './components/order-summary/order-summary.component';
import {ReservationComponent} from "./components/reservation/reservation.component";

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
    BlockUIModule.forRoot(),
    OrderUiRoutingModule,
  ],
  declarations: [
    AddDishToCartComponent,
    CheckoutComponent,
    EditCartDishComponent,
    OpeningHourFilterComponent,
    OrderHomeComponent,
    OrderRestaurantComponent,
    OrderRestaurantImprintComponent,
    OrderRestaurantOpeningHoursComponent,
    OrderRestaurantsComponent,
    OrderRestaurantsRowComponent,
    OrderSummaryComponent,
    ReservationComponent
  ],
  providers: [
  ]
})
export class OrderUiModule {
}
