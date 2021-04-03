import {Component, OnDestroy, OnInit} from '@angular/core';

import {BehaviorSubject, Subscription} from "rxjs";

import {NgbModal} from "@ng-bootstrap/ng-bootstrap";

import {DishCategoryModel} from "../../../shared/models/dish-category.model";
import {DishModel} from "../../../shared/models/dish.model";
import {DishVariantModel} from "../../../shared/models/dish-variant.model";

import {RestaurantAdminFacade} from "../../restaurant-admin.facade";

import {AddDishCategoryComponent} from "../add-dish-category/add-dish-category.component";
import {ChangeDishCategoryComponent} from "../change-dish-category/change-dish-category.component";
import {EditDishComponent} from "../edit-dish/edit-dish.component";
import {RemoveDishCategoryComponent} from "../remove-dish-category/remove-dish-category.component";
import {RemoveDishComponent} from "../remove-dish/remove-dish.component";
import {FormBuilder, FormGroup, Validators} from "@angular/forms";
import {debounceTime} from "rxjs/operators";

@Component({
  selector: 'app-dish-management',
  templateUrl: './dish-management.component.html',
  styleUrls: [
    './dish-management.component.css',
    '../../../../assets/css/frontend_v3.min.css',
    '../../../../assets/css/backend_v2.min.css'
  ]
})
export class DishManagementComponent implements OnInit, OnDestroy {

  public externalMenuForm: FormGroup;

  public dishCategories: DishCategoryModel[];

  public activeDishCategoryId$: BehaviorSubject<string> = new BehaviorSubject<string>(undefined);

  private restaurantSubscription: Subscription;
  private dishCategoriesSubscription: Subscription;

  private externalMenuId: string = "EA9D3F69-4709-4F4A-903C-7EA68C0A36C7".toLocaleLowerCase();

  constructor(
    private facade: RestaurantAdminFacade,
    private formBuilder: FormBuilder,
    private modalService: NgbModal
  ) {
  }

  ngOnInit(): void {
    this.externalMenuForm = this.formBuilder.group({
      enabled: [false, Validators.required],
      name: [''],
      description: [''],
      url: ['']
    });
    this.externalMenuForm.valueChanges
      .pipe(debounceTime(1000))
      .subscribe(value => {
        const nameControl = this.externalMenuForm.get('name');
        const descriptionControl = this.externalMenuForm.get('description');
        const urlControl = this.externalMenuForm.get('url');

        if (this.externalMenuForm.value.enabled) {
          nameControl.setValidators([Validators.required]);
          nameControl.updateValueAndValidity();
          descriptionControl.setValidators([Validators.required]);
          descriptionControl.updateValueAndValidity();
          urlControl.setValidators([Validators.required, Validators.pattern(/^(https?:\/\/){1,1}(www\.)?[-a-zäöüA-ZÄÖÜ0-9@:%._\+~#=]{1,256}\.[a-zäöüA-ZÄÖÜ0-9()]{1,6}\b([-a-zäöüA-ZÄÖÜ0-9()@:%_\+.~#?&//=]*)$/)]);
          urlControl.updateValueAndValidity();
        } else {
          nameControl.setValidators(null);
          nameControl.updateValueAndValidity();
          descriptionControl.setValidators(null);
          descriptionControl.updateValueAndValidity();
          urlControl.setValidators(null);
          urlControl.updateValueAndValidity();
        }

        if (this.externalMenuForm.dirty && this.externalMenuForm.valid) {
          if (this.externalMenuForm.value.enabled) {

            this.facade.addOrChangeExternalMenu(
              this.externalMenuId,
              value.name,
              value.description,
              value.url
            );
          } else {
            this.facade.removeExternalMenu(this.externalMenuId);
          }
        }
        this.externalMenuForm.markAsPristine();
      });

    this.restaurantSubscription = this.facade.getRestaurant$().subscribe(
      restaurant => {
        const index = restaurant.externalMenus?.findIndex(en => en.id === this.externalMenuId) ?? -1;
        if (index >= 0) {
          const externalMenu = restaurant.externalMenus[index];
          this.externalMenuForm.patchValue({
            enabled: true,
            name: externalMenu.name,
            description: externalMenu.description,
            url: externalMenu.url
          });
          this.externalMenuForm.markAsPristine();
        } else {
          this.externalMenuForm.patchValue({
            enabled: false
          });
        }
      }
    );

    this.dishCategoriesSubscription = this.facade.getDishCategories$().subscribe(
      dishCategories => {
        this.dishCategories = dishCategories;

        if (this.activeDishCategoryId$.value === undefined) {
          this.activeDishCategoryId$.next(dishCategories.length > 0 ? dishCategories[0].id : undefined);
        }
        const index = dishCategories.findIndex(en => en.id === this.activeDishCategoryId$.value)
        if (index < 0) {
          this.activeDishCategoryId$.next(dishCategories.length > 0 ? dishCategories[0].id : undefined);
        }
      }
    );
  }

  ngOnDestroy(): void {
    this.restaurantSubscription?.unsubscribe();
    this.dishCategoriesSubscription?.unsubscribe();
  }

  get emf() {
    return this.externalMenuForm.controls;
  }

  public isFirstDishCategory(dishCategory: DishCategoryModel): boolean {
    const pos = this.dishCategories.findIndex(en => en.id === dishCategory.id);
    return pos === 0;
  }

  public isLastDishCategory(dishCategory: DishCategoryModel): boolean {
    const pos = this.dishCategories.findIndex(en => en.id === dishCategory.id);
    return pos === this.dishCategories.length - 1;
  }

  public isFirstDish(dishCategory: DishCategoryModel, dish: DishModel): boolean {
    const pos = dishCategory.dishes.findIndex(en => en.id === dish.id);
    return pos === 0;
  }

  public isLastDish(dishCategory: DishCategoryModel, dish: DishModel): boolean {
    const pos = dishCategory.dishes.findIndex(en => en.id === dish.id);
    return pos === dishCategory.dishes.length - 1;
  }

  public getPriceOfVariant(variant: DishVariantModel): string {
    return '€' + variant.price.toLocaleString('de', {minimumFractionDigits: 2});
  }

  public onChangeActiveDishCategoryId(dishCategoryId: string): void {
    this.activeDishCategoryId$.next(dishCategoryId);
  }

  public openAddDishCategoryForm(): void {
    const modalRef = this.modalService.open(AddDishCategoryComponent);
    if (this.dishCategories !== undefined && this.dishCategories.length > 0) {
      modalRef.componentInstance.afterCategoryId = this.dishCategories[this.dishCategories.length - 1].id;
    }
    modalRef.result.then(id => {
      this.activeDishCategoryId$.next(id);
    }, () => {
    });
  }

  public openChangeDishCategoryForm(dishCategory: DishCategoryModel): void {
    const modalRef = this.modalService.open(ChangeDishCategoryComponent);
    modalRef.componentInstance.dishCategory = dishCategory.clone();
    modalRef.result.then(() => {
    }, () => {
    });
  }

  public onEnableDishCategory(dishCategory: DishCategoryModel): void {
    this.facade.enableDishCategory(dishCategory.id);
  }

  public onDisableDishCategory(dishCategory: DishCategoryModel): void {
    this.facade.disableDishCategory(dishCategory.id);
  }

  public openRemoveDishCategoryForm(dishCategory: DishCategoryModel): void {
    const modalRef = this.modalService.open(RemoveDishCategoryComponent);
    modalRef.componentInstance.dishCategory = dishCategory.clone();
    modalRef.result.then(() => {
    }, () => {
    });
  }

  public decOrderOfDishCategory(dishCategory: DishCategoryModel): void {
    this.facade.decOrderOfDishCategory(dishCategory.id);
  }

  public incOrderOfDishCategory(dishCategory: DishCategoryModel): void {
    this.facade.incOrderOfDishCategory(dishCategory.id);
  }

  public openEditDishForm(dishCategory: DishCategoryModel, dish: DishModel): void {
    const modalRef = this.modalService.open(EditDishComponent);
    modalRef.componentInstance.dishCategoryId = dishCategory.id;
    modalRef.componentInstance.dish = dish.clone();
    modalRef.result.then(() => {
    }, () => {
    });
  }

  public openRemoveDishForm(dishCategory: DishCategoryModel, dish: DishModel): void {
    const modalRef = this.modalService.open(RemoveDishComponent);
    modalRef.componentInstance.dishCategoryId = dishCategory.id;
    modalRef.componentInstance.dish = dish.clone();
    modalRef.result.then(() => {
    }, () => {
    });
  }

  public decOrderOfDish(dishCategory: DishCategoryModel, dish: DishModel): void {
    this.facade.decOrderOfDish(dishCategory.id, dish.id);
  }

  public incOrderOfDish(dishCategory: DishCategoryModel, dish: DishModel): void {
    this.facade.incOrderOfDish(dishCategory.id, dish.id);
  }

}
