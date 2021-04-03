import {Injectable} from "@angular/core";
import {HttpErrorResponse} from "@angular/common/http";

import {BehaviorSubject, combineLatest, Observable, of, throwError} from "rxjs";
import {catchError, debounceTime, distinctUntilChanged, map, switchMap, take} from "rxjs/operators";

import {CuisineModel} from "../shared/models/cuisine.model";
import {DishCategoryModel} from "../shared/models/dish-category.model";
import {DishModel} from "../shared/models/dish.model";
import {DishVariantModel} from "../shared/models/dish-variant.model";
import {RestaurantModel} from "../shared/models/restaurant.model";

import {HttpErrorHandlingService} from "../shared/services/http-error-handling.service";

import {CartModel} from "./models/cart.model";
import {CartDishModel} from "./models/cart-dish.model";
import {OrderType} from "./models/order-type";
import {OrderTypeConverter} from "./models/order-type-converter";
import {StoredCartModel} from "./models/stored-cart.model";

import {OrderService} from "./services/order.service";
import {StoredCartService} from "./services/stored-cart.service";
import {StoredCartDishModel} from "./models/stored-cart-dish.model";
import {Guid} from "guid-typescript";
import {CheckoutModel} from "./models/checkout.model";
import {OrderModel} from "./models/order.model";

@Injectable()
export class OrderFacade {

  private isInitializing$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(true);
  private isInitialized$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(undefined);
  private initializationError$: BehaviorSubject<string> = new BehaviorSubject<string>(undefined);
  private cuisines$: BehaviorSubject<CuisineModel[]> = new BehaviorSubject<CuisineModel[]>(new Array<CuisineModel>());

  private isSearching$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  private selectedSearchPhrase$: BehaviorSubject<string> = new BehaviorSubject<string>('');
  private selectedOrderType$: BehaviorSubject<OrderType> = new BehaviorSubject<OrderType>(OrderType.Pickup);
  private selectedOrderTime$: BehaviorSubject<Date> = new BehaviorSubject<Date>(undefined);
  private selectedCuisine$: BehaviorSubject<string> = new BehaviorSubject<string>(undefined);
  private restaurants$: BehaviorSubject<RestaurantModel[]> = new BehaviorSubject<RestaurantModel[]>(new Array<RestaurantModel>());
  private openedRestaurants$: BehaviorSubject<RestaurantModel[]> = new BehaviorSubject<RestaurantModel[]>(new Array<RestaurantModel>());
  private closedRestaurants$: BehaviorSubject<RestaurantModel[]> = new BehaviorSubject<RestaurantModel[]>(new Array<RestaurantModel>());

  private storedCart$: BehaviorSubject<StoredCartModel> = new BehaviorSubject<StoredCartModel>(undefined);
  private selectedRestaurant$: BehaviorSubject<RestaurantModel> = new BehaviorSubject<RestaurantModel>(undefined);
  private dishCategoriesOfSelectedRestaurant$: BehaviorSubject<DishCategoryModel[]> = new BehaviorSubject<DishCategoryModel[]>(undefined);

  private cart$: BehaviorSubject<CartModel> = new BehaviorSubject<CartModel>(undefined);

  private isCartVisible$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  private isCheckingOut$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  private isCheckedOut$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  private checkoutError$: BehaviorSubject<string> = new BehaviorSubject<string>(undefined);
  private order$: BehaviorSubject<OrderModel> = new BehaviorSubject<OrderModel>(undefined);

  constructor(
    private orderService: OrderService,
    private storedCartService: StoredCartService,
    private httpErrorHandlingService: HttpErrorHandlingService
  ) {
    combineLatest([
      this.selectedSearchPhrase$.pipe(debounceTime(500), distinctUntilChanged()),
      this.selectedOrderType$.pipe(distinctUntilChanged()),
      this.selectedOrderTime$.pipe(distinctUntilChanged()),
      this.selectedCuisine$.pipe(distinctUntilChanged())
    ])
      .subscribe(() => this.updateRestaurantSearchResult());
  }

  public getIsInitializing$(): Observable<boolean> {
    return this.isInitializing$;
  }

  public getIsInitialized$(): Observable<boolean> {
    return this.isInitialized$;
  }

  public getInitializationError$(): Observable<string> {
    return this.initializationError$;
  }

  public initialize(): void {
    if (this.isInitialized$.value !== undefined) {
      return;
    }

    const observables = new Array<Observable<void>>();

    observables.push(this.orderService.getAllCuisinesAsync()
      .pipe(
        take(1),
        map(cuisines => {
          this.cuisines$.next(cuisines);
        })
      )
    );

    const storedCart = this.storedCartService.loadFromStorage();

    if (storedCart !== undefined) {
      this.storedCart$.next(storedCart);

      observables.push(this.orderService.getRestaurantAsync(storedCart.restaurantId)
        .pipe(
          take(1),
          map(restaurant => {
            this.selectedRestaurant$.next(new RestaurantModel(restaurant));
          }),
          catchError((error: HttpErrorResponse) => {
            return of(void 0);
          })
        )
      );

      observables.push(this.orderService.getDishesOfRestaurantAsync(storedCart.restaurantId)
        .pipe(
          take(1),
          map(dishCategories => {
            this.dishCategoriesOfSelectedRestaurant$.next(dishCategories);
          })
        )
      );
    }

    this.isInitialized$.next(undefined);
    this.isInitializing$.next(true);
    combineLatest(observables)
      .subscribe(
        () => {
          this.updateCartModel();
          this.isInitializing$.next(false);
          this.isInitialized$.next(true);
        },
        (error: HttpErrorResponse | Error) => {
          this.isInitializing$.next(false);
          this.isInitialized$.next(false);
          if (error instanceof HttpErrorResponse) {
            const errorText = this.httpErrorHandlingService.handleError(error).getJoinedGeneralErrors();
            this.initializationError$.next(errorText);
          } else if (error instanceof Error) {
            this.initializationError$.next(error.message);
          }
        }
      );
  }

  public getCuisines$(): Observable<CuisineModel[]> {
    return this.cuisines$;
  }

  public getCuisines(): CuisineModel[] {
    return this.cuisines$.value;
  }


  public getIsSearching$(): Observable<boolean> {
    return this.isSearching$;
  }

  public getSelectedSearchPhrase$(): Observable<string> {
    return this.selectedSearchPhrase$;
  }

  public getSelectedSearchPhrase(): string {
    return this.selectedSearchPhrase$.value;
  }

  public setSelectedSearchPhrase(selectedSearchPhrase: string): void {
    this.selectedSearchPhrase$.next(selectedSearchPhrase);
  }

  public getSelectedOrderType$(): Observable<OrderType> {
    return this.selectedOrderType$;
  }

  public getSelectedOrderType(): OrderType {
    return this.selectedOrderType$.value;
  }

  public setSelectedOrderType(selectedOrderType: OrderType): void {
    this.selectedOrderType$.next(selectedOrderType);
  }

  public setSelectedOrderTypeIfNotSet(selectedOrderType: OrderType): void {
    if (!this.selectedOrderType$.value) {
      this.selectedOrderType$.next(selectedOrderType);
    }
  }

  public getSelectedOrderTime$(): Observable<Date> {
    return this.selectedOrderTime$;
  }

  public getSelectedOrderTime(): Date {
    return this.selectedOrderTime$.value;
  }

  public setSelectedOrderTime(selectedOpeningHourFilter: Date): void {
    this.selectedOrderTime$.next(selectedOpeningHourFilter);
  }

  public getSelectedCuisine$(): Observable<string> {
    return this.selectedCuisine$;
  }

  public getSelectedCuisine(): string {
    return this.selectedCuisine$.value;
  }

  public setSelectedCuisine(selectedCuisineFilter: string): void {
    this.selectedCuisine$.next(selectedCuisineFilter);
  }

  public resetFilters(): void {
    this.selectedSearchPhrase$.next('');
    this.selectedOrderType$.next(undefined);
    this.selectedOrderTime$.next(undefined);
    this.selectedCuisine$.next(undefined);
  }

  public getRestaurants$(): Observable<RestaurantModel[]> {
    return this.restaurants$;
  }

  public getRestaurants(): RestaurantModel[] {
    return this.restaurants$.value;
  }

  public getOpenedRestaurants$(): Observable<RestaurantModel[]> {
    return this.openedRestaurants$;
  }

  public getClosedRestaurants$(): Observable<RestaurantModel[]> {
    return this.closedRestaurants$;
  }


  public getSelectedRestaurant$(): Observable<RestaurantModel> {
    return this.selectedRestaurant$;
  }

  public getSelectedRestaurant(): RestaurantModel {
    return this.selectedRestaurant$.value;
  }

  public getDishCategoriesOfSelectedRestaurant$(): Observable<DishCategoryModel[]> {
    return this.dishCategoriesOfSelectedRestaurant$;
  }

  public getDishCategoriesOfSelectedRestaurant(): DishCategoryModel[] {
    return this.dishCategoriesOfSelectedRestaurant$.value;
  }

  public selectRestaurantId$(restaurantId: string): Observable<void> {
    return this.orderService.getRestaurantAsync(restaurantId)
      .pipe(
        take(1),
        switchMap(restaurant => {
          return this.selectRestaurant$(restaurant);
        })
      );
  }

  public selectRestaurant$(restaurant: RestaurantModel): Observable<void> {
    if (this.selectedRestaurant$.value === undefined || this.selectedRestaurant$.value.id !== restaurant.id) {
      this.storedCart$.next(undefined);
      this.storedCartService.removeFromStorage();
    }

    this.selectedRestaurant$.next(new RestaurantModel(restaurant));

    this.dishCategoriesOfSelectedRestaurant$.next(undefined);
    this.isCartVisible$.next(false);
    this.updateCartModel();

    const observables = new Array<Observable<void>>();

    observables.push(this.orderService.getDishesOfRestaurantAsync(restaurant.id)
      .pipe(
        take(1),
        map(dishCategories => {
          this.dishCategoriesOfSelectedRestaurant$.next(dishCategories);
        })
      )
    );

    this.isInitialized$.next(undefined);
    this.isInitializing$.next(true);
    return combineLatest(observables)
      .pipe(
        map(() => {
          this.updateCartModel();
          this.isInitializing$.next(false);
          this.isInitialized$.next(true);
        }),
        catchError((response: HttpErrorResponse) => {
          this.isInitializing$.next(false);
          this.isInitialized$.next(false);
          this.initializationError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public startOrder(orderType: OrderType, serviceTime: Date): void {
    const selectedRestaurant = this.selectedRestaurant$.value;

    switch (orderType) {
      case OrderType.Pickup:
        if (!selectedRestaurant.pickupInfo || !selectedRestaurant.pickupInfo.enabled) {
          throw new Error('Entschuldigung, die Bestellart Abholung wird von dem Restaurant nicht unterstützt');
        }
        break;
      case OrderType.Delivery:
        if (!selectedRestaurant.deliveryInfo || !selectedRestaurant.deliveryInfo.enabled) {
          throw new Error('Entschuldigung, die Bestellart Lieferung wird von dem Restaurant nicht unterstützt');
        }
        break;
      case OrderType.Reservation:
        if (!selectedRestaurant.reservationInfo || !selectedRestaurant.reservationInfo.enabled) {
          throw new Error('Entschuldigung, die Bestellart Reservierung wird von dem Restaurant nicht unterstützt');
        }
        break;
    }

    const storedCart = new StoredCartModel();

    storedCart.orderType = OrderTypeConverter.convertToString(orderType);
    storedCart.restaurantId = selectedRestaurant.alias;
    storedCart.cartDishes = new Array<StoredCartDishModel>();
    storedCart.serviceTime = serviceTime?.toISOString();

    this.storedCart$.next(storedCart);
    this.storedCartService.saveToStorage(storedCart);
    this.updateCartModel();
  }

  public changeOrderType(orderType: OrderType) {
    const storedCart = this.storedCart$.value;

    if (!storedCart) {
      return;
    }
    storedCart.orderType = OrderTypeConverter.convertToString(orderType);

    this.storedCart$.next(storedCart);
    this.storedCartService.saveToStorage(storedCart);
    this.updateCartModel();
  }

  public getCart$(): Observable<CartModel> {
    return this.cart$;
  }

  public getCart(): CartModel {
    return this.cart$.value;
  }

  public addDishToCart(dish: DishModel, variant: DishVariantModel, count: number, remarks: string): void {
    const storedCart = this.storedCart$.value;

    if (!storedCart) {
      throw new Error('no cart defined');
    }

    if (count <= 0) {
      return;
    }

    let storedCartDish = storedCart.cartDishes.find(en => en.dishId === dish.id && en.variantId === variant.variantId);

    if (storedCartDish !== undefined) {
      storedCartDish.count += count;
      storedCartDish.remarks = remarks;
    } else {
      storedCartDish = new StoredCartDishModel();
      storedCartDish.itemId = Guid.create().toString();
      storedCartDish.dishId = dish.id;
      storedCartDish.variantId = variant.variantId;
      storedCartDish.count = count;
      storedCartDish.remarks = remarks;
      storedCart.cartDishes.push(storedCartDish);
    }

    this.isCartVisible$.next(true);

    this.storedCart$.next(storedCart);
    this.storedCartService.saveToStorage(storedCart);
    this.updateCartModel();
  }

  public setCountOfCartDish(itemId: string, count: number): void {
    const storedCart = this.storedCart$.value;

    if (!storedCart) {
      throw new Error('no cart defined');
    }

    const index = storedCart.cartDishes.findIndex(en => en.itemId === itemId);
    if (index < 0) {
      return;
    }

    storedCart.cartDishes[index].count = count;
    if (storedCart.cartDishes[index].count <= 0) {
      storedCart.cartDishes.splice(index, 1);
    }

    this.storedCart$.next(storedCart);
    this.storedCartService.saveToStorage(storedCart);
    this.updateCartModel();
  }

  public incrementCountOfCartDish(itemId: string): void {
    const storedCart = this.storedCart$.value;

    if (!storedCart) {
      throw new Error('no cart defined');
    }

    const index = storedCart.cartDishes.findIndex(en => en.itemId === itemId);
    if (index < 0) {
      return;
    }

    storedCart.cartDishes[index].count += 1;

    this.storedCart$.next(storedCart);
    this.storedCartService.saveToStorage(storedCart);
    this.updateCartModel();
  }

  public decrementCountOfCartDish(itemId: string): void {
    const storedCart = this.storedCart$.value;

    if (!storedCart) {
      throw new Error('no cart defined');
    }

    const index = storedCart.cartDishes.findIndex(en => en.itemId === itemId);
    if (index < 0) {
      return;
    }

    storedCart.cartDishes[index].count -= 1;

    if (storedCart.cartDishes[index].count <= 0) {
      storedCart.cartDishes.splice(index, 1);
    }

    this.storedCart$.next(storedCart);
    this.storedCartService.saveToStorage(storedCart);
    this.updateCartModel();
  }

  public changeRemarksOfCartDish(itemId: string, remarks: string): void {
    const storedCart = this.storedCart$.value;

    if (!storedCart) {
      throw new Error('no cart defined');
    }
    const index = storedCart.cartDishes.findIndex(en => en.itemId === itemId);
    if (index < 0) {
      return;
    }

    storedCart.cartDishes[index].remarks = remarks;

    this.storedCart$.next(storedCart);
    this.storedCartService.saveToStorage(storedCart);
    this.updateCartModel();
  }

  public removeCartDishFromCart(itemId: string): void {
    const storedCart = this.storedCart$.value;

    if (!storedCart) {
      throw new Error('no cart defined');
    }
    const index = storedCart.cartDishes.findIndex(en => en.itemId === itemId);
    if (index < 0) {
      return;
    }

    storedCart.cartDishes.splice(index, 1);

    this.storedCart$.next(storedCart);
    this.storedCartService.saveToStorage(storedCart);
    this.updateCartModel();
  }

  public discardCart(): void {
    this.storedCart$.next(undefined);
    this.storedCartService.removeFromStorage();
    this.updateCartModel();
  }

  public showCart(): void {
    this.isCartVisible$.next(true);
    this.updateCartModel();
  }

  public hideCart(): void {
    this.isCartVisible$.next(false);
    this.updateCartModel();
  }

  public getOrder$(): Observable<OrderModel> {
    return this.order$;
  }

  public getOrder(): OrderModel {
    return this.order$.value;
  }

  public getIsCheckingOut$(): Observable<boolean> {
    return this.isCheckingOut$;
  }

  public getIsCheckedOut$(): Observable<boolean> {
    return this.isCheckedOut$;
  }

  public getCheckoutError$(): Observable<string> {
    return this.checkoutError$;
  }

  public checkout(checkoutModel: CheckoutModel): void {
    this.isCheckedOut$.next(undefined);
    this.isCheckingOut$.next(true);
    this.orderService.sendCheckoutAsync(checkoutModel)
      .pipe(take(1))
      .subscribe(order => {
          this.isCheckingOut$.next(false);
          this.isCheckedOut$.next(true);
          this.order$.next(order);
          this.discardCart();
        },
        (response: HttpErrorResponse) => {
          this.isCheckingOut$.next(false);
          this.isCheckedOut$.next(false);
          this.checkoutError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        }
      );
  }

  public resetCheckout(): void {
    this.isCheckedOut$.next(false);
    this.order$.next(undefined);
  }

  public getStartDateOfReservation(): Date {
    return new Date(2021, 2, 23);
  }

  private updateRestaurantSearchResult() {
    this.isSearching$.next(true);
    this.orderService.searchForRestaurantsAsync(this.selectedSearchPhrase$.value, this.selectedOrderType$.value, this.selectedCuisine$.value, undefined)
      .pipe(take(1))
      .subscribe(result => {
          this.isSearching$.next(false);

          if (result === undefined) {
            this.restaurants$.next(new Array<RestaurantModel>());
            this.openedRestaurants$.next(new Array<RestaurantModel>());
            this.closedRestaurants$.next(new Array<RestaurantModel>());
            return;
          }

          const restaurants = new Array<RestaurantModel>(result.length);
          for (let i = 0; i < result.length; i++) {
            restaurants[i] = new RestaurantModel(result[i]);
          }
          restaurants.sort(OrderFacade.restaurantSortFunc);
          this.restaurants$.next(restaurants);

          const selectedOrderTime = this.selectedOrderTime$.value;

          const openedRestaurants = new Array<RestaurantModel>();
          const closedRestaurants = new Array<RestaurantModel>();
          for (let restaurant of restaurants) {
            if (restaurant.isOpen(selectedOrderTime)) {
              openedRestaurants.push(restaurant);
            } else {
              closedRestaurants.push(restaurant);
            }
          }
          openedRestaurants.sort(OrderFacade.restaurantSortFunc);
          this.openedRestaurants$.next(openedRestaurants);
          closedRestaurants.sort(OrderFacade.restaurantSortFunc);
          this.closedRestaurants$.next(closedRestaurants);
        },
        () => {
          this.isSearching$.next(false);
          this.restaurants$.next(new Array<RestaurantModel>());
          this.openedRestaurants$.next(new Array<RestaurantModel>());
          this.closedRestaurants$.next(new Array<RestaurantModel>());
        });
  }

  private updateCartModel() {
    const cart = OrderFacade.generateCartModel(
      this.storedCart$.value,
      this.selectedRestaurant$.value,
      this.dishCategoriesOfSelectedRestaurant$.value,
      this.isCartVisible$.value
    );
    this.cart$.next(cart);
  }

  private static generateCartModel(
    storedCart: StoredCartModel,
    selectedRestaurant: RestaurantModel,
    dishCategoriesOfSelectedRestaurant: DishCategoryModel[],
    isCartVisible: boolean
  ): CartModel {
    if (!storedCart || !selectedRestaurant || !dishCategoriesOfSelectedRestaurant) {
      return undefined;
    }

    let averageTime: number;
    let minimumOrderValue: number;
    let maximumOrderValue: number;
    let costs: number;

    switch (storedCart.orderType) {
      case 'pickup':
        averageTime = selectedRestaurant.pickupInfo.averageTime;
        minimumOrderValue = selectedRestaurant.pickupInfo.minimumOrderValue;
        maximumOrderValue = selectedRestaurant.pickupInfo.maximumOrderValue;
        costs = undefined;
        break;
      case 'delivery':
        averageTime = selectedRestaurant.deliveryInfo.averageTime;
        minimumOrderValue = selectedRestaurant.deliveryInfo.minimumOrderValue;
        maximumOrderValue = selectedRestaurant.deliveryInfo.maximumOrderValue;
        costs = selectedRestaurant.deliveryInfo.costs;
        break;
      case 'reservation':
        averageTime = undefined;
        minimumOrderValue = undefined;
        maximumOrderValue = undefined;
        costs = undefined;
        break;
    }

    const dishes = new Map<string, DishModel>();
    for (const dishCategory of dishCategoriesOfSelectedRestaurant) {
      if (!dishCategory.dishes) {
        continue;
      }
      for (const dish of dishCategory.dishes) {
        dishes.set(dish.id, dish);
      }
    }

    const cartDishes = new Array<CartDishModel>();
    for (const storedCartDish of storedCart.cartDishes) {
      const dish = dishes.get(storedCartDish.dishId);
      if (!dish) {
        throw new Error('dish with id ' + storedCartDish.dishId + ' not known');
      }

      const variant = dish.variants.find(en => en.variantId === storedCartDish.variantId);
      if (!variant) {
        throw new Error('variant with id ' + storedCartDish.variantId + ' not known');
      }

      const CartDish = new CartDishModel(
        storedCartDish.itemId,
        dish,
        variant,
        storedCartDish.count,
        storedCartDish.remarks
      );

      cartDishes.push(CartDish);
    }

    let serviceTime: Date = undefined;
    try {
      const dt = storedCart.serviceTime.split(/[: T-]/).map(parseFloat);
      serviceTime = new Date(Date.UTC(dt[0], dt[1] - 1, dt[2], dt[3] || 0, dt[4] || 0, dt[5] || 0, 0));
    } catch {
    }

    return new CartModel(
      OrderTypeConverter.convertFromString(storedCart.orderType),
      selectedRestaurant.id,
      averageTime,
      minimumOrderValue,
      maximumOrderValue,
      costs,
      selectedRestaurant.hygienicHandling,
      cartDishes,
      isCartVisible,
      serviceTime
    );
  }

  private static restaurantSortFunc(a: RestaurantModel, b: RestaurantModel): number {
    if (a.name < b.name) {
      return -1;
    } else if (a.name > b.name) {
      return +1;
    } else {
      return 0;
    }
  }

}
