import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {HttpErrorResponse} from '@angular/common/http';
import {ActivatedRoute} from '@angular/router';

import {concatMap} from 'rxjs/operators';

import {BlockUI, NgBlockUI} from 'ng-block-ui';

import {HttpErrorHandlingService} from '../../../shared/services/http-error-handling.service';

import {AuthService} from '../../../auth/services/auth.service';
import {ConfirmPasswordValidator} from '../../../auth/validators/password.validator';


@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css', '../../../../assets/css/frontend_v3.min.css', '../../../../assets/css/backend_v2.min.css']
})
export class ResetPasswordComponent implements OnInit {
  @BlockUI() blockUI: NgBlockUI;

  userId: string;
  passwordResetCode: string;
  valid: boolean = false;
  success: boolean = false;

  changePasswordForm: FormGroup;
  errorMessage: string;
  submitted = false;

  constructor(
    private route: ActivatedRoute,
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

    this.blockUI.start('Initialisiere...');

    this.route.queryParams
      .pipe(
        concatMap(params => {
          this.userId = params.userId;
          this.passwordResetCode = params.passwordResetCode;
          return this.authService.validatePasswordResetCodeAsync(this.userId, this.passwordResetCode)
        })
      )
      .subscribe(
        () => {
          this.valid = true;
          this.blockUI.stop();
        },
        (response: HttpErrorResponse) => {
          this.valid = false;
          this.errorMessage = this.httpErrorHandlingService.handleError(response).getJoinedGeneralErrors();
          this.blockUI.stop();
        },
      );
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
    this.authService.changePasswordWithResetCodeAsync(this.userId, this.passwordResetCode, data.password)
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
