import {Component, OnInit} from '@angular/core';

import {Observable} from 'rxjs';

import {NgbModal} from '@ng-bootstrap/ng-bootstrap';

import {CuisineModel} from '../../../shared/models/cuisine.model';

import {AddCuisineComponent} from '../add-cuisine/add-cuisine.component';
import {ChangeCuisineComponent} from '../change-cuisine/change-cuisine.component';
import {RemoveCuisineComponent} from '../remove-cuisine/remove-cuisine.component';
import {SystemAdminFacade} from "../../system-admin.facade";

@Component({
  selector: 'app-admin-cuisines',
  templateUrl: './admin-cuisines.component.html',
  styleUrls: [
    './admin-cuisines.component.css',
    '../../../../assets/css/frontend_v3.min.css',
    '../../../../assets/css/backend_v2.min.css'
  ]
})
export class AdminCuisinesComponent implements OnInit {

  cuisines$: Observable<CuisineModel[]>;

  constructor(
    private modalService: NgbModal,
    private facade: SystemAdminFacade
  ) {
  }

  ngOnInit() {
    this.cuisines$ = this.facade.getCuisines$();
  }

  openAddCuisineForm(): void {
    this.modalService.open(AddCuisineComponent);
  }

  openChangeCuisineForm(cuisine: CuisineModel): void {
    const modalRef = this.modalService.open(ChangeCuisineComponent);
    modalRef.componentInstance.cuisine = cuisine;
  }

  openRemoveCuisineForm(cuisine: CuisineModel): void {
    const modalRef = this.modalService.open(RemoveCuisineComponent);
    modalRef.componentInstance.cuisine = cuisine;
  }

}
