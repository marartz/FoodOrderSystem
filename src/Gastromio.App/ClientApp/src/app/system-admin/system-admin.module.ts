import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';

import {NgbModule} from '@ng-bootstrap/ng-bootstrap';

import {BlockUIModule} from 'ng-block-ui';

import {SystemAdminRoutingModule} from './system-admin.routing.module';

import {AuthModule} from '../auth/auth.module';
import {SharedModule} from '../shared/shared.module';

import {CuisineAdminService} from './services/cuisine-admin.service';
import {RestaurantSysAdminService} from './services/restaurant-sys-admin.service';
import {UserAdminService} from './services/user-admin.service';

import {AddCuisineComponent} from './components/add-cuisine/add-cuisine.component';
import {AddRestaurantComponent} from './components/add-restaurant/add-restaurant.component';
import {AddUserComponent} from './components/add-user/add-user.component';
import {AdminCuisinesComponent} from './components/admin-cuisines/admin-cuisines.component';
import {AdminDishImportComponent} from './components/admin-dish-import/admin-dish-import.component';
import {AdminRestaurantImportComponent} from './components/admin-restaurant-import/admin-restaurant-import.component';
import {AdminRestaurantsComponent} from './components/admin-restaurants/admin-restaurants.component';
import {AdminUsersComponent} from './components/admin-users/admin-users.component';
import {ChangeCuisineComponent} from './components/change-cuisine/change-cuisine.component';
import {ChangeRestaurantAccessSettingsComponent} from "./components/change-restaurant-access-settings/change-restaurant-access-settings.component";
import {ChangeRestaurantGeneralSettingsComponent} from "./components/change-restaurant-general-settings/change-restaurant-general-settings.component";
import {ChangeUserDetailsComponent} from './components/change-user-details/change-user-details.component';
import {ChangeUserPasswordComponent} from './components/change-user-password/change-user-password.component';
import {RemoveCuisineComponent} from './components/remove-cuisine/remove-cuisine.component';
import {RemoveRestaurantComponent} from './components/remove-restaurant/remove-restaurant.component';
import {RemoveUserComponent} from './components/remove-user/remove-user.component';
import {SystemAdminComponent} from "./components/system-admin/system-admin.component";
import {SystemAdminFacade} from "./system-admin.facade";

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
    BlockUIModule.forRoot(),
    SystemAdminRoutingModule,
    SharedModule,
    AuthModule
  ],
  declarations: [
    AddCuisineComponent,
    AddRestaurantComponent,
    AddUserComponent,
    AdminCuisinesComponent,
    AdminDishImportComponent,
    AdminRestaurantImportComponent,
    AdminRestaurantsComponent,
    AdminUsersComponent,
    ChangeCuisineComponent,
    ChangeRestaurantAccessSettingsComponent,
    ChangeRestaurantGeneralSettingsComponent,
    ChangeUserDetailsComponent,
    ChangeUserPasswordComponent,
    RemoveCuisineComponent,
    RemoveRestaurantComponent,
    RemoveUserComponent,
    SystemAdminComponent
  ],
  providers: [
    CuisineAdminService,
    RestaurantSysAdminService,
    UserAdminService,
    SystemAdminFacade
  ]
})
export class SystemAdminModule {
}
