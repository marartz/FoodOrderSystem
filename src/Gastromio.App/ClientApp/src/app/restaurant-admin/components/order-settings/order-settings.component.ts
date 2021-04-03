import {Component, OnDestroy, OnInit} from '@angular/core';
import {FormBuilder, FormGroup} from "@angular/forms";
import {debounceTime} from "rxjs/operators";

import {RestaurantAdminFacade} from "../../restaurant-admin.facade";
import {Subscription} from "rxjs";
import {ServiceInfoModel} from "../../../shared/models/restaurant.model";

@Component({
  selector: 'app-order-settings',
  templateUrl: './order-settings.component.html',
  styleUrls: [
    './order-settings.component.css',
    '../../../../assets/css/frontend_v3.min.css',
    '../../../../assets/css/backend_v2.min.css'
  ]
})
export class OrderSettingsComponent implements OnInit, OnDestroy {

  supportedOrderMode: string;

  pickupEnabled: boolean;
  deliveryEnabled: boolean;
  reservationEnabled: boolean;
  changeServiceInfoForm: FormGroup;

  subscription: Subscription;

  constructor(
    private facade: RestaurantAdminFacade,
    private formBuilder: FormBuilder
  ) {
    this.changeServiceInfoForm = this.formBuilder.group({
      pickupAverageTime: [null],
      pickupMinimumOrderValue: [null],
      pickupMaximumOrderValue: [null],
      deliveryAverageTime: [null],
      deliveryMinimumOrderValue: [null],
      deliveryMaximumOrderValue: [null],
      deliveryCosts: [null],
      reservationSystemUrl: [null],
      hygienicHandling: ['']
    });
    this.changeServiceInfoForm.valueChanges
      .pipe(debounceTime(1000))
      .subscribe(value => {
        if (this.changeServiceInfoForm.dirty && this.changeServiceInfoForm.valid) {
          this.updateData(
            this.pickupEnabled,
            this.deliveryEnabled,
            this.reservationEnabled,
            value
          );
        }
        this.changeServiceInfoForm.markAsPristine();
      });
  }

  ngOnInit(): void {
    this.subscription = this.facade.getRestaurant$().subscribe(restaurant => {

      this.supportedOrderMode = restaurant.supportedOrderMode;

      this.pickupEnabled = restaurant.pickupInfo?.enabled ?? false;
      this.deliveryEnabled = restaurant.deliveryInfo?.enabled ?? false;
      this.reservationEnabled = restaurant.reservationInfo?.enabled ?? false;

      this.changeServiceInfoForm.patchValue({
        pickupAverageTime: restaurant.pickupInfo?.averageTime ?? null,
        pickupMinimumOrderValue: restaurant.pickupInfo?.minimumOrderValue ?? null,
        pickupMaximumOrderValue: restaurant.pickupInfo?.maximumOrderValue ?? null,
        deliveryAverageTime: restaurant.deliveryInfo?.averageTime ?? null,
        deliveryMinimumOrderValue: restaurant.deliveryInfo?.minimumOrderValue ?? null,
        deliveryMaximumOrderValue: restaurant.deliveryInfo?.maximumOrderValue ?? null,
        deliveryCosts: restaurant.deliveryInfo?.costs ?? null,
        reservationSystemUrl: restaurant.reservationInfo.reservationSystemUrl,
        hygienicHandling: restaurant.hygienicHandling
      });

      this.changeServiceInfoForm.markAsPristine();
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  changeSupportedOrderMode(supportedOrderMode: string) {
    this.facade.changeSupportedOrderMode(supportedOrderMode);
  }

  togglePickupEnabled() {
    this.updateData(
      !this.pickupEnabled,
      this.deliveryEnabled,
      this.reservationEnabled,
      this.changeServiceInfoForm.value
    );
  }

  toggleDeliveryEnabled() {
    this.updateData(
      this.pickupEnabled,
      !this.deliveryEnabled,
      this.reservationEnabled,
      this.changeServiceInfoForm.value
    );
  }

  toggleReservationEnabled() {
    this.updateData(
      this.pickupEnabled,
      this.deliveryEnabled,
      !this.reservationEnabled,
      this.changeServiceInfoForm.value
    );
  }

  private updateData(pickupEnabled: boolean, deliveryEnabled: boolean, reservationEnabled: boolean, value: any): void {
    this.facade.changeServiceInfo(new ServiceInfoModel({
      pickupEnabled: pickupEnabled,
      pickupAverageTime: value.pickupAverageTime,
      pickupMinimumOrderValue: value.pickupMinimumOrderValue,
      pickupMaximumOrderValue: value.pickupMaximumOrderValue,
      deliveryEnabled: deliveryEnabled,
      deliveryAverageTime: value.deliveryAverageTime,
      deliveryMinimumOrderValue: value.deliveryMinimumOrderValue,
      deliveryMaximumOrderValue: value.deliveryMaximumOrderValue,
      deliveryCosts: value.deliveryCosts,
      reservationEnabled: reservationEnabled,
      reservationSystemUrl: value.reservationSystemUrl,
      hygienicHandling: value.hygienicHandling
    }));
  }

}
