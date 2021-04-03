import {Component, OnInit, OnDestroy, ViewChild, AfterViewInit} from '@angular/core';

import {Observable} from "rxjs";

import {NgbModal} from '@ng-bootstrap/ng-bootstrap';

import {RestaurantModel} from '../../../shared/models/restaurant.model';

import {ServerPaginationComponent, FetchPageInfo} from '../../../shared/components/pagination/server-pagination.component';

import {SystemAdminFacade} from "../../system-admin.facade";

import {AddRestaurantComponent} from '../add-restaurant/add-restaurant.component';
import {ChangeRestaurantAccessSettingsComponent} from "../change-restaurant-access-settings/change-restaurant-access-settings.component";
import {ChangeRestaurantGeneralSettingsComponent} from "../change-restaurant-general-settings/change-restaurant-general-settings.component";
import {RemoveRestaurantComponent} from '../remove-restaurant/remove-restaurant.component';

@Component({
  selector: 'app-admin-restaurants',
  templateUrl: './admin-restaurants.component.html',
  styleUrls: [
    './admin-restaurants.component.css',
    '../../../../assets/css/frontend_v3.min.css',
    '../../../../assets/css/backend_v2.min.css'
  ]
})
export class AdminRestaurantsComponent implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild(ServerPaginationComponent) pagingComponent: ServerPaginationComponent;
  pageOfRestaurants: RestaurantModel[];

  public searchPhrase$: Observable<string>;

  constructor(
    private modalService: NgbModal,
    private facade: SystemAdminFacade
  ) {
  }

  ngOnInit() {
    this.searchPhrase$ = this.facade.getRestaurantSearchPhrase$();
    this.searchPhrase$
      .subscribe(
        () => {
          this.pagingComponent.triggerFetchPage(1);
        }
      );
  }

  ngAfterViewInit() {
    // ViewChild has to be rendered before it can be accessed
    this.pagingComponent.triggerFetchPage(1);
  }

  ngOnDestroy() {
  }

  onUpdateSearch(value: string) {
    this.facade.setRestaurantSearchPhrase(value);
  }

  onFetchPage(info: FetchPageInfo) {
    this.facade.searchForRestaurants$(info.skip, info.take)
      .subscribe((result) => {
        this.pageOfRestaurants = result.items;
        this.pagingComponent.updatePaging(result.total, result.items.length);
      }, () => {
      });
  }

  openAddRestaurantForm(): void {
    const modalRef = this.modalService.open(AddRestaurantComponent);
    modalRef.result.then(() => {
      this.pagingComponent.triggerFetchPage();
    }, () => {
    });
  }

  openChangeRestaurantGeneralSettingsForm(restaurant: RestaurantModel): void {
    const modalRef = this.modalService.open(ChangeRestaurantGeneralSettingsComponent);
    modalRef.componentInstance.restaurant = restaurant;
    modalRef.result.then(() => {
      this.pagingComponent.triggerFetchPage();
    }, () => {
    });
  }

  openChangeRestaurantAccessSettingsForm(restaurant: RestaurantModel): void {
    const modalRef = this.modalService.open(ChangeRestaurantAccessSettingsComponent);
    modalRef.componentInstance.restaurant = restaurant;
    modalRef.result.then(() => {
      this.pagingComponent.triggerFetchPage();
    }, () => {
    });
  }

  openRemoveRestaurantForm(restaurant: RestaurantModel): void {
    const modalRef = this.modalService.open(RemoveRestaurantComponent);
    modalRef.componentInstance.restaurant = restaurant;
    modalRef.result.then(() => {
      this.pagingComponent.triggerFetchPage();
    }, () => {
    });
  }

  onActivate(restaurant: RestaurantModel): void {
    this.facade.activateRestaurant$(restaurant.id)
      .subscribe(() => {
        restaurant.isActive = true;
      });
  }

  onDeactivate(restaurant: RestaurantModel): void {
    this.facade.deactivateRestaurant$(restaurant.id)
      .subscribe(() => {
        restaurant.isActive = false;
      });
  }

  onEnableSupport(restaurant: RestaurantModel): void {
    this.facade.enableSupportForRestaurant$(restaurant.id)
      .subscribe(() => {
        restaurant.needsSupport = true;
      });
  }

  onDisableSupport(restaurant: RestaurantModel): void {
    this.facade.disableSupportForRestaurant$(restaurant.id)
      .subscribe(() => {
        restaurant.needsSupport = false;
      });
  }

}
