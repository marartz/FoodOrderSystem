import {Component, OnInit} from '@angular/core';

import {ImportLogLineModel} from '../../models/import-log-line.model';

import {SystemAdminFacade} from "../../system-admin.facade";

@Component({
  selector: 'app-admin-restaurant-import',
  templateUrl: './admin-restaurant-import.component.html',
  styleUrls: ['./admin-restaurant-import.component.css', '../../../../assets/css/frontend_v3.min.css', '../../../../assets/css/backend_v2.min.css']
})
export class AdminRestaurantImportComponent implements OnInit {

  restaurantImportFile: File;
  logLines: Array<ImportLogLineModel>;

  constructor(
    private facade: SystemAdminFacade
  ) {
  }

  ngOnInit() {
  }

  handleRestaurantImportFileInput(target: any): void {
    const files = target.files;
    this.restaurantImportFile = files.item(0);
  }

  onSimulateRestaurants(): void {
    this.logLines = undefined;
    this.facade.importRestaurants$(this.restaurantImportFile, true)
      .subscribe(log => {
        if (!log)
          return;
        this.logLines = log.lines;
      }, () => {
        this.logLines = undefined;
      });
  }

  onImportRestaurants(): void {
    this.logLines = undefined;
    this.facade.importRestaurants$(this.restaurantImportFile, false)
      .subscribe(log => {
        if (!log)
          return;
        this.logLines = log.lines;
      }, () => {
        this.logLines = undefined;
      });
  }

  getColorForLogLineType(type: number): string {
    switch (type)
    {
      case 0:
        return 'green';
      case 1:
        return 'yellow';
      case 2:
        return 'red';
      default:
        return '';
    }
  }

}
