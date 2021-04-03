import {Component, Input, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from "@angular/forms";

import {BehaviorSubject, Observable} from "rxjs";

import {NgbActiveModal} from "@ng-bootstrap/ng-bootstrap";

import {BlockUI, NgBlockUI} from "ng-block-ui";

import {RestaurantModel} from "../../../shared/models/restaurant.model";
import {CuisineStatus, SystemAdminFacade} from "../../system-admin.facade";

@Component({
  selector: 'app-change-restaurant-general-settings',
  templateUrl: './change-restaurant-general-settings.component.html',
  styleUrls: [
    './change-restaurant-general-settings.component.css',
    '../../../../assets/css/frontend_v3.min.css',
    '../../../../assets/css/modals.component.min.css'
  ]
})
export class ChangeRestaurantGeneralSettingsComponent implements OnInit {
  @Input() public restaurant: RestaurantModel;
  @BlockUI() blockUI: NgBlockUI;

  cuisineStatusArray$: BehaviorSubject<CuisineStatus[]> = new BehaviorSubject<CuisineStatus[]>(new Array<CuisineStatus>());

  changeSettingsForm: FormGroup;
  message$: Observable<string>;

  constructor(
    public activeModal: NgbActiveModal,
    private formBuilder: FormBuilder,
    private facade: SystemAdminFacade
  ) {
  }

  ngOnInit(): void {
    this.facade.getIsUpdating$()
      .subscribe(isUpdating => {
        if (isUpdating) {
          this.blockUI.start('Verarbeite Daten...');
        } else {
          this.blockUI.stop();
        }
      });

    this.message$ = this.facade.getUpdateError$();

    this.facade.getCuisines$()
      .subscribe(cuisines => {
        const cuisineStatusArray = this.cuisineStatusArray$.value;
        for (let cuisine of cuisines) {
          const status = this.restaurant.cuisines.some(en => en.id === cuisine.id);
          cuisineStatusArray.push(new CuisineStatus({
            id: cuisine.id,
            name: cuisine.name,
            oldStatus: status,
            newStatus: status
          }));
        }
        this.cuisineStatusArray$.next(this.cuisineStatusArray$.value);
      });

    this.changeSettingsForm = this.formBuilder.group({
      name: [this.restaurant.name, Validators.required],
      importId: [this.restaurant.importId]
    });
  }

  get f() {
    return this.changeSettingsForm.controls;
  }

  toggleCuisine(cuisineId: string): void {
    const cuisineStatusArray = this.cuisineStatusArray$.value;
    const index = cuisineStatusArray.findIndex(en => en.id === cuisineId);
    const cuisineStatus = cuisineStatusArray[index];
    cuisineStatus.newStatus = !cuisineStatus.newStatus;
    this.cuisineStatusArray$.next(this.cuisineStatusArray$.value);
  }

  onSubmit(data): void {
    if (!this.changeSettingsForm.valid) {
      return;
    }

    const cuisineStatusArray = this.cuisineStatusArray$.value;

    this.facade.updateRestaurantGeneralSettings$(
      this.restaurant,
      cuisineStatusArray,
      data.name,
      data.importId
    )
      .subscribe(
        () => {
          this.activeModal.close();
        }
      );
  }

}
