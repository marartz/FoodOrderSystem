import {Component, OnInit, OnDestroy, ViewChild, AfterViewInit} from '@angular/core';

import {Observable} from 'rxjs';

import {NgbModal} from '@ng-bootstrap/ng-bootstrap';

import {UserModel} from '../../../shared/models/user.model';

import {FetchPageInfo, ServerPaginationComponent} from '../../../shared/components/pagination/server-pagination.component';

import {SystemAdminFacade} from "../../system-admin.facade";

import {AddUserComponent} from '../add-user/add-user.component';
import {ChangeUserDetailsComponent} from '../change-user-details/change-user-details.component';
import {ChangeUserPasswordComponent} from '../change-user-password/change-user-password.component';
import {RemoveUserComponent} from '../remove-user/remove-user.component';

@Component({
  selector: 'app-admin-users',
  templateUrl: './admin-users.component.html',
  styleUrls: [
    './admin-users.component.css',
    '../../../../assets/css/frontend_v3.min.css',
    '../../../../assets/css/backend_v2.min.css'
  ]
})
export class AdminUsersComponent implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild(ServerPaginationComponent) pagingComponent: ServerPaginationComponent;
  pageOfUsers: UserModel[];

  public searchPhrase$: Observable<string>;

  constructor(
    private modalService: NgbModal,
    private facade: SystemAdminFacade
  ) {
  }

  ngOnInit() {
    this.searchPhrase$ = this.facade.getUserSearchPhrase$();
    this.searchPhrase$
      .subscribe(
        () => {
          this.pagingComponent.triggerFetchPage(1);
        }
      );
  }

  ngAfterViewInit() {
    // ViewChild has to be rendered before it can be accessed
    this.pagingComponent.triggerFetchPage(1);
  }

  ngOnDestroy() {
  }

  onUpdateSearch(value: string) {
    this.facade.setUserSearchPhrase(value);
  }

  onFetchPage(info: FetchPageInfo) {
    this.facade.searchForUsers$(info.skip, info.take)
      .subscribe((result) => {
        this.pageOfUsers = result.items;
        this.pagingComponent.updatePaging(result.total, result.items.length);
      }, () => {
      });
  }

  openAddUserForm(): void {
    const modalRef = this.modalService.open(AddUserComponent);
    modalRef.result.then(() => {
      this.pagingComponent.triggerFetchPage();
    }, () => {
    });
  }

  openChangeUserDetailsForm(user: UserModel): void {
    const modalRef = this.modalService.open(ChangeUserDetailsComponent);
    modalRef.componentInstance.user = user;
    modalRef.result.then(() => {
      this.pagingComponent.triggerFetchPage();
    }, () => {
    });
  }

  openChangeUserPasswordForm(user: UserModel): void {
    const modalRef = this.modalService.open(ChangeUserPasswordComponent);
    modalRef.componentInstance.user = user;
  }

  openRemoveUserForm(user: UserModel): void {
    const modalRef = this.modalService.open(RemoveUserComponent);
    modalRef.componentInstance.user = user;
    modalRef.result.then(() => {
      this.pagingComponent.triggerFetchPage();
    }, () => {
    });
  }

  localizeRole(role: string): string {
    switch (role) {
      case 'SystemAdmin':
        return 'Systemadministrator';
      case 'RestaurantAdmin':
        return 'Restaurantadministrator';
      case 'Customer':
        return 'Kunde';
      default:
        return 'Kunde';
    }
  }

}
