import {Component, OnDestroy, OnInit} from '@angular/core';

import {BehaviorSubject, Subscription} from "rxjs";

import {RestaurantAdminFacade} from "../../restaurant-admin.facade";
import {OpeningPeriodModel} from "../../../shared/models/restaurant.model";
import {debounceTime, take} from "rxjs/operators";

@Component({
  selector: 'app-opening-hours-settings',
  templateUrl: './opening-hours-settings.component.html',
  styleUrls: [
    './opening-hours-settings.component.css',
    '../../../../assets/css/frontend_v2.min.css',
    '../../../../assets/css/backend_v2.min.css'
  ]
})
export class OpeningHoursSettingsComponent implements OnInit, OnDestroy {

  public regularOpeningHoursViewModel$: BehaviorSubject<RegularOpeningHoursViewModel>;

  private subscriptions: Array<Subscription>;

  constructor(
    private facade: RestaurantAdminFacade
  ) {
    const regularOpeningHoursViewModel = OpeningHoursSettingsComponent.createRegularOpeningHoursViewModel();
    this.regularOpeningHoursViewModel$ = new BehaviorSubject<RegularOpeningHoursViewModel>(regularOpeningHoursViewModel);
  }

  public ngOnInit(): void {
    this.facade.getRestaurant$()
      .subscribe(restaurant => {
        if (restaurant === undefined)
          return;

        this.subscriptions = new Array<Subscription>();

        const regularOpeningHoursViewModel = OpeningHoursSettingsComponent.createRegularOpeningHoursViewModel();

        if (restaurant.regularOpeningDays) {
          for (let regularOpeningDay of restaurant.regularOpeningDays) {
            for (let openingPeriod of regularOpeningDay.openingPeriods) {
              let column = regularOpeningHoursViewModel.weekDays[regularOpeningDay.dayOfWeek].openingPeriods.length

              const openingPeriodViewModel = new OpeningPeriodViewModel();
              openingPeriodViewModel.column = column;
              openingPeriodViewModel.baseModel = openingPeriod;
              openingPeriodViewModel.start = OpeningHoursSettingsComponent.totalMinutesToString(openingPeriod.start);
              openingPeriodViewModel.end = OpeningHoursSettingsComponent.totalMinutesToString(openingPeriod.end);

              const openingPeriodViewModel$ = new BehaviorSubject<OpeningPeriodViewModel>(openingPeriodViewModel);
              this.addSubscriptionForRegular(regularOpeningDay.dayOfWeek, openingPeriodViewModel$);

              regularOpeningHoursViewModel.weekDays[regularOpeningDay.dayOfWeek].openingPeriods.push(openingPeriodViewModel$);
            }
          }

          for (let weekDayOpeningPeriods of regularOpeningHoursViewModel.weekDays) {
            weekDayOpeningPeriods.openingPeriods.sort((a, b) => {
              if (a.value.start < b.value.start)
                return -1;
              else if (a.value.start > b.value.start)
                return +1;
              else
                return 0;
            })
          }

          let maxOpeningPeriodCount = 0;
          for (let i = 0; i < regularOpeningHoursViewModel.weekDays.length; i++) {
            if (regularOpeningHoursViewModel.weekDays[i].openingPeriods.length > maxOpeningPeriodCount) {
              maxOpeningPeriodCount = regularOpeningHoursViewModel.weekDays[i].openingPeriods.length;
            }
          }

          for (let i = 0; i < regularOpeningHoursViewModel.weekDays.length; i++) {
            while (regularOpeningHoursViewModel.weekDays[i].openingPeriods.length < maxOpeningPeriodCount) {
              const openingPeriod$ = new BehaviorSubject<OpeningPeriodViewModel>(undefined);
              this.addSubscriptionForRegular(regularOpeningHoursViewModel.weekDays[i].dayOfWeek, openingPeriod$);
              regularOpeningHoursViewModel.weekDays[i].openingPeriods.push(openingPeriod$);
            }
          }

          regularOpeningHoursViewModel.columns = new Array<number>();
          for (let i = 0; i < maxOpeningPeriodCount; i++) {
            regularOpeningHoursViewModel.columns.push(i);
          }

          this.regularOpeningHoursViewModel$.next(regularOpeningHoursViewModel);
        }
      });
  }

  public ngOnDestroy(): void {
    this.removeSubscriptions();
  }

  public addToRegular(dayOfWeek: number): void {
    const regularOpeningHoursViewModel = this.regularOpeningHoursViewModel$.value;

    let index = regularOpeningHoursViewModel.weekDays[dayOfWeek].openingPeriods.findIndex(en => en !== undefined && en.value !== undefined && en.value.baseModel === undefined);
    if (index >= 0)
      return;

    index = regularOpeningHoursViewModel.weekDays[dayOfWeek].openingPeriods.findIndex(en => en !== undefined && en.value === undefined);

    const openingPeriodViewModel = new OpeningPeriodViewModel();
    openingPeriodViewModel.baseModel = undefined;
    openingPeriodViewModel.start = undefined;
    openingPeriodViewModel.end = undefined;

    if (index >= 0) {
      openingPeriodViewModel.column = index;
      regularOpeningHoursViewModel.weekDays[dayOfWeek].openingPeriods[index].next(openingPeriodViewModel);
    } else {
      index = regularOpeningHoursViewModel.columns.length;
      openingPeriodViewModel.column = index;
      for (let weekDay of regularOpeningHoursViewModel.weekDays) {
        const openingPeriod$ = new BehaviorSubject<OpeningPeriodViewModel>(undefined);
        this.addSubscriptionForRegular(dayOfWeek, openingPeriod$);
        weekDay.openingPeriods.push(openingPeriod$);
      }
      regularOpeningHoursViewModel.columns.push(index);
      regularOpeningHoursViewModel.weekDays[dayOfWeek].openingPeriods[index].next(openingPeriodViewModel);
    }

    this.regularOpeningHoursViewModel$.next(regularOpeningHoursViewModel);
  }

  public changeStartOfRegular(openingPeriod$: BehaviorSubject<OpeningPeriodViewModel>, newValue: string): void {
    openingPeriod$.value.start = newValue;
    openingPeriod$.next(openingPeriod$.value);
  }

  public changeEndOfRegular(openingPeriod$: BehaviorSubject<OpeningPeriodViewModel>, newValue: string): void {
    openingPeriod$.value.end = newValue;
    openingPeriod$.next(openingPeriod$.value);
  }

  public removeFromRegular(dayOfWeek: number, column: number): void {
    const regularOpeningHoursViewModel = this.regularOpeningHoursViewModel$.value;
    const openingPeriod$ = regularOpeningHoursViewModel.weekDays[dayOfWeek].openingPeriods[column];
    if (openingPeriod$.value === undefined)
      return;

    if (openingPeriod$.value.baseModel === undefined) {
      regularOpeningHoursViewModel.weekDays[dayOfWeek].openingPeriods[column].next(undefined);

      const allUndefined = regularOpeningHoursViewModel.weekDays.every(en => en.openingPeriods[regularOpeningHoursViewModel.columns.length - 1].value === undefined);
      if (allUndefined) {
        for (let weekDay of regularOpeningHoursViewModel.weekDays) {
          weekDay.openingPeriods.splice(regularOpeningHoursViewModel.columns.length - 1, 1);
        }
        regularOpeningHoursViewModel.columns.splice(regularOpeningHoursViewModel.columns.length - 1, 1);
      }

      this.regularOpeningHoursViewModel$.next(regularOpeningHoursViewModel);
    } else {
      this.facade.removeRegularOpeningPeriod(dayOfWeek, openingPeriod$.value.baseModel.start)
        .pipe(take(1))
        .subscribe(() => { }, () => { openingPeriod$.value.failure = true; });
    }
  }

  private removeSubscriptions(): void {
    if (this.subscriptions === undefined)
      return;

    for (let subscription of this.subscriptions) {
      subscription.unsubscribe();
    }

    this.subscriptions = new Array<Subscription>();
  }

  private addSubscriptionForRegular(dayOfWeek: number, openingPeriod$: BehaviorSubject<OpeningPeriodViewModel>): void {
    const subscription = openingPeriod$
      .pipe(
        debounceTime(1000)
      )
      .subscribe(openingPeriod => {
        if (openingPeriod === undefined)
          return;

        const startParseResult = OpeningHoursSettingsComponent.parseTimeValue(openingPeriod.start);
        const endParseResult = OpeningHoursSettingsComponent.parseTimeValue(openingPeriod.end);

        if (!startParseResult.isValid || !endParseResult.isValid) {
          return;
        }

        if (openingPeriod.baseModel === undefined) {
          this.facade.addRegularOpeningPeriod(dayOfWeek, startParseResult.value, endParseResult.value)
            .pipe(take(1))
            .subscribe(() => { }, () => { openingPeriod$.value.failure = true; });
        } else if (openingPeriod.baseModel.start !== startParseResult.value || openingPeriod.baseModel.end !== endParseResult.value) {
          this.facade.changeRegularOpeningPeriod(dayOfWeek, openingPeriod.baseModel.start, startParseResult.value, endParseResult.value)
            .pipe(take(1))
            .subscribe(() => { }, () => { openingPeriod$.value.failure = true; });
        }
      });

    this.subscriptions.push(subscription);
  }

  private static createRegularOpeningHoursViewModel(): RegularOpeningHoursViewModel {
    const regularOpeningHoursViewModel = new RegularOpeningHoursViewModel();

    regularOpeningHoursViewModel.weekDays = new Array<RegularOpeningDayViewModel>(7);
    regularOpeningHoursViewModel.weekDays[0] = OpeningHoursSettingsComponent.createWeekDayOpeningPeriod(0, "Montag");
    regularOpeningHoursViewModel.weekDays[1] = OpeningHoursSettingsComponent.createWeekDayOpeningPeriod(1, "Dienstag");
    regularOpeningHoursViewModel.weekDays[2] = OpeningHoursSettingsComponent.createWeekDayOpeningPeriod(2, "Mittwoch");
    regularOpeningHoursViewModel.weekDays[3] = OpeningHoursSettingsComponent.createWeekDayOpeningPeriod(3, "Donnerstag");
    regularOpeningHoursViewModel.weekDays[4] = OpeningHoursSettingsComponent.createWeekDayOpeningPeriod(4, "Freitag");
    regularOpeningHoursViewModel.weekDays[5] = OpeningHoursSettingsComponent.createWeekDayOpeningPeriod(5, "Samstag");
    regularOpeningHoursViewModel.weekDays[6] = OpeningHoursSettingsComponent.createWeekDayOpeningPeriod(6, "Sonntag");

    return regularOpeningHoursViewModel;
  }

  private static createWeekDayOpeningPeriod(dayOfWeek: number, dayOfWeekText: string): RegularOpeningDayViewModel {
    const weekDayOpeningPeriods = new RegularOpeningDayViewModel();
    weekDayOpeningPeriods.dayOfWeek = dayOfWeek;
    weekDayOpeningPeriods.dayOfWeekText = dayOfWeekText;
    weekDayOpeningPeriods.openingPeriods = new Array<BehaviorSubject<OpeningPeriodViewModel>>();
    return weekDayOpeningPeriods;
  }

  private static totalMinutesToString(totalMinutes: number): string {
    const hours = Math.floor(totalMinutes / 60);
    const minutes = Math.floor(totalMinutes % 60);
    return hours.toString().padStart(2, '0') + ':' + minutes.toString().padStart(2, '0');
  }

  private static parseTimeValue(text: string): TimeParseResult {
    if (!text) {
      return new TimeParseResult(false, 0);
    }

    text = text.trim();

    if (text.length < 5) {
      return new TimeParseResult(false, 0);
    }

    text = text.substr(0, 5);

    if (text.substr(2, 1) !== ':') {
      return new TimeParseResult(false, 0);
    }

    const hoursText = text.substr(0, 2);
    const hours = Number(hoursText);
    if (hours === Number.NaN) {
      return new TimeParseResult(false, 0);
    }
    if (hours < 0 || hours > 23) {
      return new TimeParseResult(false, 0);
    }

    const minutesText = text.substr(3, 2);
    const minutes = Number(minutesText);
    if (minutes === Number.NaN) {
      return new TimeParseResult(false, 0);
    }
    if (minutes < 0 || minutes > 59) {
      return new TimeParseResult(false, 0);
    }

    return new TimeParseResult(true, hours * 60 + minutes);
  }

}

export class RegularOpeningHoursViewModel {

  public columns: Array<number>;
  public weekDays: Array<RegularOpeningDayViewModel>;

}

export class RegularOpeningDayViewModel {

  public dayOfWeek: number;
  public dayOfWeekText: string;
  public openingPeriods: Array<BehaviorSubject<OpeningPeriodViewModel>>;

}

export class DeviatingOpeningHoursViewModel {

  public columns: Array<number>;

}

export class DeviatingOpeningDayViewModel {

  public date: Date;
  public openingPeriods: Array<BehaviorSubject<OpeningPeriodViewModel>>

}

export class OpeningPeriodViewModel {

  public column: number;
  public baseModel: OpeningPeriodModel;
  public start: string;
  public end: string;
  public failure: boolean = false;

}

class TimeParseResult {

  constructor(isValid: boolean, value: number) {
    this.isValid = isValid;
    this.value = value;
  }

  isValid: boolean;
  value: number;

}
