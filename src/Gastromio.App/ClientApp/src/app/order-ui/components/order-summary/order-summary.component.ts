import {Component, OnInit} from '@angular/core';

import {filter} from "rxjs/operators";

import {RestaurantModel} from '../../../shared/models/restaurant.model';

import {OrderModel} from '../../../order/models/order.model';

import {OrderFacade} from "../../../order/order.facade";
import {BlockUI, NgBlockUI} from "ng-block-ui";

@Component({
  selector: 'app-order-summary',
  templateUrl: './order-summary.component.html',
  styleUrls: ['./order-summary.component.css', '../../../../assets/css/frontend_v3.min.css']
})
export class OrderSummaryComponent implements OnInit {

  @BlockUI() blockUI: NgBlockUI;

  initialized: boolean = false;
  restaurant: RestaurantModel;
  order: OrderModel;

  constructor(
    private orderFacade: OrderFacade
  ) {
  }

  ngOnInit() {
    this.orderFacade.getIsInitializing$()
      .subscribe(isInitializing => {
        if (isInitializing) {
          this.blockUI.start();
        } else {
          this.blockUI.stop();
        }
      });

    this.orderFacade.getIsInitialized$()
      .pipe(
        filter(isInitialized => isInitialized === true)
      )
      .subscribe(() => {
        this.initialized = true;
        this.restaurant = this.orderFacade.getSelectedRestaurant();
        this.order = this.orderFacade.getOrder();
        this.orderFacade.resetCheckout();
      });
  }

  getOrderTypeText(): string {
    return this.order.cartInfo.orderType === 2 ? "Reservierungsanfrage" : "Bestellung";
  }

}
