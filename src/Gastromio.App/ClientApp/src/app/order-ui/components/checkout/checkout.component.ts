import {Component, OnInit} from '@angular/core';
import {Location} from '@angular/common';
import {Router} from '@angular/router';

import {NgbModal} from '@ng-bootstrap/ng-bootstrap';

import {BlockUI, NgBlockUI} from 'ng-block-ui';

import {RestaurantModel} from '../../../shared/models/restaurant.model';
import {DishCategoryModel} from '../../../shared/models/dish-category.model';
import {PaymentMethodModel} from '../../../shared/models/payment-method.model';

import {HttpErrorHandlingService} from '../../../shared/services/http-error-handling.service';

import {CartDishModel} from '../../../order/models/cart-dish.model';
import {CartModel} from '../../../order/models/cart.model';
import {CheckoutModel} from '../../../order/models/checkout.model';
import {OrderTypeConverter} from "../../../order/models/order-type-converter";
import {StoredCartDishModel} from '../../../order/models/stored-cart-dish.model';

import {OrderFacade} from "../../../order/order.facade";

import {OpeningHourFilterComponent} from '../opening-hour-filter/opening-hour-filter.component';
import {EditCartDishComponent} from '../edit-cart-dish/edit-cart-dish.component';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: [
    './checkout.component.css',
    '../../../../assets/css/frontend_v3.min.css'
  ]
})
export class CheckoutComponent implements OnInit {
  @BlockUI() blockUI: NgBlockUI;

  generalError: string;

  initialized: boolean;
  submitted = false;

  restaurant: RestaurantModel;
  dishCategories: DishCategoryModel[];

  givenName: string;
  lastName: string;
  street: string;
  zipCode: number;
  city: string;
  phone: string;
  email: string;
  comments: string;
  paymentMethodId: string;
  serviceTime: Date;

  isPhoneNumberVisible: boolean = false;

  constructor(
    private orderFacade: OrderFacade,
    private modalService: NgbModal,
    private httpErrorHandlingService: HttpErrorHandlingService,
    private router: Router,
    private location: Location
  ) {
  }

  ngOnInit() {
    this.initialized = false;

    this.orderFacade.getIsInitializing$()
      .subscribe(isInitializing => {
        if (isInitializing) {
          this.blockUI.start();
        } else {
          this.blockUI.stop();
        }
      })

    this.orderFacade.getIsInitialized$()
      .subscribe(isInitialized => {
        if (!isInitialized)
          return;

        const cart = this.orderFacade.getCart();
        if (!cart) {
          this.generalError = 'Sie haben keine Gerichte im Warenkorb';
          return;
        } else {
          this.generalError = undefined;
        }

        this.initialized = isInitialized;

        this.restaurant = this.orderFacade.getSelectedRestaurant();
        this.dishCategories = this.orderFacade.getDishCategoriesOfSelectedRestaurant();
        this.serviceTime = this.orderFacade.getCart().getServiceTime();

        if (this.restaurant.supportedOrderMode === 'anytime' && this.restaurant.isOpen(undefined) && !this.serviceTime) {
          this.serviceTime = CheckoutComponent.roundOnQuarterHours(new Date());
        }
      }, error => {
        this.blockUI.stop();
        this.generalError = this.httpErrorHandlingService.handleError(error).getJoinedGeneralErrors();
      });

    this.orderFacade.getInitializationError$()
      .subscribe(initializationError => {
        this.generalError = initializationError;
      })

    this.orderFacade.getIsCheckingOut$()
      .subscribe(isCheckingOut => {
        if (isCheckingOut) {
          this.blockUI.start('Bestellung wird verarbeitet...');
        } else {
          this.blockUI.stop();
        }
      })

    this.orderFacade.getIsCheckedOut$()
      .subscribe(isCheckedOut => {
        if (!isCheckedOut)
          return;
        this.router.navigateByUrl('/order-summary');
      })

    this.orderFacade.getCheckoutError$()
      .subscribe(checkoutError => {
        this.generalError = checkoutError;
      });

    this.orderFacade.initialize();
  }

  getBannerStyle(
    restaurant: RestaurantModel
  ): string {
    if (!restaurant) {
      return undefined;
    }
    return '/api/v1/restaurants/' + restaurant.id + '/images/banner';
  }

  getGivenNameError(): string {
    if (!this.givenName || this.givenName.trim().length === 0)
      return 'Bitte gib Deinen Vornamen an.';
    return undefined;
  }

  getLastNameError(): string {
    if (!this.lastName || this.lastName.trim().length === 0)
      return 'Bitte gib Deinen Nachnamen an.';
    return undefined;
  }

  getStreetError(): string {
    if (!this.getCart().isDelivery())
      return undefined;
    if (!this.street || this.street.trim().length === 0)
      return 'Bitte gib eine Straße an.';
    return undefined;
  }

  getZipCodeError(): string {
    if (!this.getCart().isDelivery())
      return undefined;
    if (!this.zipCode || this.zipCode < 10000 || this.zipCode > 99999)
      return 'Bitte gib eine Postleitzahl an.';
    return undefined;
  }

  getCityError(): string {
    if (!this.getCart().isDelivery())
      return undefined;
    if (!this.city || this.city.trim().length === 0)
      return 'Bitte gib eine Stadt an.';
    return undefined;
  }

  getPhoneError(): string {
    if (!this.phone || this.phone.trim().length === 0)
      return 'Bitte gib eine Telefonnummer an.';
    const regex = /^(((((((00|\+)49[ \-/]?)|0)[1-9][0-9]{1,4})[ \-/]?)|((((00|\+)49\()|\(0)[1-9][0-9]{1,4}\)[ \-/]?))[0-9]{1,7}([ \-/]?[0-9]{1,5})?)$/;
    if (!regex.test(this.phone))
      return 'Bitte gib eine gültige Telefonnummer an.';
    return undefined;
  }

  getEmailError(): string {
    if (!this.email || this.email.trim().length === 0)
      return 'Bitte gib eine E-Mail Adresse an.';
    const regex = RegExp('^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$');
    if (!regex.test(this.email))
      return 'Bitte gib eine gültige E-Mail Adresse an.';
    return undefined;
  }

  getPaymentError(): string {
    if (!this.paymentMethodId || this.paymentMethodId.trim().length === 0)
      return 'Bitte wähle eine Zahlungsmethode aus.';
    return undefined;
  }

  onSelectPaymentMethod(paymentMethod: PaymentMethodModel): void {
    this.paymentMethodId = paymentMethod.id;
  }

  onAsFastAsPossible(): void {
    this.serviceTime = undefined;
  }

  onSelectServiceTime(): void {
    const modalRef = this.modalService.open(OpeningHourFilterComponent, {centered: true});
    modalRef.componentInstance.value = this.serviceTime;
    modalRef.result.then((value: Date) => {
      this.serviceTime = value;
    }, () => {
    });
  }

  getServiceTimeText(): string {
    if (!this.serviceTime)
      return undefined;

    let hoursText = this.serviceTime.getHours().toString();
    hoursText = ('0' + hoursText).slice(-2);

    let minutesText = this.serviceTime.getMinutes().toString();
    minutesText = ('0' + minutesText).slice(-2);

    return this.serviceTime.toLocaleDateString() + ', ' + hoursText + ':' + minutesText;
  }

  getServiceTimeError(): string {
    if (!this.restaurant.isOrderPossibleAt(this.serviceTime))
      return 'Eine elektronische Bestellung zum gewählten Zeitpunkt ist nicht möglich.';

    return undefined;
  }

  getCartError(): string {
    if (this.getCart().getCartDishes().length === 0)
      return 'Der Warenkorb ist leer. Bitte wähle erst ein oder mehrere Gerichte aus.';
    else if (!this.getCart().isValid())
      return this.getCart().getValidationError();
    return undefined;
  }

  getCart(): CartModel {
    return this.orderFacade.getCart();
  }

  onBack(): void {
    this.location.back();
  }

  public onEditCartDish(cartDish: CartDishModel): void {
    const modalRef = this.modalService.open(EditCartDishComponent);
    modalRef.componentInstance.cartDish = cartDish;
    modalRef.result.then(() => {
    }, () => {
    });
  }

  public onRemoveDishVariantFromCart(cartDishVariant: CartDishModel): void {
    if (cartDishVariant === undefined) {
      return;
    }
    this.orderFacade.removeCartDishFromCart(cartDishVariant.getItemId());
  }

  public showPhone(): void {
    this.isPhoneNumberVisible = true;
  }

  isValid(): boolean {
    return !this.getGivenNameError() &&
      !this.getLastNameError() &&
      !this.getStreetError() &&
      !this.getZipCodeError() &&
      !this.getCityError() &&
      !this.getEmailError() &&
      !this.getPhoneError() &&
      !this.getPaymentError() &&
      !this.getServiceTimeError() &&
      !this.getCartError()
  }

  onCheckout(): void {
    this.submitted = true;

    if (!this.isValid()) {
      return;
    }

    const cart = this.getCart();

    const checkoutModel = new CheckoutModel();
    checkoutModel.givenName = this.givenName;
    checkoutModel.lastName = this.lastName;
    checkoutModel.street = this.street;
    checkoutModel.zipCode = this.zipCode?.toString();
    checkoutModel.city = this.city;
    checkoutModel.phone = this.phone;
    checkoutModel.email = this.email;
    checkoutModel.addAddressInfo = this.comments;
    checkoutModel.orderType = OrderTypeConverter.convertToString(cart.getOrderType());
    checkoutModel.restaurantId = cart.getRestaurantId();
    checkoutModel.cartDishes = new Array<StoredCartDishModel>();

    for (const cartDish of cart.getCartDishes()) {
      const storedCartDish = new StoredCartDishModel();
      storedCartDish.itemId = cartDish.getItemId();
      storedCartDish.dishId = cartDish.getDish().id;
      storedCartDish.variantId = cartDish.getVariant().variantId;
      storedCartDish.count = cartDish.getCount();
      storedCartDish.remarks = cartDish.getRemarks();
      checkoutModel.cartDishes.push(storedCartDish);
    }

    checkoutModel.comments = this.comments;
    checkoutModel.paymentMethodId = this.paymentMethodId;
    checkoutModel.serviceTime = this.serviceTime?.toISOString();

    console.log('Checkout Model: ', checkoutModel);

    this.orderFacade.checkout(checkoutModel);
  }

  private static roundOnQuarterHours(date: Date): Date {
    let minutesToAdd = Math.ceil(date.getMinutes() / 15) * 15 - date.getMinutes();
    return new Date(date.getTime() + minutesToAdd * 60000);
  }

}
