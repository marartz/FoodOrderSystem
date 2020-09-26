import {Component, Input, OnInit} from '@angular/core';
import {NgbActiveModal, NgbCalendar, NgbDateStruct, NgbTimeStruct} from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-opening-hour-filter',
  templateUrl: './opening-hour-filter.component.html',
  styleUrls: [
    './opening-hour-filter.component.css',
    '../../assets/css/backend_v2.min.css',
    '../../assets/css/frontend_v2.min.css',
    '../../assets/css/animations_v2.min.css'
  ]
})
export class OpeningHourFilterComponent implements OnInit {

  @Input() public value: Date;

  minDate: NgbDateStruct;
  date: NgbDateStruct;
  time: NgbTimeStruct;

  constructor(
    public activeModal: NgbActiveModal,
    private calendar: NgbCalendar
  ) {
  }

  ngOnInit() {
    const now = new Date();

    this.minDate = {year: now.getFullYear(), month: now.getMonth() + 1, day: now.getDate()};

    if (this.value !== undefined) {
      if (this.value < now) {
        this.value = now;
      }
      this.date = {year: this.value.getFullYear(), month: this.value.getMonth() + 1, day: this.value.getDate()};
      this.time = {hour: this.value.getHours(), minute: this.value.getMinutes(), second: 0};
    } else {
      this.date = this.calendar.getToday();
      this.time = {hour: now.getHours(), minute: now.getMinutes(), second: 0};
    }
  }

  onTimeChanged(): void {
    const now = new Date();
    let date = this.calculateDate();

    if (date < now) {
      this.time = {hour: now.getHours(), minute: now.getMinutes(), second: 0};
    }
  }

  onClose(): void {
    const now = new Date();
    let date = this.calculateDate();

    if (date < now) {
      date = now;
    }

    this.activeModal.close(date);
  }

  private calculateDate(): Date {
    return new Date(
      this.date.year,
      this.date.month - 1,
      this.date.day,
      this.time.hour,
      this.time.minute,
      0,
      0
    );
  }

}
