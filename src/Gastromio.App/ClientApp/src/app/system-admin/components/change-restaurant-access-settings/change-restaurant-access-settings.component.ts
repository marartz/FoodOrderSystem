import {Component, Input, OnInit} from '@angular/core';
import {FormBuilder} from "@angular/forms";

import {Observable, of} from "rxjs";
import {debounceTime, distinctUntilChanged, map, switchMap} from "rxjs/operators";

import {NgbActiveModal, NgbTypeaheadSelectItemEvent} from "@ng-bootstrap/ng-bootstrap";

import {BlockUI, NgBlockUI} from "ng-block-ui";

import {RestaurantModel} from "../../../shared/models/restaurant.model";
import {UserModel} from "../../../shared/models/user.model";

import {SystemAdminFacade} from "../../system-admin.facade";

@Component({
  selector: 'app-change-restaurant-access-settings',
  templateUrl: './change-restaurant-access-settings.component.html',
  styleUrls: [
    './change-restaurant-access-settings.component.css',
    '../../../../assets/css/frontend_v3.min.css',
    '../../../../assets/css/modals.component.min.css'
  ]
})
export class ChangeRestaurantAccessSettingsComponent implements OnInit {

  @Input() public restaurant: RestaurantModel;
  @BlockUI() blockUI: NgBlockUI;

  administrators: UserModel[];
  message$: Observable<string>;

  constructor(
    public activeModal: NgbActiveModal,
    private formBuilder: FormBuilder,
    private facade: SystemAdminFacade
  ) {
  }

  ngOnInit(): void {
    this.facade.getIsUpdating$()
      .subscribe(isUpdating => {
        if (isUpdating) {
          this.blockUI.start('Verarbeite Daten...');
        } else {
          this.blockUI.stop();
        }
      });

    this.message$ = this.facade.getUpdateError$();

    this.administrators = Object.assign([], this.restaurant.administrators);
    if (this.administrators === undefined) {
      this.administrators = new Array<UserModel>();
    }
  }

  searchForUser = (text: Observable<string>) =>
    text.pipe(
      debounceTime(200),
      distinctUntilChanged(),
      switchMap(term => {
        if (term.length < 2) {
          return of([]);
        } else {
          return this.facade.searchForAdminUsers$(term)
            .pipe(
              map(users => {
                if (users.length > 2) {
                  return [users[0], users[1]];
                } else {
                  return users;
                }
              })
            )
        }
      }),
    )

  formatUser(user: UserModel): string {
    return user.email;
  }

  onUserSelected(event: NgbTypeaheadSelectItemEvent, input: HTMLInputElement): void {
    const user: UserModel = event.item;

    event.preventDefault();
    input.value = '';

    const index = this.administrators.findIndex(en => en.id === user.id);
    if (index < 0) {
      this.administrators.push(user);
      this.sortAdministrators();
    }
  }

  onRemoveUser(user: UserModel): void {
    const index = this.administrators.findIndex(en => en.id === user.id);
    this.administrators.splice(index, 1);
  }

  sortAdministrators(): void {
    this.administrators.sort((a, b) => {
      if (a.email < b.email)
        return -1;
      else if (a.email > b.email)
        return 1;
      return 0;
    });
  }

  onSave(): void {
    this.facade.updateAdministratorsOfRestaurant$(this.restaurant, this.administrators);
  }

}
