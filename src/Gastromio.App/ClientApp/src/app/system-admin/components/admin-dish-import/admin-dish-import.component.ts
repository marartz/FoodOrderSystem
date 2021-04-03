import {Component, OnInit} from '@angular/core';

import {ImportLogLineModel} from '../../models/import-log-line.model';

import {SystemAdminFacade} from "../../system-admin.facade";

@Component({
  selector: 'app-admin-dish-import',
  templateUrl: './admin-dish-import.component.html',
  styleUrls: ['./admin-dish-import.component.css', '../../../../assets/css/frontend_v3.min.css', '../../../../assets/css/backend_v2.min.css']
})
export class AdminDishImportComponent implements OnInit {

  dishImportFile: File;
  logLines: Array<ImportLogLineModel>;

  constructor(
    private facade: SystemAdminFacade
  ) {
  }

  ngOnInit() {
  }

  handleDishImportFileInput(target: any): void {
    const files = target.files;
    this.dishImportFile = files.item(0);
  }

  onSimulateDishes(): void {
    this.logLines = undefined;
    this.facade.importDishes$(this.dishImportFile, true)
      .subscribe(log => {
        if (!log)
          return;
        this.logLines = log.lines;
      }, () => {
        this.logLines = undefined;
      });
  }

  onImportDishes(): void {
    this.logLines = undefined;
    this.facade.importDishes$(this.dishImportFile, false)
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
