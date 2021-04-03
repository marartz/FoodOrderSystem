import {Component, OnDestroy, OnInit} from '@angular/core';

import {Observable} from 'rxjs';
import {map} from 'rxjs/operators';

import {NgbModal} from '@ng-bootstrap/ng-bootstrap';

import {BlockUI, NgBlockUI} from 'ng-block-ui';

import {RestaurantModel} from '../../../shared/models/restaurant.model';
import {CuisineModel} from '../../../shared/models/cuisine.model';

import {OrderFacade} from "../../../order/order.facade";
import {OrderType} from '../../../order/models/order-type';
import {OrderTypeConverter} from "../../../order/models/order-type-converter";

import {OpeningHourFilterComponent} from '../opening-hour-filter/opening-hour-filter.component';

@Component({
  selector: 'app-order-restaurants',
  templateUrl: './order-restaurants.component.html',
  styleUrls: [
    './order-restaurants.component.css',
    '../../../../assets/css/frontend_v3.min.css',
    '../../../../assets/css/components/_1_hero.min.css',
	'../../../../assets/css/components/_2_action-bar.min.css',
	'../../../../assets/css/components/_2_restaurants-row.min.css',
	'../../../../assets/css/components/_3_advanced-filter.min.css'
  ]
})
export class OrderRestaurantsComponent implements OnInit, OnDestroy {
  @BlockUI() blockUI: NgBlockUI;

  cuisines: CuisineModel[];

  selectedSearchPhrase$: Observable<string>;
  selectedOrderType$: Observable<string>;
  selectedOrderTime$: Observable<Date>;
  selectedOrderTimeText$: Observable<string>;

  showMobileFilterDetails: boolean;

  totalRestaurantCount$: Observable<number>;

  openedRestaurants$: Observable<RestaurantModel[]>;
  openedRestaurantCount$: Observable<number>;
  closedRestaurants$: Observable<RestaurantModel[]>;
  closedRestaurantCount$: Observable<number>;

  constructor(
    private orderFacade: OrderFacade,
    private modalService: NgbModal,
  ) {
  }

  ngOnInit() {
    this.orderFacade.setSelectedOrderTypeIfNotSet(OrderType.Pickup);

    this.orderFacade.getIsSearching$()
      .subscribe(isSearching => {
        if (isSearching) {
          this.blockUI.start('Suche läuft');
        } else {
          this.blockUI.stop();
        }
      });

    this.selectedSearchPhrase$ = this.orderFacade.getSelectedSearchPhrase$();

    this.selectedOrderType$ = this.orderFacade.getSelectedOrderType$()
      .pipe(
        map(selectedOrderType => OrderTypeConverter.convertToString(selectedOrderType))
      );

    this.selectedOrderTime$ = this.orderFacade.getSelectedOrderTime$();

    this.selectedOrderTimeText$ = this.selectedOrderTime$
      .pipe(
        map(selectedOrderTime => {
          if (!selectedOrderTime)
            return undefined;

          let hoursText = selectedOrderTime.getHours().toString();
          hoursText = ('0' + hoursText).slice(-2);

          let minutesText = selectedOrderTime.getMinutes().toString();
          minutesText = ('0' + minutesText).slice(-2);

          return selectedOrderTime.toLocaleDateString() + ', ' + hoursText + ':' + minutesText;
        })
      );

    this.orderFacade.getCuisines$()
      .subscribe((cuisines) => {
        this.cuisines = cuisines;
      });

    this.totalRestaurantCount$ = this.orderFacade.getRestaurants$()
      .pipe(
        map(restaurants => restaurants?.length ?? 0)
      )

    this.openedRestaurants$ = this.orderFacade.getOpenedRestaurants$();
    this.openedRestaurantCount$ = this.orderFacade.getOpenedRestaurants$()
      .pipe(
        map(openedRestaurants => openedRestaurants?.length ?? 0)
      );

    this.closedRestaurants$ = this.orderFacade.getClosedRestaurants$();
    this.closedRestaurantCount$ = this.orderFacade.getClosedRestaurants$()
      .pipe(
        map(closedRestaurants => closedRestaurants?.length ?? 0)
      );
  }

  ngOnDestroy() {
  }

  onToggleMobileFilterDetails(): void {
    this.showMobileFilterDetails = !this.showMobileFilterDetails;
  }

  onHideMobileFilterDetails(): void {
    this.showMobileFilterDetails = false;
  }

  openOpeningHourFilter(): void {
    const modalRef = this.modalService.open(OpeningHourFilterComponent, {centered: true});
    modalRef.componentInstance.value = this.orderFacade.getSelectedOrderTime() ?? new Date();
    modalRef.result.then((value: Date) => {
      this.orderFacade.setSelectedOrderTime(value);
    }, () => {
    });
  }

  getBannerStyle(
    restaurant: RestaurantModel
  ): string {
    if (!restaurant) {
      return undefined;
    }
    return '/api/v1/restaurants/' + restaurant.id + '/images/banner';
  }

  onDeliverySelected(): void {
    this.orderFacade.setSelectedOrderType(OrderType.Delivery);
  }

  onPickupSelected(): void {
    this.orderFacade.setSelectedOrderType(OrderType.Pickup);
  }

  onReservationSelected(): void {
    this.orderFacade.setSelectedOrderType(OrderType.Reservation);
  }

  onSearchType(value: string) {
    this.orderFacade.setSelectedSearchPhrase(value);
  }

  isCuisineSelected(cuisine: CuisineModel): boolean {
    const selectedCuisine = this.orderFacade.getSelectedCuisine();
    if (cuisine) {
      return selectedCuisine === cuisine.id;
    } else {
      return selectedCuisine === '';
    }
  }

  selectCuisine(cuisine: CuisineModel): void {
    if (!cuisine) {
      this.orderFacade.setSelectedCuisine('');
    }
    else
    {
      this.orderFacade.setSelectedCuisine(cuisine.id);
    }
  }

}
