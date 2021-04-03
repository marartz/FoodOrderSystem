import {Component, Input, OnDestroy, OnInit} from '@angular/core';

import {RestaurantModel} from '../../../shared/models/restaurant.model';

@Component({
  selector: 'app-order-restaurants-row',
  templateUrl: './order-restaurants-row.component.html',
  styleUrls: [
    './order-restaurants-row.component.css',
    '../../../../assets/css/frontend_v3.min.css',
	'../../../../assets/css/components/_2_restaurants-row.min.css'
  ]
})
export class OrderRestaurantsRowComponent implements OnInit, OnDestroy {

  @Input() restaurant: RestaurantModel;
  @Input() orderType: string;
  @Input() selectedOpeningHourFilter: Date;

  constructor() {
  }

  ngOnInit() {
  }

  ngOnDestroy() {
  }

  hasLogo(restaurant: RestaurantModel  ): boolean {
    return restaurant.imageTypes.some(en => en === 'logo');
  }

  getLogoUrl(restaurant: RestaurantModel): string {
    if (!restaurant) {
      return undefined;
    }
    return '/api/v1/restaurants/' + restaurant.id + '/images/logo';
  }

  hasBanner(restaurant: RestaurantModel): boolean {
    return restaurant.imageTypes.some(en => en === 'banner');
  }

  getBannerStyle(restaurant: RestaurantModel): string {
    if (!restaurant) {
      return undefined;
    }
    return '/api/v1/restaurants/' + restaurant.id + '/images/banner';
  }

  isRestaurantOpen(restaurant: RestaurantModel): boolean {
    const date = this.selectedOpeningHourFilter ?? new Date();
    return restaurant.isOpen(date);
  }

  getRestaurantClosedText(restaurant: RestaurantModel): string {
    const now = new Date();

    if (!this.selectedOpeningHourFilter) {
      return "Im Moment " + restaurant.getRestaurantClosedReason(now);
    } else {
      const nowDay = now.getDate();
      const nowMonth = now.getMonth() + 1;
      const day = this.selectedOpeningHourFilter.getDate();
      const month = this.selectedOpeningHourFilter.getMonth() + 1;
      const hour = this.selectedOpeningHourFilter.getHours();
      const minute = this.selectedOpeningHourFilter.getMinutes();

      if (day !== nowDay || month !== nowMonth) {
        return "Am " + day + "." + month + ". " + hour + ":" + minute + " Uhr " + restaurant.getRestaurantClosedReason(this.selectedOpeningHourFilter);
      } else {
        return "Um " + hour + ":" + minute + " Uhr " + restaurant.getRestaurantClosedReason(this.selectedOpeningHourFilter);
      }
    }
  }

}
