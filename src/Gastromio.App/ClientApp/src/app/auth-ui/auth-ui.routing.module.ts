import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';

import {LoginAuthGuardService} from "../auth/services/login-auth-guard.service";

import {LoginComponent} from './components/login/login.component';
import {ChangePasswordComponent} from './components/change-password/change-password.component';
import {ForgotPasswordComponent} from './components/forgot-password/forgot-password.component';
import {ResetPasswordComponent} from './components/reset-password/reset-password.component';

const routes: Routes = [
  {path: 'login', component: LoginComponent},
  {path: 'changepassword', component: ChangePasswordComponent, canActivate: [LoginAuthGuardService]},
  {path: 'forgotpassword', component: ForgotPasswordComponent},
  {path: 'resetpassword', component: ResetPasswordComponent},
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthUiRoutingModule {
}
