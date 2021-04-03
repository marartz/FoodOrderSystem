import {Component, OnInit, Input} from '@angular/core';
import {FormGroup, FormBuilder, Validators} from '@angular/forms';

import {Observable} from "rxjs";

import {NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';

import {BlockUI, NgBlockUI} from 'ng-block-ui';

import {RestaurantAdminFacade} from "../../restaurant-admin.facade";

@Component({
  selector: 'app-add-dish-category',
  templateUrl: './add-dish-category.component.html',
  styleUrls: [
    './add-dish-category.component.css',
    '../../../../assets/css/frontend_v3.min.css',
    '../../../../assets/css/modals.component.min.css'
  ]
})
export class AddDishCategoryComponent implements OnInit {

  @Input() public afterCategoryId: string;
  @BlockUI() blockUI: NgBlockUI;

  addDishCategoryForm: FormGroup;
  message$: Observable<string>;
  submitted = false;

  constructor(
    public activeModal: NgbActiveModal,
    private formBuilder: FormBuilder,
    private facade: RestaurantAdminFacade
  ) {
    this.addDishCategoryForm = this.formBuilder.group({
      name: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    this.message$ = this.facade.getUpdateError$();
  }

  get f() {
    return this.addDishCategoryForm.controls;
  }

  onSubmit(data) {
    this.submitted = true;
    if (this.addDishCategoryForm.invalid) {
      return;
    }

    this.facade.addDishCategory(data.name, this.afterCategoryId)
      .subscribe(id => {
        this.addDishCategoryForm.reset();
        this.activeModal.close(id);
      });
  }

}
