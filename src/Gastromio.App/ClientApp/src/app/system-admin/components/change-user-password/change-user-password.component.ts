import {Component, OnInit, Input} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';

import {Observable} from "rxjs";

import {NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';

import {BlockUI, NgBlockUI} from 'ng-block-ui';

import {UserModel} from '../../../shared/models/user.model';

import {ConfirmPasswordValidator} from '../../../auth/validators/password.validator';

import {SystemAdminFacade} from "../../system-admin.facade";

@Component({
  selector: 'app-change-user-password',
  templateUrl: './change-user-password.component.html',
  styleUrls: [
    './change-user-password.component.css',
    '../../../../assets/css/frontend_v3.min.css',
    '../../../../assets/css/modals.component.min.css'
  ]
})
export class ChangeUserPasswordComponent implements OnInit {

  @Input() public user: UserModel;
  @BlockUI() blockUI: NgBlockUI;

  changeUserPasswordForm: FormGroup;
  message$: Observable<string>;
  submitted = false;

  constructor(
    public activeModal: NgbActiveModal,
    private formBuilder: FormBuilder,
    private facade: SystemAdminFacade
  ) {
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

    this.changeUserPasswordForm = this.formBuilder.group({
      password: ['', [Validators.required, Validators.pattern('(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&]).{6,}')]],
      passwordRepeat: ['']
    }, {validators: ConfirmPasswordValidator('password', 'passwordRepeat')});
  }

  get f() {
    return this.changeUserPasswordForm.controls;
  }

  onSubmit(data) {
    this.submitted = true;
    if (this.changeUserPasswordForm.invalid) {
      return;
    }

    this.facade.changeUserPassword$(this.user.id, data.password)
      .subscribe(() => {
        this.activeModal.close('Close click');
      });
  }

}
