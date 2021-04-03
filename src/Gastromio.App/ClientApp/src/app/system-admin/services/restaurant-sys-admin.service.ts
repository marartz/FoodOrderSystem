import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';

import {Observable} from 'rxjs';
import {map, take} from "rxjs/operators";

import {RestaurantModel} from '../../shared/models/restaurant.model';
import {UserModel} from "../../shared/models/user.model";

import {PagingModel} from '../../shared/components/pagination/paging.model';

import {AuthService} from '../../auth/services/auth.service';

import {ImportLogModel} from '../models/import-log.model';

@Injectable()
export class RestaurantSysAdminService {

  private baseUrl = 'api/v1/systemadmin';

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {
  }

  public searchForRestaurantsAsync(search: string, skipCount: number, takeCode: number): Observable<PagingModel<RestaurantModel>> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.get<PagingModel<RestaurantModel>>(this.baseUrl + '/restaurants?search=' + encodeURIComponent(search)
      + '&skip=' + skipCount + '&take=' + takeCode, httpOptions)
      .pipe(take(1));
  }

  public searchForUsersAsync(search: string): Observable<UserModel[]> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.get<PagingModel<UserModel>>(this.baseUrl + '/users?search=' + encodeURIComponent(search), httpOptions)
      .pipe(
        take(1),
        map(pagingModel => {
          return pagingModel.items;
        })
      );
  }

  public addRestaurantAsync(name: string): Observable<RestaurantModel> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<RestaurantModel>(this.baseUrl + '/restaurants', {name}, httpOptions)
      .pipe(take(1));
  }

  public changeRestaurantNameAsync(id: string, name: string): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/changename',
      {name}, httpOptions)
      .pipe(take(1));
  }

  public setRestaurantImportIdAsync(id: string, importId: string): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/setimportid',
      {importId}, httpOptions)
      .pipe(take(1));
  }

  public activateRestaurantAsync(id: string): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/activate',
      {}, httpOptions)
      .pipe(take(1));
  }

  public deactivateRestaurantAsync(id: string): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/deactivate',
      {}, httpOptions)
      .pipe(take(1));
  }

  public addCuisineToRestaurantAsync(id: string, cuisineId: string): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/addcuisine', {cuisineId}, httpOptions)
      .pipe(take(1));
  }

  public removeCuisineFromRestaurantAsync(id: string, cuisineId: string): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/removecuisine', {cuisineId}, httpOptions)
      .pipe(take(1));
  }

  public addAdminToRestaurantAsync(id: string, userId: string): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/addadmin', {userId}, httpOptions)
      .pipe(take(1));
  }

  public removeAdminFromRestaurantAsync(id: string, userId: string): Observable<boolean> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.post<boolean>(this.baseUrl + '/restaurants/' + encodeURIComponent(id) + '/removeadmin', {userId}, httpOptions)
      .pipe(take(1));
  }

  public enableSupportForRestaurantAsync(restaurantId: string): Observable<void> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    const url = this.baseUrl + '/restaurants/' + encodeURIComponent(restaurantId) + '/enablesupport';
    return this.http.post<void>(url, {}, httpOptions)
      .pipe(take(1));
  }

  public disableSupportForRestaurantAsync(restaurantId: string): Observable<void> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    const url = this.baseUrl + '/restaurants/' + encodeURIComponent(restaurantId) + '/disablesupport';
    return this.http.post<void>(url, {}, httpOptions)
      .pipe(take(1));
  }

  public removeRestaurantAsync(restaurantId: string): Observable<void> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    return this.http.delete<void>(this.baseUrl + '/restaurants/' + restaurantId, httpOptions)
      .pipe(take(1));
  }

  public importRestaurantsAsync(importFile: File, dryRun: boolean): Observable<ImportLogModel> {
    const formData: FormData = new FormData();
    formData.append('fileKey', importFile, importFile.name);
    const httpOptions = {
      headers: new HttpHeaders({
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    let url = this.baseUrl + '/restaurants/import';
    if (dryRun) {
      url = url + '?dryrun=true';
    }
    return this.http.post<ImportLogModel>(url, formData, httpOptions)
      .pipe(take(1));
  }

  public importDishesAsync(importFile: File, dryRun: boolean): Observable<ImportLogModel> {
    const formData: FormData = new FormData();
    formData.append('fileKey', importFile, importFile.name);
    const httpOptions = {
      headers: new HttpHeaders({
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.authService.getToken(),
      })
    };
    let url = this.baseUrl + '/dishes/import';
    if (dryRun) {
      url = url + '?dryrun=true';
    }
    return this.http.post<ImportLogModel>(url, formData, httpOptions)
      .pipe(take(1));
  }

}
