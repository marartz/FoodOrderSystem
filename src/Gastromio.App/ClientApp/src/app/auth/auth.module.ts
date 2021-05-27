import {NgModule} from '@angular/core';

import {HttpClientModule} from '@angular/common/http';

import {SharedModule} from '../shared/shared.module';

import {AuthService} from './services/auth.service';

import {CustomerAuthGuardService} from './services/customer-auth-guard.service';
import {LoginAuthGuardService} from "./services/login-auth-guard.service";
import {RestaurantAdminAuthGuardService} from './services/restaurant-admin-auth-guard.service';
import {SystemAdminAuthGuardService} from './services/system-admin-auth-guard.service';

@NgModule({
  imports: [
    HttpClientModule,
    SharedModule
  ],
  declarations: [
  ],
  providers: [
    AuthService,
    LoginAuthGuardService,
    CustomerAuthGuardService,
    RestaurantAdminAuthGuardService,
    SystemAdminAuthGuardService
  ]
})
export class AuthModule {
}
