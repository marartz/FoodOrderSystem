import {Component, OnInit, Input} from '@angular/core';
import {NgbActiveModal} from '@ng-bootstrap/ng-bootstrap';

import {RestaurantModel} from '../../../shared/models/restaurant.model';

@Component({
  selector: 'app-order-restaurant-imprint',
  templateUrl: './order-restaurant-imprint.component.html',
  styleUrls: ['./order-restaurant-imprint.component.css', '../../../../assets/css/frontend_v3.min.css', '../../../../assets/css/modals.component.min.css']
})
export class OrderRestaurantImprintComponent implements OnInit {
  @Input() restaurant: RestaurantModel;

  constructor(
    public activeModal: NgbActiveModal
  ) {
  }

  ngOnInit() {
  }
}
