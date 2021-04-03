import {Component, OnInit, Input} from '@angular/core';

import {Observable} from "rxjs";

import {NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';

import {BlockUI, NgBlockUI} from 'ng-block-ui';

import {CuisineModel} from '../../../shared/models/cuisine.model';

import {SystemAdminFacade} from "../../system-admin.facade";

@Component({
  selector: 'app-remove-cuisine',
  templateUrl: './remove-cuisine.component.html',
  styleUrls: [
    './remove-cuisine.component.css',
    '../../../../assets/css/frontend_v3.min.css',
    '../../../../assets/css/modals.component.min.css'
  ]
})
export class RemoveCuisineComponent implements OnInit {

  @Input() public cuisine: CuisineModel;
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
    this.facade.removeCuisine$(this.cuisine.id)
      .subscribe(() => {
        this.activeModal.close('Close click');
      });
  }

}
