import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';

import {Observable} from "rxjs";

import {NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';

import {BlockUI, NgBlockUI} from 'ng-block-ui';

import {ConfirmPasswordValidator} from '../../../auth/validators/password.validator';

import {SystemAdminFacade} from "../../system-admin.facade";

@Component({
  selector: 'app-add-user',
  templateUrl: './add-user.component.html',
  styleUrls: [
    './add-user.component.css',
    '../../../../assets/css/frontend_v3.min.css',
    '../../../../assets/css/modals.component.min.css'
  ]
})
export class AddUserComponent implements OnInit {
  @BlockUI() blockUI: NgBlockUI;

  addUserForm: FormGroup;
  message$: Observable<string>;
  submitted = false;

  constructor(
    public activeModal: NgbActiveModal,
    private formBuilder: FormBuilder,
    private facade: SystemAdminFacade
  ) {
    this.addUserForm = this.formBuilder.group({
      role: ['', Validators.required],
      email: ['', [Validators.required, Validators.email, Validators.pattern('^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$')]],
      password: ['', [Validators.required, Validators.pattern('(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&]).{6,}')]],
      passwordRepeat: ['']
    }, {validators: ConfirmPasswordValidator('password', 'passwordRepeat')});
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
    return this.addUserForm.controls;
  }

  onSubmit(data) {
    this.submitted = true;
    if (this.addUserForm.invalid) {
      return;
    }

    this.facade.addUser$(data.role, data.email, data.password)
      .subscribe(() => {
        this.activeModal.close('Close click');
      });
  }
}
