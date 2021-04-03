import {Component, OnDestroy, OnInit} from '@angular/core';
import {Title} from '@angular/platform-browser';
import {ActivatedRoute, Router} from '@angular/router';

import {combineLatest} from 'rxjs';
import {filter, take, tap} from 'rxjs/operators';

import {NgbModal} from '@ng-bootstrap/ng-bootstrap';

import {BlockUI, NgBlockUI} from 'ng-block-ui';

import {HttpErrorHandlingService} from '../../../shared/services/http-error-handling.service';

import {RestaurantModel} from '../../../shared/models/restaurant.model';
import {DishCategoryModel} from '../../../shared/models/dish-category.model';
import {DishModel} from '../../../shared/models/dish.model';

import {CartModel} from '../../../order/models/cart.model';
import {CartDishModel} from '../../../order/models/cart-dish.model';
import {OrderType} from "../../../order/models/order-type";

import {OrderFacade} from '../../../order/order.facade';

import {AddDishToCartComponent} from '../add-dish-to-cart/add-dish-to-cart.component';
import {EditCartDishComponent} from '../edit-cart-dish/edit-cart-dish.component';
import {OrderRestaurantOpeningHoursComponent} from '../order-restaurant-opening-hours/order-restaurant-opening-hours.component';
import {OrderRestaurantImprintComponent} from '../order-restaurant-imprint/order-restaurant-imprint.component';

@Component({
  selector: 'app-order-restaurant',
  templateUrl: './order-restaurant.component.html',
  styleUrls: [
    './order-restaurant.component.css',
    '../../../../assets/css/frontend_v3.min.css',
    '../../../../assets/css/components/_1_hero.min.css',
	'../../../../assets/css/components/_2_action-bar.min.css',
	'../../../../assets/css/components/_3_advanced-filter.min.css'
  ]
})
export class OrderRestaurantComponent implements OnInit, OnDestroy {
  @BlockUI() blockUI: NgBlockUI;

  url: string;

      orderType: string;

  generalError: string;

  initialized: boolean;

  restaurantId: string;
  restaurant: RestaurantModel;
  openingHours: string;
  dishCategories: DishCategoryModel[];

  searchPhrase: string;
  filteredDishCategories: DishCategoryModel[];

  allowCart: boolean;

  proceedError: string;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private titleService: Title,
    private orderFacade: OrderFacade,
    private httpErrorHandlingService: HttpErrorHandlingService,
    private modalService: NgbModal,
  ) {
  }

  ngOnInit() {
    this.url = this.router.url;
    this.initialized = false;

    this.blockUI.start('Restaurant wird geladen ...');

    let orderType: OrderType;
    let serviceTime: Date;

    const observables = [
      this.orderFacade.getIsInitialized$().pipe(
        filter(isInitialized => isInitialized === true)
      ),
      this.route.paramMap.pipe(tap(params => {
        this.restaurantId = params.get('restaurantId');
      })),
      this.route.queryParams.pipe(tap(params => {
        const orderTypeText = params.orderType?.toLocaleLowerCase();
        if (!orderTypeText) {
          orderType = OrderType.Pickup;
        } else {
          switch (orderTypeText) {
            case 'pickup':
              orderType = OrderType.Pickup;
              break;
            case 'delivery':
              orderType = OrderType.Delivery;
              break;
            case 'reservation':
              orderType = OrderType.Reservation;
              break;
            default:
              this.blockUI.stop();
              this.generalError = 'Unbekannte Bestellart: ' + orderTypeText;
          }
        }

        this.allowCart = orderType !== OrderType.Reservation;

        const serviceTimeText = params.serviceTime;
        if (serviceTimeText) {
          try {
            const dt = serviceTimeText.split(/[: T-]/).map(parseFloat);
            serviceTime = new Date(Date.UTC(dt[0], dt[1] - 1, dt[2], dt[3] || 0, dt[4] || 0, dt[5] || 0, 0));
          }
          catch {}
        }
      }))
    ];

    combineLatest(observables).pipe(take(1)).subscribe(() => {
      this.orderFacade.selectRestaurantId$(this.restaurantId).subscribe(() => {
        this.blockUI.stop();

        try {
          this.restaurant = this.orderFacade.getSelectedRestaurant();
          this.dishCategories = this.orderFacade.getDishCategoriesOfSelectedRestaurant();
          this.openingHours = this.restaurant.openingHoursTodayText;

          const cart = this.orderFacade.getCart();

          if (!cart || cart.getOrderType() !== orderType || cart.getServiceTime() != serviceTime) {
            this.orderFacade.startOrder(orderType, serviceTime);
          }

          this.filterDishCategories();

          this.titleService.setTitle(this.restaurant.name + ' - Gastromio');

          this.initialized = true;
        }
        catch (e) {
          if (e instanceof Error) {
            this.generalError = e.message;
          }
          else {
            throw e;
          }
        }
      }, error => {
        console.log('error: ', error);
        this.blockUI.stop();
        this.generalError = this.httpErrorHandlingService.handleError(error).getJoinedGeneralErrors();
      });
    });
  }

  ngOnDestroy() {
  }

  getBannerStyle(): string {
    if (!this.restaurant) {
      return undefined;
    }
    return 'url(\'/api/v1/restaurants/' + this.restaurant.id + '/images/banner' + '\')';
  }

  showReservationValidFrom(): boolean {
    const now = new Date();
    return now < this.orderFacade.getStartDateOfReservation();
  }

  scrollToDishCategory(dishCategoryId: string): void {
    const element = document.querySelector('#dc' + dishCategoryId);
    if (element === null) {
      return;
    }
    element.scrollIntoView();
  }

  scrollToExternalMenu(externalMenuId: string): void {
    const element = document.querySelector('#em' + externalMenuId);
    if (element === null) {
      return;
    }
    element.scrollIntoView();
  }

  isCartVisible(): boolean {
    const cart = this.orderFacade.getCart();
    return cart ? cart.isVisible() : false;
  }

  hideCart() {
    this.orderFacade.hideCart();
  }

  getCart(): CartModel {
    return this.orderFacade.getCart();
  }

  openOpeningHoursModal(): void {
    const modalRef = this.modalService.open(OrderRestaurantOpeningHoursComponent);
    modalRef.result.then(() => {
    }, () => {
    });
  }

  openImprintModal(): void {
    const modalRef = this.modalService.open(OrderRestaurantImprintComponent);
    modalRef.componentInstance.restaurant = this.restaurant;
    modalRef.result.then(() => {
    }, () => {
    });
  }

  onSearchTextChanged(searchPhrase: string): void {
    if (!searchPhrase || searchPhrase.trim().length === 0) {
      this.searchPhrase = undefined;
    } else {
      this.searchPhrase = searchPhrase.toLocaleLowerCase();
    }
    this.filterDishCategories();
  }

  filterDishCategories(): void {
    if (!this.searchPhrase) {
      this.filteredDishCategories = this.dishCategories;
      return;
    }

    this.filteredDishCategories = new Array<DishCategoryModel>();

    for (let dishCategory of this.dishCategories) {
      let hasMatch = false;

      let dishCategoryClone = new DishCategoryModel();
      dishCategoryClone.id = dishCategory.id;
      dishCategoryClone.name = dishCategory.name;
      dishCategoryClone.dishes = new Array<DishModel>();

      for (let dish of dishCategory.dishes) {
        const nameContainsSearchPhrase = dish.name && dish.name.toLocaleLowerCase().indexOf(this.searchPhrase) > -1;
        const descriptionContainsSearchPhrase = dish.description && dish.description.toLocaleLowerCase().indexOf(this.searchPhrase) > -1;
        if (nameContainsSearchPhrase || descriptionContainsSearchPhrase) {
          dishCategoryClone.dishes.push(dish);
          hasMatch = true;
        }
      }

      if (hasMatch) {
        this.filteredDishCategories.push(dishCategoryClone);
      }
    }
  }

  getFirstDishVariant(dish: DishModel): string {
    if (dish === undefined || dish.variants === undefined || dish.variants.length === 0) {
      return '0,00';
    }

    return dish.variants[0].price.toLocaleString('de', {minimumFractionDigits: 2});
  }

  public onAddDishToCart(dish: DishModel): void {
    if (!this.allowCart)
      return;

    if (dish === undefined || dish.variants === undefined || dish.variants.length === 0) {
      return;
    }
    const modalRef = this.modalService.open(AddDishToCartComponent);
    modalRef.componentInstance.dish = dish;
    modalRef.result.then(() => {
      this.proceedError = undefined;
    }, () => {
    });
  }

  public onEditCartDish(cartDish: CartDishModel): void {
    const modalRef = this.modalService.open(EditCartDishComponent);
    modalRef.componentInstance.cartDish = cartDish;
    modalRef.result.then(() => {
      this.proceedError = undefined;
    }, () => {
    });
  }

  public onIncrementDishVariantCount(cartDishVariant: CartDishModel): void {
    if (cartDishVariant === undefined) {
      return;
    }
    this.orderFacade.incrementCountOfCartDish(cartDishVariant.getItemId());
    this.proceedError = undefined;
  }

  public onDecrementDishVariantCount(cartDishVariant: CartDishModel): void {
    if (cartDishVariant === undefined) {
      return;
    }
    this.orderFacade.decrementCountOfCartDish(cartDishVariant.getItemId());
    this.proceedError = undefined;
  }

  public onRemoveDishVariantFromCart(cartDishVariant: CartDishModel): void {
    if (cartDishVariant === undefined) {
      return;
    }
    this.orderFacade.removeCartDishFromCart(cartDishVariant.getItemId());
    this.proceedError = undefined;
  }

  public proceedToCheckout(): void {
    if (!this.getCart().isValid()) {
      this.proceedError = this.getCart().getValidationError();
      return;
    }
    this.proceedError = undefined;
    this.router.navigateByUrl('/checkout');
  }

  public isReservationEnabled(): boolean {
    return this.restaurant.reservationInfo?.enabled;
  }

  public hasExternalReservationSystem(): boolean {
    return this.restaurant.reservationInfo.reservationSystemUrl &&
      this.restaurant.reservationInfo.reservationSystemUrl.length > 0;
  }

  toggleCartVisibility(): void {
    const cart = this.orderFacade.getCart();
    if (!cart) {
      return;
    }
    if (cart.isVisible()) {
      this.orderFacade.hideCart();
    } else {
      this.orderFacade.showCart();
    }
  }
}
