import {Injectable} from "@angular/core";
import {HttpErrorResponse} from "@angular/common/http";

import {BehaviorSubject, combineLatest, Observable, throwError} from "rxjs";
import {catchError, map, tap} from "rxjs/operators";

import {CuisineModel} from "../shared/models/cuisine.model";
import {DishCategoryModel} from "../shared/models/dish-category.model";
import {DishModel} from "../shared/models/dish.model";
import {
  AddressModel,
  ContactInfoModel,
  DeliveryInfoModel, DeviatingOpeningDayModel, ExternalMenu, OpeningPeriodModel,
  PickupInfoModel, RegularOpeningDayModel,
  ReservationInfoModel,
  RestaurantModel,
  ServiceInfoModel
} from "../shared/models/restaurant.model";
import {PaymentMethodModel} from "../shared/models/payment-method.model";

import {HttpErrorHandlingService} from "../shared/services/http-error-handling.service";

import {RestaurantRestAdminService} from "./services/restaurant-rest-admin.service";
import {DateModel} from "../shared/models/date.model";
import {AuthService} from "../auth/services/auth.service";

@Injectable()
export class RestaurantAdminFacade {

  private restaurantId: string;

  private isInitializing$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(true);
  private isInitialized$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(undefined);
  private initializationError$: BehaviorSubject<string> = new BehaviorSubject<string>(undefined);

  private cuisines$: BehaviorSubject<CuisineModel[]> = new BehaviorSubject<CuisineModel[]>(undefined);
  private paymentMethods$: BehaviorSubject<PaymentMethodModel[]> = new BehaviorSubject<PaymentMethodModel[]>(undefined);
  private restaurant$: BehaviorSubject<RestaurantModel> = new BehaviorSubject<RestaurantModel>(undefined);
  private dishCategories$: BehaviorSubject<DishCategoryModel[]> = new BehaviorSubject<DishCategoryModel[]>(undefined);

  private selectedTab$: BehaviorSubject<string> = new BehaviorSubject<string>("general");

  private isUpdating$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  private isUpdated$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(undefined);
  private updateError$: BehaviorSubject<string> = new BehaviorSubject<string>(undefined);

  static earliestOpeningTime: number = 4 * 60;

  constructor(
    private authService: AuthService,
    private restaurantAdminService: RestaurantRestAdminService,
    private httpErrorHandlingService: HttpErrorHandlingService
  ) {
  }

  public initialize(restaurantId: string): void {
    this.restaurantId = restaurantId;

    const getCuisines$ = this.restaurantAdminService.getCuisinesAsync()
      .pipe(
        tap(cuisines => {
          this.cuisines$.next(cuisines);
        })
      );

    const getPaymentMethods$ = this.restaurantAdminService.getPaymentMethodsAsync()
      .pipe(
        tap(paymentMethods => {
          this.paymentMethods$.next(paymentMethods
            .sort((a, b) => {
              if (a.name < b.name) {
                return -1;
              }
              if (a.name > b.name) {
                return 1;
              }
              return 0;
            })
          );
        })
      );

    const getRestaurant$ = this.restaurantAdminService.getRestaurantAsync(restaurantId)
      .pipe(
        tap(restaurant => {
          restaurant.cuisines.sort((a, b) => {
            if (a.name < b.name) {
              return -1;
            }
            if (a.name > b.name) {
              return 1;
            }
            return 0;
          });

          restaurant.paymentMethods.sort((a, b) => {
            if (a.name < b.name) {
              return -1;
            }
            if (a.name > b.name) {
              return 1;
            }
            return 0;
          });

          this.restaurant$.next(restaurant);
        })
      );

    const getDishesOfRestaurant$ = this.restaurantAdminService.getDishesOfRestaurantAsync(restaurantId)
      .pipe(
        tap(dishCategories => {
          this.dishCategories$.next(dishCategories);
        })
      )

    const observables = [
      getCuisines$,
      getPaymentMethods$,
      getRestaurant$,
      getDishesOfRestaurant$
    ];

    combineLatest(observables)
      .subscribe(
        () => {
          this.isInitializing$.next(false);
          this.isInitialized$.next(true);
        },
        (error: HttpErrorResponse) => {
          this.isInitializing$.next(false);
          this.isInitialized$.next(false);
          this.initializationError$.next(this.httpErrorHandlingService.handleError(error).getJoinedGeneralErrors());
        }
      );
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

  public getCuisines$(): Observable<CuisineModel[]> {
    return this.cuisines$;
  }

  public getPaymentMethods$(): Observable<PaymentMethodModel[]> {
    return this.paymentMethods$;
  }

  public getRestaurant$(): Observable<RestaurantModel> {
    return this.restaurant$;
  }

  public getDishCategories$(): Observable<DishCategoryModel[]> {
    return this.dishCategories$;
  }

  public getSelectedTab$(): Observable<string> {
    return this.selectedTab$;
  }

  public getIsUpdating$(): Observable<boolean> {
    return this.isUpdating$;
  }

  public getIsUpdated$(): Observable<boolean> {
    return this.isUpdated$;
  }

  public getUpdateError$(): Observable<string> {
    return this.updateError$;
  }

  public getHasLogo$(): Observable<boolean> {
    return this.restaurant$.asObservable()
      .pipe(
        map(restaurant => {
          if (!restaurant || !restaurant.imageTypes) {
            return false;
          }
          return restaurant.imageTypes.some(en => en === 'logo');
        })
      );
  }

  public getLogoUrl$(): Observable<string> {
    return this.restaurant$.asObservable()
      .pipe(
        map(restaurant => {
          if (restaurant && restaurant.imageTypes && restaurant.imageTypes.some(en => en === 'logo')) {
            return '/api/v1/restaurants/' + restaurant.id + '/images/logo?random=' + Math.random();
          } else {
            return undefined;
          }
        })
      );
  }

  public getHasBanner$(): Observable<boolean> {
    return this.restaurant$.asObservable()
      .pipe(
        map(restaurant => {
          if (!restaurant || !restaurant.imageTypes) {
            return false;
          }
          return restaurant.imageTypes.some(en => en === 'banner');
        })
      );
  }

  public getBannerUrl$(): Observable<string> {
    return this.restaurant$.asObservable()
      .pipe(
        map(restaurant => {
          if (restaurant && restaurant.imageTypes && restaurant.imageTypes.some(en => en === 'banner')) {
            return '/api/v1/restaurants/' + restaurant.id + '/images/banner?random=' + Math.random();
          } else {
            return undefined;
          }
        })
      );
  }

  public getPaymentMethodStatus$(): Observable<{}> {
    return this.restaurant$.asObservable()
      .pipe(
        map(restaurant => {
          const result = {};
          for (let paymentMethod of this.paymentMethods$.value) {
            result[paymentMethod.id] = restaurant.paymentMethods.some(en => en.id === paymentMethod.id);
          }
          return result;
        })
      );
  }


  // actions

  public selectTab(tab: string): void {
    this.selectedTab$.next(tab);
  }

  public changeAddress(address: AddressModel): void {
    this.isUpdating$.next(true);
    this.restaurantAdminService.changeRestaurantAddressAsync(this.restaurant$.value.id, address)
      .subscribe(() => {
        this.isUpdating$.next(false);
        this.updateError$.next(undefined);

        this.restaurant$.value.address = address;

        this.setUpdateInfoToRestaurant(this.restaurant$.value);
        this.restaurant$.next(this.restaurant$.value)
        this.isUpdated$.next(true);
      }, response => {
        this.isUpdating$.next(false);
        this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
      });
  }

  public changeContactInfo(contactInfo: ContactInfoModel): void {
    this.isUpdating$.next(true);
    this.restaurantAdminService.changeRestaurantContactInfoAsync(this.restaurant$.value.id, contactInfo)
      .subscribe(() => {
        this.isUpdating$.next(false);
        this.updateError$.next(undefined);
        this.restaurant$.value.contactInfo = contactInfo;
        this.setUpdateInfoToRestaurant(this.restaurant$.value);
        this.restaurant$.next(this.restaurant$.value)
        this.isUpdated$.next(true);
      }, response => {
        this.isUpdating$.next(false);
        this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
      });
  }

  public changeLogo(logo: string): void {
    this.isUpdating$.next(true);
    this.restaurantAdminService.changeRestaurantImageAsync(this.restaurant$.value.id, 'logo', logo)
      .subscribe(() => {
        this.isUpdating$.next(false);
        this.updateError$.next(undefined);

        if (!this.restaurant$.value.imageTypes) {
          this.restaurant$.value.imageTypes = new Array<string>();
        }
        if (!this.restaurant$.value.imageTypes.some(en => en === 'logo')) {
          this.restaurant$.value.imageTypes.push('logo');
        }

        this.setUpdateInfoToRestaurant(this.restaurant$.value);
        this.restaurant$.next(this.restaurant$.value)
        this.isUpdated$.next(true);
      }, (response: HttpErrorResponse) => {
        this.isUpdating$.next(false);
        this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
      });
  }

  public removeLogo(): void {
    this.isUpdating$.next(true);
    this.restaurantAdminService.changeRestaurantImageAsync(this.restaurant$.value.id, 'logo', undefined)
      .subscribe(() => {
        this.isUpdating$.next(false);
        this.updateError$.next(undefined);

        this.restaurant$.value.imageTypes = this.restaurant$.value.imageTypes.filter(en => en !== 'logo');

        this.setUpdateInfoToRestaurant(this.restaurant$.value);
        this.restaurant$.next(this.restaurant$.value)
        this.isUpdated$.next(true);
      }, (response: HttpErrorResponse) => {
        this.isUpdating$.next(false);
        this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
      });
  }

  public changeBanner(banner: string): void {
    this.isUpdating$.next(true);
    this.restaurantAdminService.changeRestaurantImageAsync(this.restaurant$.value.id, 'banner', banner)
      .subscribe(() => {
        this.isUpdating$.next(false);
        this.updateError$.next(undefined);

        if (!this.restaurant$.value.imageTypes) {
          this.restaurant$.value.imageTypes = new Array<string>();
        }
        if (!this.restaurant$.value.imageTypes.some(en => en === 'banner')) {
          this.restaurant$.value.imageTypes.push('banner');
        }

        this.setUpdateInfoToRestaurant(this.restaurant$.value);
        this.restaurant$.next(this.restaurant$.value)
        this.isUpdated$.next(true);
      }, (response: HttpErrorResponse) => {
        this.isUpdating$.next(false);
        this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
      });
  }

  public removeBanner(): void {
    this.isUpdating$.next(true);
    this.restaurantAdminService.changeRestaurantImageAsync(this.restaurant$.value.id, 'banner', undefined)
      .subscribe(() => {
        this.isUpdating$.next(false);
        this.updateError$.next(undefined);

        this.restaurant$.value.imageTypes = this.restaurant$.value.imageTypes.filter(en => en !== 'banner');

        this.setUpdateInfoToRestaurant(this.restaurant$.value);
        this.restaurant$.next(this.restaurant$.value)
        this.isUpdated$.next(true);
      }, (response: HttpErrorResponse) => {
        this.isUpdating$.next(false);
        this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
      });
  }

  public changeSupportedOrderMode(supportedOrderMode: string): void {
    this.isUpdating$.next(true);
    this.restaurantAdminService.changeSupportedOrderMode(this.restaurant$.value.id, supportedOrderMode)
      .subscribe(() => {
        this.isUpdating$.next(false);
        this.updateError$.next(undefined);

        this.restaurant$.value.supportedOrderMode = supportedOrderMode;

        this.setUpdateInfoToRestaurant(this.restaurant$.value);
        this.restaurant$.next(this.restaurant$.value)
        this.isUpdated$.next(true);
      }, (response: HttpErrorResponse) => {
        this.isUpdating$.next(false);
        this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
      });
  }

  public changeServiceInfo(serviceInfo: ServiceInfoModel): void {
    this.isUpdating$.next(true);
    this.restaurantAdminService.changeRestaurantServiceInfoAsync(this.restaurant$.value.id, serviceInfo)
      .subscribe(() => {
        this.isUpdating$.next(false);
        this.updateError$.next(undefined);

        this.restaurant$.value.pickupInfo = new PickupInfoModel({
          enabled: serviceInfo.pickupEnabled,
          averageTime: serviceInfo.pickupAverageTime,
          minimumOrderValue: serviceInfo.pickupMinimumOrderValue,
          maximumOrderValue: serviceInfo.pickupMaximumOrderValue,
        });

        this.restaurant$.value.deliveryInfo = new DeliveryInfoModel({
          enabled: serviceInfo.deliveryEnabled,
          averageTime: serviceInfo.deliveryAverageTime,
          minimumOrderValue: serviceInfo.deliveryMinimumOrderValue,
          maximumOrderValue: serviceInfo.deliveryMaximumOrderValue,
          costs: serviceInfo.deliveryCosts
        });

        this.restaurant$.value.reservationInfo = new ReservationInfoModel({
          enabled: serviceInfo.reservationEnabled,
          reservationSystemUrl: serviceInfo.reservationSystemUrl
        });

        this.restaurant$.value.hygienicHandling = serviceInfo.hygienicHandling;

        this.setUpdateInfoToRestaurant(this.restaurant$.value);
        this.restaurant$.next(this.restaurant$.value)
        this.isUpdated$.next(true);
      }, (response: HttpErrorResponse) => {
        this.isUpdating$.next(false);
        this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
      });
  }

  public togglePaymentMethod(paymentMethodId: string): void {
    let observable: Observable<boolean>;

    const isCurrentlyEnabled = this.restaurant$.value.paymentMethods.some(en => en.id === paymentMethodId);
    if (isCurrentlyEnabled) {
      const cashPaymentMethodId = '8DBBC822-E4FF-47B6-8CA2-68F4F0C51AA3'.toLocaleLowerCase();
      if (paymentMethodId === cashPaymentMethodId) {
        this.updateError$.next("Barzahlung kann nicht deaktiviert werden.");
        return;
      }
      observable = this.restaurantAdminService.removePaymentMethodFromRestaurantAsync(this.restaurant$.value.id, paymentMethodId);
    } else {
      observable = this.restaurantAdminService.addPaymentMethodToRestaurantAsync(this.restaurant$.value.id, paymentMethodId);
    }

    this.isUpdating$.next(true);
    observable
      .subscribe(() => {
        this.isUpdating$.next(false);
        this.updateError$.next(undefined);

        if (isCurrentlyEnabled) {
          this.restaurant$.value.paymentMethods = this.restaurant$.value.paymentMethods.filter(en => en.id !== paymentMethodId);
        } else {
          const paymentMethod = this.paymentMethods$.value.find(en => en.id === paymentMethodId);
          this.restaurant$.value.paymentMethods.push(paymentMethod);
          this.restaurant$.value.paymentMethods.sort((a, b) => {
            if (a.name < b.name) {
              return -1;
            }
            if (a.name > b.name) {
              return 1;
            }
            return 0;
          });
        }

        this.setUpdateInfoToRestaurant(this.restaurant$.value);
        this.restaurant$.next(this.restaurant$.value)
        this.isUpdated$.next(true);
      }, (response: HttpErrorResponse) => {
        this.isUpdating$.next(false);
        this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
      });
  }

  public addRegularOpeningPeriod(dayOfWeek: number, start: number, end: number): Observable<boolean> {
    this.isUpdating$.next(true);
    return this.restaurantAdminService.addRegularOpeningPeriodToRestaurantAsync(this.restaurant$.value.id, dayOfWeek, start, end)
      .pipe(
        tap(() => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);

          const regularOpeningDays = this.restaurant$.value.regularOpeningDays;
          let regularOpeningDay = this.restaurant$.value.regularOpeningDays.find(en => en.dayOfWeek === dayOfWeek);
          if (regularOpeningDay === undefined) {
            regularOpeningDay = new RegularOpeningDayModel({
              dayOfWeek: dayOfWeek,
              openingPeriods: new Array<OpeningPeriodModel>()
            })
            regularOpeningDays.push(regularOpeningDay);
          }
          regularOpeningDay.openingPeriods.push(new OpeningPeriodModel({
            start: start,
            end: end
          }));

          this.setUpdateInfoToRestaurant(this.restaurant$.value);
          this.restaurant$.next(this.restaurant$.value)
          this.isUpdated$.next(true);
        }),
        catchError((response: HttpErrorResponse) => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public changeRegularOpeningPeriod(dayOfWeek: number, oldStart: number, newStart: number, newEnd: number): Observable<boolean> {
    this.isUpdating$.next(true);
    return this.restaurantAdminService.changeRegularOpeningPeriodOfRestaurantAsync(this.restaurant$.value.id, dayOfWeek, oldStart, newStart, newEnd)
      .pipe(
        tap(() => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);

          const regularOpeningDay = this.restaurant$.value.regularOpeningDays.find(en => en.dayOfWeek === dayOfWeek);
          const openingPeriod = regularOpeningDay.openingPeriods.find(en => en.start === oldStart);
          openingPeriod.start = newStart;
          openingPeriod.end = newEnd;

          this.setUpdateInfoToRestaurant(this.restaurant$.value);
          this.restaurant$.next(this.restaurant$.value)
          this.isUpdated$.next(true);
        }),
        catchError((response: HttpErrorResponse) => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public removeRegularOpeningPeriod(dayOfWeek: number, start: number): Observable<boolean> {
    this.isUpdating$.next(true);
    return this.restaurantAdminService.removeRegularOpeningPeriodFromRestaurantAsync(this.restaurant$.value.id, dayOfWeek, start)
      .pipe(
        tap(() => {
            this.isUpdating$.next(false);
            this.updateError$.next(undefined);

            const regularOpeningDay = this.restaurant$.value.regularOpeningDays.find(en => en.dayOfWeek === dayOfWeek);
            const index = regularOpeningDay.openingPeriods.findIndex(en => en.start === start);
            regularOpeningDay.openingPeriods.splice(index, 1);

            this.setUpdateInfoToRestaurant(this.restaurant$.value);
            this.restaurant$.next(this.restaurant$.value)
            this.isUpdated$.next(true);
          }
        ),
        catchError((response: HttpErrorResponse) => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public addDeviatingOpeningDay(date: DateModel, status: string): Observable<boolean> {
    this.isUpdating$.next(true);
    return this.restaurantAdminService.addDeviatingOpeningDayToRestaurantAsync(this.restaurant$.value.id, date, status)
      .pipe(
        tap(() => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);

          const deviatingOpeningDays = this.restaurant$.value.deviatingOpeningDays;
          let deviatingOpeningDay = deviatingOpeningDays.find(en => DateModel.isEqual(en.date, date));
          if (deviatingOpeningDay === undefined) {
            deviatingOpeningDay = new DeviatingOpeningDayModel({
              date: date,
              status: status,
              openingPeriods: new Array<OpeningPeriodModel>()
            })
            deviatingOpeningDays.push(deviatingOpeningDay);
          }

          this.setUpdateInfoToRestaurant(this.restaurant$.value);
          this.restaurant$.next(this.restaurant$.value)
          this.isUpdated$.next(true);
        }),
        catchError((response: HttpErrorResponse) => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public changeDeviatingOpeningDayStatus(date: DateModel, status: string): Observable<boolean> {
    this.isUpdating$.next(true);
    return this.restaurantAdminService.changeDeviatingOpeningDayStatusOfRestaurantAsync(this.restaurant$.value.id, date, status)
      .pipe(
        tap(() => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);

          const deviatingOpeningDays = this.restaurant$.value.deviatingOpeningDays;
          const deviatingOpeningDay = deviatingOpeningDays.find(en => DateModel.isEqual(en.date, date));
          if (deviatingOpeningDay) {
            deviatingOpeningDay.status = status;
          }

          this.setUpdateInfoToRestaurant(this.restaurant$.value);
          this.restaurant$.next(this.restaurant$.value)
          this.isUpdated$.next(true);
        }),
        catchError((response: HttpErrorResponse) => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public removeDeviatingOpeningDay(date: DateModel): Observable<boolean> {
    this.isUpdating$.next(true);
    return this.restaurantAdminService.removeDeviatingOpeningDayFromRestaurantAsync(this.restaurant$.value.id, date)
      .pipe(
        tap(() => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);

          const deviatingOpeningDays = this.restaurant$.value.deviatingOpeningDays;
          const index = deviatingOpeningDays.findIndex(en => DateModel.isEqual(en.date, date));
          if (index >= 0) {
            deviatingOpeningDays.splice(index, 1);
          }

          this.setUpdateInfoToRestaurant(this.restaurant$.value);
          this.restaurant$.next(this.restaurant$.value)
          this.isUpdated$.next(true);
        }),
        catchError((response: HttpErrorResponse) => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public addDeviatingOpeningPeriod(date: DateModel, start: number, end: number): Observable<boolean> {
    this.isUpdating$.next(true);
    return this.restaurantAdminService.addDeviatingOpeningPeriodToRestaurantAsync(this.restaurant$.value.id, date, start, end)
      .pipe(
        tap(() => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);

          const deviatingOpeningDays = this.restaurant$.value.deviatingOpeningDays;
          let deviatingOpeningDay = deviatingOpeningDays.find(en => DateModel.isEqual(en.date, date));
          if (deviatingOpeningDay === undefined) {
            deviatingOpeningDay = new DeviatingOpeningDayModel({
              date: date,
              status: "open",
              openingPeriods: new Array<OpeningPeriodModel>()
            })
            deviatingOpeningDays.push(deviatingOpeningDay);
          }
          deviatingOpeningDay.openingPeriods.push(new OpeningPeriodModel({
            start: start,
            end: end
          }));

          this.setUpdateInfoToRestaurant(this.restaurant$.value);
          this.restaurant$.next(this.restaurant$.value)
          this.isUpdated$.next(true);
        }),
        catchError((response: HttpErrorResponse) => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public changeDeviatingOpeningPeriod(date: DateModel, oldStart: number, newStart: number, newEnd: number): Observable<boolean> {
    this.isUpdating$.next(true);
    return this.restaurantAdminService.changeDeviatingOpeningPeriodOfRestaurantAsync(this.restaurant$.value.id, date, oldStart, newStart, newEnd)
      .pipe(
        tap(() => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);

          const deviatingOpeningDays = this.restaurant$.value.deviatingOpeningDays;
          const deviatingOpeningDay = deviatingOpeningDays.find(en => DateModel.isEqual(en.date, date));
          const openingPeriod = deviatingOpeningDay.openingPeriods.find(en => en.start === oldStart);
          openingPeriod.start = newStart;
          openingPeriod.end = newEnd;

          this.setUpdateInfoToRestaurant(this.restaurant$.value);
          this.restaurant$.next(this.restaurant$.value)
          this.isUpdated$.next(true);
        }),
        catchError((response: HttpErrorResponse) => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public removeDeviatingOpeningPeriod(date: DateModel, start: number): Observable<boolean> {
    this.isUpdating$.next(true);
    return this.restaurantAdminService.removeDeviatingOpeningPeriodFromRestaurantAsync(this.restaurant$.value.id, date, start)
      .pipe(
        tap(() => {
            this.isUpdating$.next(false);
            this.updateError$.next(undefined);

            const deviatingOpeningDays = this.restaurant$.value.deviatingOpeningDays;
            const deviatingOpeningDay = deviatingOpeningDays.find(en => DateModel.isEqual(en.date, date));
            const index = deviatingOpeningDay.openingPeriods.findIndex(en => en.start === start);
            deviatingOpeningDay.openingPeriods.splice(index, 1);
            if (deviatingOpeningDay.openingPeriods.length === 0) {
              deviatingOpeningDay.status = "closed";
            }

            this.setUpdateInfoToRestaurant(this.restaurant$.value);
            this.restaurant$.next(this.restaurant$.value)
            this.isUpdated$.next(true);
          }
        ),
        catchError((response: HttpErrorResponse) => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public addOrChangeExternalMenu(externalMenuId: string, name: string, description: string, url: string): void {
    this.isUpdating$.next(true);
    this.restaurantAdminService.addOrChangeExternalMenu(this.restaurant$.value.id, externalMenuId, name, description, url)
      .subscribe(() => {
        this.isUpdating$.next(false);
        this.updateError$.next(undefined);

        const index = this.restaurant$.value.externalMenus?.findIndex(en => en.id === externalMenuId) ?? -1;
        if (index < 0) {
          if (!this.restaurant$.value.externalMenus)
            this.restaurant$.value.externalMenus = new Array<ExternalMenu>();
          this.restaurant$.value.externalMenus.push(new ExternalMenu({
            id: externalMenuId,
            name: name,
            description: description,
            url: url
          }));
        } else {
          const externalMenu = this.restaurant$.value.externalMenus[index];
          externalMenu.name = name;
          externalMenu.description = description;
          externalMenu.url = url;
        }

        this.setUpdateInfoToRestaurant(this.restaurant$.value);
        this.restaurant$.next(this.restaurant$.value)
        this.isUpdated$.next(true);
      }, (response: HttpErrorResponse) => {
        this.isUpdating$.next(false);
        this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
      });
  }

  public removeExternalMenu(externalMenuId: string): void {
    const index = this.restaurant$.value.externalMenus?.findIndex(en => en.id === externalMenuId) ?? -1;
    if (index < 0)
      return;

    this.isUpdating$.next(true);
    this.restaurantAdminService.removeExternalMenu(this.restaurant$.value.id, externalMenuId)
      .subscribe(() => {
        this.isUpdating$.next(false);
        this.updateError$.next(undefined);

        const index = this.restaurant$.value.externalMenus?.findIndex(en => en.id === externalMenuId) ?? -1;
        if (index >= 0) {
          this.restaurant$.value.externalMenus.splice(index, 1);
        }

        this.setUpdateInfoToRestaurant(this.restaurant$.value);
        this.restaurant$.next(this.restaurant$.value)
        this.isUpdated$.next(true);
      }, (response: HttpErrorResponse) => {
        this.isUpdating$.next(false);
        this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
      });
  }

  public addDishCategory(name: string, afterCategoryId: string): Observable<string> {
    this.isUpdating$.next(true);
    return this.restaurantAdminService.addDishCategoryToRestaurantAsync(this.restaurant$.value.id, name, afterCategoryId)
      .pipe(
        tap(id => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);

          const dishCategories = this.dishCategories$.value;
          const index = dishCategories.findIndex(en => en.id === afterCategoryId);
          const dishCategory = new DishCategoryModel({
            id: id,
            name: name,
            dishes: new Array<DishModel>()
          });
          dishCategories.splice(index + 1, 0, dishCategory);

          this.dishCategories$.next(dishCategories);
          this.isUpdated$.next(true);
        }),
        catchError((response: HttpErrorResponse) => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public changeDishCategory(dishCategoryId: string, name: string): Observable<boolean> {
    this.isUpdating$.next(true);
    return this.restaurantAdminService.changeDishCategoryOfRestaurantAsync(this.restaurant$.value.id, dishCategoryId, name)
      .pipe(
        tap(() => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);

          const dishCategories = this.dishCategories$.value;
          const index = dishCategories.findIndex(en => en.id === dishCategoryId);
          dishCategories[index].name = name;

          this.dishCategories$.next(dishCategories);
          this.isUpdated$.next(true);
        }),
        catchError((response: HttpErrorResponse) => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public removeDishCategory(dishCategoryId: string): Observable<boolean> {
    this.isUpdating$.next(true);
    return this.restaurantAdminService.removeDishCategoryFromRestaurantAsync(this.restaurant$.value.id, dishCategoryId)
      .pipe(
        tap(() => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);

          const dishCategories = this.dishCategories$.value;
          const index = dishCategories.findIndex(en => en.id === dishCategoryId);
          dishCategories.splice(index, 1);

          this.dishCategories$.next(dishCategories);
          this.isUpdated$.next(true);
        }),
        catchError((response: HttpErrorResponse) => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public decOrderOfDishCategory(dishCategoryId: string): void {
    const dishCategories = this.dishCategories$.value;
    const pos = dishCategories.findIndex(en => en.id === dishCategoryId);
    if (pos < 1) {
      return;
    }

    this.isUpdating$.next(true);
    this.restaurantAdminService.decOrderOfDishCategoryAsync(this.restaurant$.value.id, dishCategoryId)
      .subscribe(() => {
        this.isUpdating$.next(false);
        this.updateError$.next(undefined);

        [dishCategories[pos - 1], dishCategories[pos]] = [dishCategories[pos], dishCategories[pos - 1]];

        this.dishCategories$.next(this.dishCategories$.value);
        this.isUpdated$.next(true);
      }, (response: HttpErrorResponse) => {
        this.isUpdating$.next(false);
        this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
      });
  }

  public incOrderOfDishCategory(dishCategoryId: string): void {
    const dishCategories = this.dishCategories$.value;
    const pos = dishCategories.findIndex(en => en.id === dishCategoryId);
    if (pos >= dishCategories.length - 1) {
      return;
    }

    this.isUpdating$.next(true);
    this.restaurantAdminService.incOrderOfDishCategoryAsync(this.restaurant$.value.id, dishCategoryId)
      .subscribe(() => {
        this.isUpdating$.next(false);
        this.updateError$.next(undefined);

        [dishCategories[pos], dishCategories[pos + 1]] = [dishCategories[pos + 1], dishCategories[pos]];

        this.dishCategories$.next(this.dishCategories$.value);
        this.isUpdated$.next(true);
      }, (response: HttpErrorResponse) => {
        this.isUpdating$.next(false);
        this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
      });
  }

  public enableDishCategory(dishCategoryId: string): void {
    this.isUpdating$.next(true);
    this.restaurantAdminService.enableDishCategoryAsync(this.restaurant$.value.id, dishCategoryId)
      .subscribe(() => {
        this.isUpdating$.next(false);
        this.updateError$.next(undefined);

        const dishCategories = this.dishCategories$.value;
        const dishCategoryIndex = dishCategories.findIndex(en => en.id === dishCategoryId);
        const dishCategory = dishCategories[dishCategoryIndex];
        dishCategory.enabled = true;

        this.dishCategories$.next(this.dishCategories$.value);
        this.isUpdated$.next(true);
      }, (response: HttpErrorResponse) => {
        this.isUpdating$.next(false);
        this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
      });
  }

  public disableDishCategory(dishCategoryId: string): void {
    this.isUpdating$.next(true);
    this.restaurantAdminService.disableDishCategoryAsync(this.restaurant$.value.id, dishCategoryId)
      .subscribe(() => {
        this.isUpdating$.next(false);
        this.updateError$.next(undefined);

        const dishCategories = this.dishCategories$.value;
        const dishCategoryIndex = dishCategories.findIndex(en => en.id === dishCategoryId);
        const dishCategory = dishCategories[dishCategoryIndex];
        dishCategory.enabled = false;

        this.dishCategories$.next(this.dishCategories$.value);
        this.isUpdated$.next(true);
      }, (response: HttpErrorResponse) => {
        this.isUpdating$.next(false);
        this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
      });
  }

  public addOrChangedDish(dishCategoryId: string, dish: DishModel): Observable<string> {
    this.isUpdating$.next(true);
    return this.restaurantAdminService.addOrChangeDishOfRestaurantAsync(this.restaurant$.value.id, dishCategoryId, dish)
      .pipe(
        tap(dishId => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);

          const dishCategories = this.dishCategories$.value;
          const dishCategoryIndex = dishCategories.findIndex(en => en.id === dishCategoryId);
          const dishCategory = dishCategories[dishCategoryIndex];

          if (dish.id === undefined) {
            dish.id = dishId;
            dishCategory.dishes.push(dish);
          } else {
            const dishIndex = dishCategory.dishes.findIndex(en => en.id === dish.id);
            dishCategory.dishes[dishIndex] = dish;
          }

          this.dishCategories$.next(this.dishCategories$.value);
          this.isUpdated$.next(true);
        }),
        catchError((response: HttpErrorResponse) => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      )
  }

  public removeDish(dishCategoryId: string, dishId: string): Observable<boolean> {
    this.isUpdating$.next(true);
    return this.restaurantAdminService.removeDishFromRestaurantAsync(this.restaurant$.value.id, dishCategoryId, dishId)
      .pipe(
        tap(() => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);

          const dishCategories = this.dishCategories$.value;
          const dishCategoryIndex = dishCategories.findIndex(en => en.id === dishCategoryId);
          const dishCategory = dishCategories[dishCategoryIndex];
          const dishIndex = dishCategory.dishes.findIndex(en => en.id === dishId);
          dishCategory.dishes.splice(dishIndex, 1);

          this.dishCategories$.next(this.dishCategories$.value);
          this.isUpdated$.next(true);
        }),
        catchError((response: HttpErrorResponse) => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      )
  }

  public decOrderOfDish(dishCategoryId: string, dishId: string): void {
    const dishCategories = this.dishCategories$.value;
    const indexDishCategory = dishCategories.findIndex(en => en.id === dishCategoryId);
    if (indexDishCategory < 0) {
      return;
    }
    const dishCategory = dishCategories[indexDishCategory];
    const pos = dishCategory.dishes.findIndex(en => en.id === dishId);
    if (pos < 1) {
      return;
    }

    this.isUpdating$.next(true);
    this.restaurantAdminService.decOrderOfDishAsync(this.restaurant$.value.id, dishId)
      .subscribe(() => {
        this.isUpdating$.next(false);
        this.updateError$.next(undefined);

        [dishCategory.dishes[pos - 1], dishCategory.dishes[pos]] = [dishCategory.dishes[pos], dishCategory.dishes[pos - 1]];

        this.dishCategories$.next(this.dishCategories$.value);
        this.isUpdated$.next(true);
      }, (response: HttpErrorResponse) => {
        this.isUpdating$.next(false);
        this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
      });
  }

  public incOrderOfDish(dishCategoryId: string, dishId: string): void {
    const dishCategories = this.dishCategories$.value;
    const indexDishCategory = dishCategories.findIndex(en => en.id === dishCategoryId);
    if (indexDishCategory < 0) {
      return;
    }
    const dishCategory = dishCategories[indexDishCategory];
    const pos = dishCategory.dishes.findIndex(en => en.id === dishId);
    if (pos >= dishCategory.dishes.length - 1) {
      return;
    }

    this.isUpdating$.next(true);
    this.restaurantAdminService.incOrderOfDishAsync(this.restaurant$.value.id, dishId)
      .subscribe(() => {
        this.isUpdating$.next(false);
        this.updateError$.next(undefined);

        [dishCategory.dishes[pos], dishCategory.dishes[pos + 1]] = [dishCategory.dishes[pos + 1], dishCategory.dishes[pos]];

        this.dishCategories$.next(this.dishCategories$.value);
        this.isUpdated$.next(true);
      }, (response: HttpErrorResponse) => {
        this.isUpdating$.next(false);
        this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
      });
  }

  private setUpdateInfoToRestaurant(restaurant: RestaurantModel)
  {
    restaurant.updatedOnDate = new Date();
    restaurant.updatedOn = restaurant.updatedOnDate.toISOString();
    restaurant.updatedBy = this.authService.getUser();
  }
}
