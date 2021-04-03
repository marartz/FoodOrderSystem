import {Component, OnInit, Input} from '@angular/core';
import {NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';

import {Observable} from "rxjs";

import {BlockUI, NgBlockUI} from 'ng-block-ui';

import {UserModel} from '../../../shared/models/user.model';

import {SystemAdminFacade} from "../../system-admin.facade";

@Component({
  selector: 'app-remove-user',
  templateUrl: './remove-user.component.html',
  styleUrls: [
    './remove-user.component.css',
    '../../../../assets/css/frontend_v3.min.css',
    '../../../../assets/css/modals.component.min.css'
  ]
})
export class RemoveUserComponent implements OnInit {
  @Input() public user: UserModel;
  @BlockUI() blockUI: NgBlockUI;

  message$: Observable<string>;

  constructor(
    public activeModal: NgbActiveModal,
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
  }

  onSubmit() {
    this.facade.removeUser$(this.user.id)
      .subscribe(() => {
        this.activeModal.close('Close click');
      });
  }
}
