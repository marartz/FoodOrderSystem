import {Injectable} from "@angular/core";
import {Router} from "@angular/router";

import {BehaviorSubject, combineLatest, Observable, of, throwError} from "rxjs";
import {catchError, concatMap, debounceTime, distinctUntilChanged, map, tap} from "rxjs/operators";

import {HttpErrorHandlingService} from "../shared/services/http-error-handling.service";

import {UserModel} from "../shared/models/user.model";

import {CuisineAdminService} from "./services/cuisine-admin.service";
import {UserAdminService} from "./services/user-admin.service";
import {RestaurantSysAdminService} from "./services/restaurant-sys-admin.service";
import {RestaurantModel} from "../shared/models/restaurant.model";
import {CuisineModel} from "../shared/models/cuisine.model";
import {HttpErrorResponse} from "@angular/common/http";
import {PagingModel} from "../shared/components/pagination/paging.model";
import {ImportLogModel} from "./models/import-log.model";

@Injectable()
export class SystemAdminFacade {

  private isInitializing$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(true);
  private isInitialized$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(undefined);
  private initializationError$: BehaviorSubject<string> = new BehaviorSubject<string>(undefined);

  private selectedTab$: BehaviorSubject<string> = new BehaviorSubject<string>("general");

  private userSearchPhrase$: BehaviorSubject<string> = new BehaviorSubject<string>('');

  private cuisines$: BehaviorSubject<CuisineModel[]> = new BehaviorSubject<CuisineModel[]>(undefined);

  private restaurantSearchPhrase$: BehaviorSubject<string> = new BehaviorSubject<string>('');

  private isSearchingFor$: BehaviorSubject<string> = new BehaviorSubject<string>(undefined);

  private isUpdating$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  private isUpdated$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(undefined);
  private updateError$: BehaviorSubject<string> = new BehaviorSubject<string>(undefined);

  static earliestOpeningTime: number = 4 * 60;

  constructor(
    private sysAdminService: RestaurantSysAdminService,
    private router: Router,
    private userAdminService: UserAdminService,
    private cuisineAdminService: CuisineAdminService,
    private restaurantSysAdminService: RestaurantSysAdminService,
    private httpErrorHandlingService: HttpErrorHandlingService
  ) {
  }

  public initialize(tab: string): void {
    this.selectedTab$.next(tab);

    const observables = new Array<Observable<void>>();

    observables
      .push(this.cuisineAdminService.getAllCuisinesAsync()
        .pipe(
          map(cuisines => {
            cuisines.sort(SystemAdminFacade.compareCuisine);
            this.cuisines$.next(cuisines);
          }),
          catchError(response => {
            return throwError(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          })
        )
      );

    combineLatest(observables)
      .subscribe(
        () => {
          this.isInitializing$.next(false);
          this.initializationError$.next(undefined);
          this.isInitialized$.next(true);
        },
        error => {
          this.isInitializing$.next(false);
          this.initializationError$.next(error);
          this.isInitialized$.next(false);
        }
      )
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

  public getSelectedTab$(): Observable<string> {
    return this.selectedTab$;
  }

  public getUserSearchPhrase$(): Observable<string> {
    return this.userSearchPhrase$.pipe(debounceTime(500), distinctUntilChanged());
  }

  public getCuisines$(): Observable<CuisineModel[]> {
    return this.cuisines$;
  }

  public getRestaurantSearchPhrase$(): Observable<string> {
    return this.restaurantSearchPhrase$.pipe(debounceTime(500), distinctUntilChanged());
  }

  public getIsSearchingFor$(): Observable<string> {
    return this.isSearchingFor$;
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


  // actions

  public ackIsUpdated()
  {
    this.isUpdated$.next(undefined);
  }

  public selectTab(tab: string): void {
    if (this.selectedTab$.value !== 'users') {
      this.userSearchPhrase$.next('');
    }

    if (this.selectedTab$.value !== 'restaurants') {
      this.restaurantSearchPhrase$.next('');
    }

    this.router.navigate(['admin', tab],);
  }

  public setUserSearchPhrase(userSearchPhrase: string): void {
    this.userSearchPhrase$.next(userSearchPhrase);
  }

  public searchForUsers$(skip: number, take: number): Observable<PagingModel<UserModel>> {
    const searchPhrase = this.userSearchPhrase$.value;
    this.isSearchingFor$.next('Benutzer');
    return this.userAdminService.searchForUsersAsync(searchPhrase, skip, take)
      .pipe(
        tap(() => {
          this.isSearchingFor$.next(undefined);
        }),
        catchError(error => {
          this.isSearchingFor$.next(undefined);
          return throwError(error);
        })
      );
  }

  public addUser$(role: string, email: string, password: string): Observable<void> {
    this.isUpdating$.next(true);
    return this.userAdminService.addUserAsync(role, email, password)
      .pipe(
        map(() => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);
          this.isUpdated$.next(true);
        }),
        catchError(response => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public changeUserDetails$(userId: string, role: string, email: string): Observable<void> {
    this.isUpdating$.next(true);
    return this.userAdminService.changeUserDetailsAsync(userId, role, email)
      .pipe(
        map(() => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);
          this.isUpdated$.next(true);
        }),
        catchError(response => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public changeUserPassword$(userId: string, password: string): Observable<void> {
    this.isUpdating$.next(true);
    return this.userAdminService.changeUserPasswordAsync(userId, password)
      .pipe(
        map(() => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);
          this.isUpdated$.next(true);
        }),
        catchError(response => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public removeUser$(userId: string): Observable<void> {
    this.isUpdating$.next(true);
    return this.userAdminService.removeUserAsync(userId)
      .pipe(
        map(() => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);
          this.isUpdated$.next(true);
        }),
        catchError(response => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public addCuisine$(name: string): Observable<void> {
    this.isUpdating$.next(true);
    return this.cuisineAdminService.addCuisineAsync(name)
      .pipe(
        map(cuisine => {
          const cuisines = this.cuisines$.value;
          cuisines.push(cuisine);
          cuisines.sort(SystemAdminFacade.compareCuisine);
          this.cuisines$.next(cuisines);

          this.isUpdating$.next(false);
          this.updateError$.next(undefined);
          this.isUpdated$.next(true);
        }),
        catchError(response => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public changeCuisine$(cuisineId: string, name: string): Observable<void> {
    this.isUpdating$.next(true);
    return this.cuisineAdminService.changeCuisineAsync(cuisineId, name)
      .pipe(
        map(() => {
          const cuisines = this.cuisines$.value;
          let cuisine = cuisines.find(en => en.id === cuisineId);
          cuisine.name = name;
          cuisines.sort(SystemAdminFacade.compareCuisine);
          this.cuisines$.next(cuisines);

          this.isUpdating$.next(false);
          this.updateError$.next(undefined);
          this.isUpdated$.next(true);
        }),
        catchError(response => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public removeCuisine$(cuisineId: string): Observable<void> {
    this.isUpdating$.next(true);
    return this.cuisineAdminService.removeCuisineAsync(cuisineId)
      .pipe(
        map(() => {
          const cuisines = this.cuisines$.value;
          let cuisineIdx = cuisines.findIndex(en => en.id === cuisineId);
          cuisines.splice(cuisineIdx, 1);
          this.cuisines$.next(cuisines);

          this.isUpdating$.next(false);
          this.updateError$.next(undefined);
          this.isUpdated$.next(true);
        }),
        catchError(response => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public setRestaurantSearchPhrase(restaurantSearchPhrase: string): void {
    this.restaurantSearchPhrase$.next(restaurantSearchPhrase);
  }

  public searchForRestaurants$(skip: number, take: number): Observable<PagingModel<RestaurantModel>> {
    const searchPhrase = this.restaurantSearchPhrase$.value;
    this.isSearchingFor$.next('Restaurants');
    return this.restaurantSysAdminService.searchForRestaurantsAsync(searchPhrase, skip, take)
      .pipe(
        tap(() => {
          this.isSearchingFor$.next(undefined);
        }),
        catchError(error => {
          this.isSearchingFor$.next(undefined);
          return throwError(error);
        })
      );
  }

  public addRestaurant$(name: string): Observable<void> {
    this.isUpdating$.next(true);
    return this.restaurantSysAdminService.addRestaurantAsync(name)
      .pipe(
        map(() => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);
          this.isUpdated$.next(true);
        }),
        catchError(response => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public searchForAdminUsers$(search: string): Observable<UserModel[]> {
    return this.restaurantSysAdminService.searchForUsersAsync(search);
  }

  public updateAdministratorsOfRestaurant$(restaurant: RestaurantModel, administrators: Array<UserModel>): void {
    this.isUpdating$.next(true);

    let curObservable: Observable<boolean> = undefined;

    for (let index = administrators.length - 1; index >= 0; index--) {
      const administrator = administrators[index];

      const adminIndex = restaurant.administrators !== undefined
        ? restaurant.administrators.findIndex(en => en.id === administrator.id)
        : -1;

      if (adminIndex < 0) {
        const nextObservable = this.restaurantSysAdminService.addAdminToRestaurantAsync(restaurant.id, administrator.id)
          .pipe(
            tap(() => {
              restaurant.administrators.push(administrator);
            })
          );
        if (curObservable !== undefined) {
          curObservable = curObservable.pipe(concatMap(() => nextObservable));
        } else {
          curObservable = nextObservable;
        }
      }
    }

    if (restaurant.administrators !== undefined) {
      for (let index = restaurant.administrators.length - 1; index >= 0; index--) {
        const administrator = restaurant.administrators[index];

        const adminIndex = administrators.findIndex(en => en.id === administrator.id);
        if (adminIndex < 0) {
          const nextObservable = this.restaurantSysAdminService.removeAdminFromRestaurantAsync(restaurant.id, administrator.id)
            .pipe(
              tap(() => {
                const tempIndex = restaurant.administrators.findIndex(en => en.id === administrator.id);
                restaurant.administrators.splice(tempIndex, 1);
              })
            );
          if (curObservable !== undefined) {
            curObservable = curObservable.pipe(concatMap(() => nextObservable));
          } else {
            curObservable = nextObservable;
          }
        }
      }
    }

    if (curObservable === undefined) {
      curObservable = of(true);
    }

    curObservable
      .subscribe(
        () => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);
          this.isUpdated$.next(true);
        },
        response => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        }
      );
  }

  public updateRestaurantGeneralSettings$(restaurant: RestaurantModel, cuisineStatusArray: Array<CuisineStatus>, name: string, importId: string): Observable<void> {
    if (!cuisineStatusArray.some(en => en.newStatus)) {
      this.updateError$.next("Bitte wähle mindestens eine Cuisine aus.");
      return;
    }

    this.isUpdating$.next(true);

    let curObservable: Observable<boolean> = undefined;

    for (let index = cuisineStatusArray.length - 1; index >= 0; index--) {
      const cuisineStatus = cuisineStatusArray[index];

      if (cuisineStatus.oldStatus != cuisineStatus.newStatus) {
        let nextChangeCuisineObservable: Observable<boolean> = undefined;
        if (cuisineStatus.newStatus) {
          nextChangeCuisineObservable = this.restaurantSysAdminService.addCuisineToRestaurantAsync(restaurant.id, cuisineStatus.id)
            .pipe(
              tap(() => {
                cuisineStatusArray[index].oldStatus = true;
                cuisineStatusArray[index].newStatus = true;
              })
            );
        } else {
          nextChangeCuisineObservable = this.restaurantSysAdminService.removeCuisineFromRestaurantAsync(restaurant.id, cuisineStatus.id)
            .pipe(
              tap(() => {
                cuisineStatusArray[index].oldStatus = false;
                cuisineStatusArray[index].newStatus = false;
              })
            );
        }

        if (curObservable !== undefined) {
          curObservable = curObservable.pipe(concatMap(() => nextChangeCuisineObservable));
        } else {
          curObservable = nextChangeCuisineObservable;
        }
      }
    }

    if (restaurant.importId !== importId) {
      const nextObservable = this.restaurantSysAdminService.setRestaurantImportIdAsync(restaurant.id, importId)
        .pipe(
          tap(() => {
            restaurant.importId = importId;
          })
        );
      if (curObservable !== undefined) {
        curObservable = curObservable.pipe(concatMap(() => nextObservable));
      } else {
        curObservable = nextObservable;
      }
    }

    let observable: Observable<boolean>;

    if (restaurant.name !== name) {
      observable = this.restaurantSysAdminService.changeRestaurantNameAsync(restaurant.id, name)
        .pipe(
          tap(() => {
            restaurant.name = name;
          }),
          concatMap(() => curObservable ?? of(true))
        )
    } else {
      observable = curObservable ?? of(true);
    }

    return observable
      .pipe(
        map(() => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);
          this.isUpdated$.next(true);
        }),
        catchError((response: HttpErrorResponse) => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      )
  }

  public activateRestaurant$(restaurantId: string): Observable<void> {
    this.isUpdating$.next(true);
    return this.restaurantSysAdminService.activateRestaurantAsync(restaurantId)
      .pipe(
        map(() => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);
          this.isUpdated$.next(true);
        }),
        catchError((response: HttpErrorResponse) => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public deactivateRestaurant$(restaurantId: string): Observable<void> {
    this.isUpdating$.next(true);
    return this.restaurantSysAdminService.deactivateRestaurantAsync(restaurantId)
      .pipe(
        map(() => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);
          this.isUpdated$.next(true);
        }),
        catchError((response: HttpErrorResponse) => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public enableSupportForRestaurant$(restaurantId: string): Observable<void> {
    this.isUpdating$.next(true);
    return this.restaurantSysAdminService.enableSupportForRestaurantAsync(restaurantId)
      .pipe(
        map(() => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);
          this.isUpdated$.next(true);
        }),
        catchError((response: HttpErrorResponse) => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public disableSupportForRestaurant$(restaurantId: string): Observable<void> {
    this.isUpdating$.next(true);
    return this.restaurantSysAdminService.disableSupportForRestaurantAsync(restaurantId)
      .pipe(
        map(() => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);
          this.isUpdated$.next(true);
        }),
        catchError((response: HttpErrorResponse) => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public removeRestaurant$(restaurantId: string): Observable<void> {
    this.isUpdating$.next(true);
    return this.restaurantSysAdminService.removeRestaurantAsync(restaurantId)
      .pipe(
        map(() => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);
          this.isUpdated$.next(true);
        }),
        catchError(response => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public importRestaurants$(importFile: File, dryRun: boolean): Observable<ImportLogModel> {
    if (!importFile) {
      this.updateError$.next('Bitte wähle erst eine Importdatei aus.');
      return of(undefined);
    }

    this.isUpdating$.next(true);
    return this.restaurantSysAdminService.importRestaurantsAsync(importFile, dryRun)
      .pipe(
        tap(() => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);
        }),
        catchError(response => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  public importDishes$(importFile: File, dryRun: boolean): Observable<ImportLogModel> {
    if (!importFile) {
      this.updateError$.next('Bitte wähle erst eine Importdatei aus.');
      return of(undefined);
    }

    this.isUpdating$.next(true);
    return this.restaurantSysAdminService.importDishesAsync(importFile, dryRun)
      .pipe(
        tap(() => {
          this.isUpdating$.next(false);
          this.updateError$.next(undefined);
        }),
        catchError(response => {
          this.isUpdating$.next(false);
          this.updateError$.next(this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors());
          return throwError(response);
        })
      );
  }

  private static compareCuisine(a: CuisineModel, b: CuisineModel): number {
    if (a.name < b.name)
      return -1;
    if (a.name > b.name)
      return 1;
    return 0;
  }

}

export class CuisineStatus {

  constructor(init?: Partial<CuisineStatus>) {
    if (init) {
      Object.assign(this, init);
    }
  }

  id: string;
  name: string;
  oldStatus: boolean;
  newStatus: boolean;

}
