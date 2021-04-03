import {Component, OnInit, OnDestroy} from '@angular/core';
import {Router} from '@angular/router';

import {RestaurantModel} from '../../../shared/models/restaurant.model';

import {AuthService} from '../../../auth/services/auth.service';

import {RestaurantRestAdminService} from '../../services/restaurant-rest-admin.service';

@Component({
  selector: 'app-admin-my-restaurants',
  templateUrl: './admin-my-restaurants.component.html',
  styleUrls: [
    './admin-my-restaurants.component.css',
    '../../../../assets/css/frontend_v3.min.css',
    '../../../../assets/css/backend_v2.min.css'
  ]
})
export class AdminMyRestaurantsComponent implements OnInit, OnDestroy {
  restaurants: RestaurantModel[];

  constructor(
    private restaurantAdminService: RestaurantRestAdminService,
    private authService: AuthService,
    private router: Router
  ) {
  }

  ngOnInit() {
    this.updateSearch();
  }

  ngOnDestroy() {
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['']);
  }

  updateSearch(): void {
    this.restaurantAdminService.getMyRestaurantsAsync()
      .subscribe((result) => {
        this.restaurants = result;
      }, (error) => {
        // TODO
      });
  }
}
