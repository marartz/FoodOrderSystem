import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';

import {SystemAdminAuthGuardService} from '../auth/services/system-admin-auth-guard.service';

import {SystemAdminComponent} from "./components/system-admin/system-admin.component";

const routes: Routes = [
  {path: 'admin/users', component: SystemAdminComponent, data: {tab:'users'}, canActivate: [SystemAdminAuthGuardService]},
  {path: 'admin/cuisines', component: SystemAdminComponent, data: {tab:'cuisines'}, canActivate: [SystemAdminAuthGuardService]},
  {path: 'admin/restaurants', component: SystemAdminComponent, data: {tab:'restaurants'}, canActivate: [SystemAdminAuthGuardService]},
  {path: 'admin/restaurant-import', component: SystemAdminComponent, data: {tab:'restaurant-import'}, canActivate: [SystemAdminAuthGuardService]},
  {path: 'admin/dish-import', component: SystemAdminComponent, data: {tab:'dish-import'}, canActivate: [SystemAdminAuthGuardService]},
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SystemAdminRoutingModule {
}
