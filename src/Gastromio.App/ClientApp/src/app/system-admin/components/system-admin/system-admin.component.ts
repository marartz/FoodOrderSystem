import {Component, OnInit} from "@angular/core";
import {ActivatedRoute} from "@angular/router";

import {delay, switchMap, tap} from "rxjs/operators";
import {merge, Observable, of} from "rxjs";

import {BlockUI, NgBlockUI} from "ng-block-ui";

import {SystemAdminFacade} from "../../system-admin.facade";
import {LinkInfo} from "../../../shared/components/scrollable-nav-bar/scrollable-nav-bar.component";

@Component({
  selector: 'app-system-admin',
  templateUrl: './system-admin.component.html',
  styleUrls: [
    './system-admin.component.css',
    '../../../../assets/css/frontend_v3.min.css',
    '../../../../assets/css/backend_v2.min.css'
  ]
})
export class SystemAdminComponent implements OnInit {

  @BlockUI() blockUI: NgBlockUI;

  public isInitialized$: Observable<boolean>;
  public initializationError$: Observable<string>;
  public selectedTab$: Observable<string>;
  public isUpdated$: Observable<boolean>;
  public updateError$: Observable<string>;

  public links: Array<LinkInfo> = [
    { id: 'users', name: 'Benutzer' },
    { id: 'cuisines', name: 'Cuisines' },
    { id: 'restaurants', name: 'Restaurants' },
    { id: 'restaurant-import', name: 'Restaurantimport' },
    { id: 'dish-import', name: 'Speisenimport' },
  ];

  constructor(
    private route: ActivatedRoute,
    private facade: SystemAdminFacade
  ) {
  }

  ngOnInit() {
    this.facade.getIsInitializing$().subscribe(isInitializing => {
      if (isInitializing) {
        this.blockUI.start('Lade Restaurantdaten...');
      } else {
        this.blockUI.stop();
      }
    });

    this.facade.getIsUpdating$().subscribe(isUpdating => {
      if (isUpdating) {
        this.blockUI.start('Speichere Restaurantdaten...');
      } else {
        this.blockUI.stop();
      }
    });

    this.facade.getIsSearchingFor$().subscribe(isSearchingFor => {
      if (isSearchingFor) {
        this.blockUI.start('Lade ' + isSearchingFor + '...');
      } else {
        this.blockUI.stop();
      }
    });

    this.isInitialized$ = this.facade.getIsInitialized$();

    this.initializationError$ = this.facade.getInitializationError$();

    this.selectedTab$ = this.facade.getSelectedTab$();

    this.isUpdated$ = this.facade.getIsUpdated$()
      .pipe(
        switchMap(isUpdated => {
          if (isUpdated) {
            return merge(
              of(true),
              of(false)
                .pipe(
                  delay(2000),
                  tap(() => {
                    this.facade.ackIsUpdated();
                  })
                )
            );
          } else {
            return of(false);
          }
        })
      );

    this.updateError$ = this.facade.getUpdateError$();

    this.route.data.subscribe(data => {
      const tab = data['tab'];
      this.facade.initialize(tab);
    });
  }

  selectTab(tab: string): void {
    console.log('selectTab: ', tab);
    this.facade.selectTab(tab);
  }

}
