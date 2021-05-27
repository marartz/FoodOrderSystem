import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';

import {Observable, throwError} from 'rxjs';
import {catchError, map, take, tap} from 'rxjs/operators';

import {UserModel} from '../../shared/models/user.model';

@Injectable()
export class AuthService {
  private loginUrl = 'api/v1/auth/login';
  private requestPasswordChangeUrl = 'api/v1/auth/requestpasswordchange';
  private validatePasswordResetCodeUrl = 'api/v1/auth/validatepasswordresetcode';
  private changePasswordWithResetCodeUrl = 'api/v1/auth/changepasswordwithresetcode';
  private changePasswordUrl = 'api/v1/auth/changepassword';
  private pingUrl = 'api/v1/auth/ping';

  constructor(
    private http: HttpClient
  ) {
  }

  public isAuthenticated(): boolean {
    return this.getToken() !== undefined && this.getUser() !== undefined;
  }

  public getToken(): string {
    const token = localStorage.getItem('token');
    if (token === null) {
      return undefined;
    }
    return token;
  }

  public getUser(): UserModel {
    const userJSON = localStorage.getItem('user');
    if (userJSON === null) {
      return undefined;
    }
    return JSON.parse(userJSON);
  }

  public loginAsync(email: string, password: string): Observable<UserModel> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
      })
    };

    return this.http.post<LoginResultModel>(this.loginUrl, {email, password}, httpOptions)
      .pipe(
        take(1),
        tap((loginResult: LoginResultModel) => {
          console.log('loginResult: ', loginResult);
          localStorage.setItem('token', loginResult.token);
          localStorage.setItem('user', JSON.stringify(loginResult.user));
        }),
        map((loginResult: LoginResultModel) => loginResult.user),
        catchError(error => {
          console.log('error: ', error);
          return throwError(error);
        })
      );
  }

  public requestPasswordChangeAsync(email: string): Observable<void> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
      })
    };

    return this.http.post(this.requestPasswordChangeUrl, {userEmail: email}, httpOptions)
      .pipe(
        take(1),
        map(() => {})
      );
  }

  public validatePasswordResetCodeAsync(userId: string, passwordResetCode: string): Observable<void> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
      })
    };

    return this.http.post(this.validatePasswordResetCodeUrl, {userId, passwordResetCode}, httpOptions)
      .pipe(
        take(1),
        map(() => {})
      );
  }

  public changePasswordWithResetCodeAsync(userId: string, passwordResetCode: string, password: string): Observable<void> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
      })
    };

    return this.http.post(this.changePasswordWithResetCodeUrl, {userId, passwordResetCode, password}, httpOptions)
      .pipe(
        take(1),
        map(() => {})
      );
  }

  public changePasswordAsync(password: string): Observable<void> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.getToken(),
      })
    };

    return this.http.post(this.changePasswordUrl, {password}, httpOptions)
      .pipe(
        take(1),
        map(() => {})
      );
  }

  public pingAsync(): Observable<void> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: 'Bearer ' + this.getToken(),
      })
    };

    return this.http.get<{}>(this.pingUrl, httpOptions)
      .pipe(
        take(1),
        map(() => {})
      );
  }

  public logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  }
}

class LoginResultModel {
  public token: string;
  public user: UserModel;
}
