import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {RouterModule} from '@angular/router';

import {HttpErrorHandlingService} from './services/http-error-handling.service';
import {ClientPaginationComponent} from './components/pagination/client-pagination.component';
import {ServerPaginationComponent} from './components/pagination/server-pagination.component';
import {ScrollableNavBarComponent} from "./components/scrollable-nav-bar/scrollable-nav-bar.component";

@NgModule({
  imports: [
    CommonModule,
    RouterModule
  ],
  declarations: [
    ClientPaginationComponent,
    ServerPaginationComponent,
    ScrollableNavBarComponent
  ],
  providers: [
    HttpErrorHandlingService
  ],
  exports: [
    ClientPaginationComponent,
    ServerPaginationComponent,
    ScrollableNavBarComponent
  ]
})
export class SharedModule {
}
