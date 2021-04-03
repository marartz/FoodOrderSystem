import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';

import {Observable} from "rxjs";

import {NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';

import {BlockUI, NgBlockUI} from 'ng-block-ui';

import {SystemAdminFacade} from "../../system-admin.facade";

@Component({
  selector: 'app-add-restaurant',
  templateUrl: './add-restaurant.component.html',
  styleUrls: [
    './add-restaurant.component.css',
    '../../../../assets/css/frontend_v3.min.css',
    '../../../../assets/css/modals.component.min.css'
  ]
})
export class AddRestaurantComponent implements OnInit {
  @BlockUI() blockUI: NgBlockUI;

  addRestaurantForm: FormGroup;
  message$: Observable<string>;
  submitted = false;

  constructor(
    public activeModal: NgbActiveModal,
    private formBuilder: FormBuilder,
    private facade: SystemAdminFacade
  ) {
    this.addRestaurantForm = this.formBuilder.group({
      name: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.facade.getIsUpdating$()
      .subscribe(isUpdating => {
        if (isUpdating) {
          this.blockUI.start('Verarbeite Daten...');
        } else {
          this.blockUI.stop();
        }
      });

    this.message$ = this.facade.getUpdateError$();
  }

  get f() {
    return this.addRestaurantForm.controls;
  }

  onSubmit(data) {
    this.submitted = true;
    if (this.addRestaurantForm.invalid) {
      return;
    }

    this.facade.addRestaurant$(data.name)
      .subscribe(() => {
        this.activeModal.close('Close click');
      });
  }
}
