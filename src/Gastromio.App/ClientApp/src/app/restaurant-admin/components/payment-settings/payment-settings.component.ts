import { Component, OnInit } from '@angular/core';

import {Observable} from "rxjs";
import {map} from "rxjs/operators";

import {PaymentMethodModel} from "../../../shared/models/payment-method.model";

import {RestaurantAdminFacade} from "../../restaurant-admin.facade";

@Component({
  selector: 'app-payment-settings',
  templateUrl: './payment-settings.component.html',
  styleUrls: [
    './payment-settings.component.css',
    '../../../../assets/css/frontend_v3.min.css',
    '../../../../assets/css/backend_v2.min.css'
  ]
})
export class PaymentSettingsComponent implements OnInit {

  paymentMethods$: Observable<PaymentMethodModel[]>;

  constructor(
    private facade: RestaurantAdminFacade
  ) { }

  ngOnInit(): void {
    this.paymentMethods$ = this.facade.getPaymentMethods$();
  }

  isPaymentMethodEnabled$(paymentMethodId: string): Observable<boolean> {
    return this.facade.getPaymentMethodStatus$()
      .pipe(
        map(status => {
          return status[paymentMethodId];
        })
      );
  }

  onTogglePaymentMethod(paymentMethodId: string): void {
    this.facade.togglePaymentMethod(paymentMethodId);
  }

}
