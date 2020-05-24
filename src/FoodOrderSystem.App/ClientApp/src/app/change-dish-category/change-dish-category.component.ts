import { Component, OnInit, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { DishCategoryModel } from '../dish-category/dish-category.model';
import { RestaurantRestAdminService } from '../restaurant-rest-admin/restaurant-rest-admin.service';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { HttpErrorHandlingService } from '../http-error-handling/http-error-handling.service';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-change-dish-category',
  templateUrl: './change-dish-category.component.html',
  styleUrls: ['./change-dish-category.component.css', '../../assets/css/admin-forms.min.css']
})
export class ChangeDishCategoryComponent implements OnInit {
  @Input() public restaurantId: string;
  @Input() public dishCategory: DishCategoryModel;
  @BlockUI() blockUI: NgBlockUI;

  changeDishCategoryForm: FormGroup;
  message: string;

  constructor(
    public activeModal: NgbActiveModal,
    private formBuilder: FormBuilder,
    private restaurantAdminService: RestaurantRestAdminService,
    private httpErrorHandlingService: HttpErrorHandlingService
  ) {
  }

  ngOnInit() {
    this.changeDishCategoryForm = this.formBuilder.group({
      name: [this.dishCategory.name, Validators.required]
    });
  }

  get f() { return this.changeDishCategoryForm.controls; }

  onSubmit(data) {
    if (this.changeDishCategoryForm.invalid) {
      return;
    }

    this.blockUI.start("Verarbeite Daten...");
    let subscription = this.restaurantAdminService.changeDishCategoryOfRestaurantAsync(this.restaurantId, this.dishCategory.id, data.name)
      .subscribe(() => {
        subscription.unsubscribe();
        this.blockUI.stop();
        this.message = undefined;
        this.changeDishCategoryForm.reset();
        this.activeModal.close(new DishCategoryModel({ id: this.dishCategory.id, name: data.name }));
      }, (response: HttpErrorResponse) => {
        subscription.unsubscribe();
        this.blockUI.stop();
        this.message = this.httpErrorHandlingService.handleError(response);
      });
  }
}
