import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';

import {Observable} from 'rxjs';

import {CuisineModel} from '../../shared/models/cuisine.model';
import {DishCategoryModel} from '../../shared/models/dish-category.model';
import {DishModel} from '../../shared/models/dish.model';
import {RestaurantModel,AddressModel,ContactInfoModel,ServiceInfoModel} from '../../shared/models/restaurant.model';
import {PaymentMethodModel} from '../../shared/models/payment-method.model';
import {UserModel} from '../../shared/models/user.model';

import {AuthService} from '../../auth/services/auth.service';

@Injectable()
export class RestaurantRestAdminService {
  private baseUrl = 'api/v1/restaurantadmin';

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {
  }

  public getMyRestaurantsAsync(): Observable<RestaurantModel[]> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.get<RestaurantModel[]>(this.baseUrl + '/myrestaurants', httpOptions);
  }

  public getRestaurantAsync(id: string): Observable<RestaurantModel> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.get<RestaurantModel>(this.baseUrl + '/restaurants/' + encodeURIComponent(id), httpOptions);
  }

  public getDishesOfRestaurantAsync(id: string): Observable<DishCategoryModel[]> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.get<DishCategoryModel[]>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/dishes', httpOptions);
  }

  public getCuisinesAsync(): Observable<CuisineModel[]> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.get<CuisineModel[]>(this.baseUrl + '/cuisines', httpOptions);
  }

  public getPaymentMethodsAsync(): Observable<PaymentMethodModel[]> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.get<PaymentMethodModel[]>(this.baseUrl + '/paymentmethods', httpOptions);
  }

  public searchForUsersAsync(search: string): Observable<UserModel[]> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.get<UserModel[]>(this.baseUrl + '/users?search=' + encodeURIComponent(search), httpOptions);
  }

  public changeRestaurantImageAsync(id: string, type: string, image: string): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/changeimage',
      {type, image}, httpOptions);
  }

  public changeRestaurantAddressAsync(id: string, address: AddressModel): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/changeaddress', address, httpOptions);
  }

  public changeRestaurantContactInfoAsync(id: string, contactInfo: ContactInfoModel): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/changecontactinfo', {
      phone: contactInfo.phone,
      fax: contactInfo.fax,
      webSite: contactInfo.webSite,
      responsiblePerson: contactInfo.responsiblePerson,
      emailAddress: contactInfo.emailAddress
    }, httpOptions);
  }

  public addOpeningPeriodToRestaurantAsync(id: string, dayOfWeek: number, start: number, end: number): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/addopeningperiod', {
      dayOfWeek,
      start,
      end
    }, httpOptions);
  }

  public changeOpeningPeriodOfRestaurantAsync(id: string, dayOfWeek: number, oldStart: number, newStart: number, newEnd: number): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/changeopeningperiod', {
      dayOfWeek,
      oldStart,
      newStart,
      newEnd
    }, httpOptions);
  }

  public removeOpeningPeriodFromRestaurantAsync(id: string, dayOfWeek: number, start: number): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/removeopeningperiod', {
      dayOfWeek,
      start
    }, httpOptions);
  }

  public changeRestaurantServiceInfoAsync(id: string, serviceInfo: ServiceInfoModel): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/changeserviceinfo', {
      pickupEnabled: serviceInfo.pickupEnabled,
      pickupAverageTime: serviceInfo.pickupAverageTime,
      pickupMinimumOrderValue: serviceInfo.pickupMinimumOrderValue,
      pickupMaximumOrderValue: serviceInfo.pickupMaximumOrderValue,
      deliveryEnabled: serviceInfo.deliveryEnabled,
      deliveryAverageTime: serviceInfo.deliveryAverageTime,
      deliveryMinimumOrderValue: serviceInfo.deliveryMinimumOrderValue,
      deliveryMaximumOrderValue: serviceInfo.deliveryMaximumOrderValue,
      deliveryCosts: serviceInfo.deliveryCosts,
      reservationEnabled: serviceInfo.reservationEnabled,
      hygienicHandling: serviceInfo.hygienicHandling
    }, httpOptions);
  }

  public addCuisineToRestaurantAsync(id: string, cuisineId: string): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/addcuisine', {cuisineId}, httpOptions);
  }

  public removeCuisineFromRestaurantAsync(id: string, cuisineId: string): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/removecuisine', {cuisineId}, httpOptions);
  }

  public addPaymentMethodToRestaurantAsync(id: string, paymentMethodId: string): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/addpaymentmethod',
      {paymentMethodId}, httpOptions);
  }

  public removePaymentMethodFromRestaurantAsync(id: string, paymentMethodId: string): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/removepaymentmethod',
      {paymentMethodId}, httpOptions);
  }

  public addAdminToRestaurantAsync(id: string, userId: string): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/addadmin', {userId}, httpOptions);
  }

  public removeAdminFromRestaurantAsync(id: string, userId: string): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/removeadmin', {userId}, httpOptions);
  }

  public addDishCategoryToRestaurantAsync(id: string, name: string, afterCategoryId: string): Observable<string> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<string>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/adddishcategory',
      {name, afterCategoryId}, httpOptions);
  }

  public changeDishCategoryOfRestaurantAsync(id: string, dishCategoryId: string, name: string): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/changedishcategory', {
      dishCategoryId,
      name
    }, httpOptions);
  }

  public incOrderOfDishCategoryAsync(id: string, dishCategoryId: string): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/incorderofdishcategory',
      {dishCategoryId}, httpOptions);
  }

  public decOrderOfDishCategoryAsync(id: string, dishCategoryId: string): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/decorderofdishcategory',
      {dishCategoryId}, httpOptions);
  }

  public removeDishCategoryFromRestaurantAsync(id: string, dishCategoryId: string): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/removedishcategory',
      {dishCategoryId}, httpOptions);
  }

  public addOrChangeDishOfRestaurantAsync(id: string, dishCategoryId: string, dish: DishModel): Observable<string> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<string>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/addoreditdish', {
      dishCategoryId,
      dish
    }, httpOptions);
  }

  public incOrderOfDishAsync(id: string, dishId: string): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/incorderofdish',
      {dishId}, httpOptions);
  }

  public decOrderOfDishAsync(id: string, dishId: string): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/decorderofdish',
      {dishId}, httpOptions);
  }

  public removeDishFromRestaurantAsync(id: string, dishCategoryId: string, dishId: string): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/removedish', {
      dishCategoryId,
      dishId
    }, httpOptions);
  }
}
