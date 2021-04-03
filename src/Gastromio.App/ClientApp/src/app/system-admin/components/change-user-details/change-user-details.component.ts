import {Component, OnInit, Input} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';

import {Observable} from "rxjs";

import {NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';

import {BlockUI, NgBlockUI} from 'ng-block-ui';

import {UserModel} from '../../../shared/models/user.model';

import {SystemAdminFacade} from "../../system-admin.facade";

@Component({
  selector: 'app-change-user-details',
  templateUrl: './change-user-details.component.html',
  styleUrls: [
    './change-user-details.component.css',
    '../../../../assets/css/frontend_v3.min.css',
    '../../../../assets/css/modals.component.min.css'
  ]
})
export class ChangeUserDetailsComponent implements OnInit {

  @Input() public user: UserModel;
  @BlockUI() blockUI: NgBlockUI;

  changeUserDetailsForm: FormGroup;
  message$: Observable<string>;

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

    this.changeUserDetailsForm = this.formBuilder.group({
      role: [this.user.role, Validators.required],
      email: [this.user.email, [Validators.required, Validators.email, Validators.pattern('^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$')]]
    });
  }

  get f() {
    return this.changeUserDetailsForm.controls;
  }

  onSubmit(data) {
    if (this.changeUserDetailsForm.invalid) {
      return;
    }

    this.facade.changeUserDetails$(this.user.id, data.role, data.email)
      .subscribe(() => {
        this.activeModal.close('Close click');
      });
  }

}
