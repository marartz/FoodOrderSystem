import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {HttpErrorResponse} from '@angular/common/http';

import {BlockUI, NgBlockUI} from 'ng-block-ui';

import {HttpErrorHandlingService} from '../../../shared/services/http-error-handling.service';

import {AuthService} from '../../../auth/services/auth.service';
import {ConfirmPasswordValidator} from '../../../auth/validators/password.validator';


@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css', '../../../../assets/css/frontend_v2.min.css', '../../../../assets/css/backend_v2.min.css']
})
export class ChangePasswordComponent implements OnInit {
  @BlockUI() blockUI: NgBlockUI;

  success: boolean = false;

  changePasswordForm: FormGroup;
  errorMessage: string;
  submitted = false;

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService,
    private httpErrorHandlingService: HttpErrorHandlingService
  ) {
  }

  ngOnInit() {
    this.changePasswordForm = this.formBuilder.group({
      password: ['', [Validators.required, Validators.pattern('(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&]).{6,}')]],
      passwordRepeat: ['']
    }, {validators: ConfirmPasswordValidator('password', 'passwordRepeat')});
  }

  get f() {
    return this.changePasswordForm.controls;
  }

  onSubmit(data) {
    this.submitted = true;
    if (this.changePasswordForm.invalid) {
      return;
    }

    this.blockUI.start('Ändere Dein Passwort...');
    this.authService.changePasswordAsync(data.password)
      .subscribe(() => {
        this.blockUI.stop();
        this.submitted = false;
        this.errorMessage = undefined;
        this.success = true;
        this.changePasswordForm.reset();
      }, (response: HttpErrorResponse) => {
        this.blockUI.stop();
        this.errorMessage = this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors();
      });
  }
}
