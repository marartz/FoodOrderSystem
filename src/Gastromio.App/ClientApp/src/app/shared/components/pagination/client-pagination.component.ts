import {Component, Input, Output, EventEmitter, OnInit, OnChanges, SimpleChanges} from '@angular/core';

@Component({
  selector: 'app-client-pagination',
  template: `
    <ul *ngIf='pager.pages && pager.pages.length' class='pagination justify-content-center'>
      <li [ngClass]='{disabled:pager.currentPage === 1}' class='page-item first-item'>
        <a (click)='setPage(1)' class='page-link' [routerLink]=''><i class='fas fa-angle-double-left'></i></a>
      </li>
      <li [ngClass]='{disabled:pager.currentPage === 1}' class='page-item previous-item'>
        <a (click)='setPage(pager.currentPage - 1)' class='page-link' [routerLink]=''><i class='fas fa-angle-left'></i></a>
      </li>
      <li *ngFor='let page of pager.pages' [ngClass]='{active:pager.currentPage === page}'
          class='page-item number-item'>
        <a (click)='setPage(page)' class='page-link' [routerLink]=''>{{page}}</a>
      </li>
      <li [ngClass]='{disabled:pager.currentPage === pager.totalPages}' class='page-item next-item'>
        <a (click)='setPage(pager.currentPage + 1)' class='page-link' [routerLink]=''><i class='fas fa-angle-right'></i></a>
      </li>
      <li [ngClass]='{disabled:pager.currentPage === pager.totalPages}' class='page-item last-item'>
        <a (click)='setPage(pager.totalPages)' class='page-link' [routerLink]=''><i
          class='fas fa-angle-double-right'></i></a>
      </li>
    </ul>`
})

export class ClientPaginationComponent implements OnInit, OnChanges {
  @Input() items: Array<any>;
  @Output() changePage = new EventEmitter<any>(true);
  @Input() initialPage = 1;
  @Input() pageSize = 10;
  @Input() maxPages = 5;

  pager: PagingInfo = {
    totalItems: 0,
    currentPage: 0,
    pageSize: 0,
    totalPages: 0,
    startPage: 0,
    endPage: 0,
    startIndex: 0,
    endIndex: 0,
    pages: []
  };

  ngOnInit() {
    // set page if items array isn't empty
    if (this.items && this.items.length) {
      this.setPage(this.initialPage);
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    // reset page if items array has changed
    if (changes.items.currentValue !== changes.items.previousValue) {
      this.setPage(this.initialPage);
    }
  }

  setPage(page: number) {
    // get new pager object for specified page
    this.pager = this.paginate(this.items.length, page, this.pageSize, this.maxPages);

    // get new page of items from items array
    const pageOfItems = this.items.slice(this.pager.startIndex, this.pager.endIndex + 1);

    // call change page function in parent component
    this.changePage.emit(pageOfItems);
  }

  private paginate(
    totalItems: number,
    currentPage: number = 1,
    pageSize: number = 10,
    maxPages: number = 10
  ): PagingInfo {
    // calculate total pages
    const totalPages = Math.ceil(totalItems / pageSize);

    // ensure current page isn't out of range
    if (currentPage < 1) {
      currentPage = 1;
    } else if (currentPage > totalPages) {
      currentPage = totalPages;
    }

    // tslint:disable-next-line:one-variable-per-declaration
    let startPage: number, endPage: number;
    if (totalPages <= maxPages) {
      // total pages less than max so show all pages
      startPage = 1;
      endPage = totalPages;
    } else {
      // total pages more than max so calculate start and end pages
      const maxPagesBeforeCurrentPage = Math.floor(maxPages / 2);
      const maxPagesAfterCurrentPage = Math.ceil(maxPages / 2) - 1;
      if (currentPage <= maxPagesBeforeCurrentPage) {
        // current page near the start
        startPage = 1;
        endPage = maxPages;
      } else if (currentPage + maxPagesAfterCurrentPage >= totalPages) {
        // current page near the end
        startPage = totalPages - maxPages + 1;
        endPage = totalPages;
      } else {
        // current page somewhere in the middle
        startPage = currentPage - maxPagesBeforeCurrentPage;
        endPage = currentPage + maxPagesAfterCurrentPage;
      }
    }

    // calculate start and end item indexes
    const startIndex = (currentPage - 1) * pageSize;
    const endIndex = Math.min(startIndex + pageSize - 1, totalItems - 1);

    // create an array of pages to ng-repeat in the pager control
    const pages = Array.from(Array((endPage + 1) - startPage).keys()).map(i => startPage + i);

    // return object with all pager properties required by the view
    return {
      totalItems,
      currentPage,
      pageSize,
      totalPages,
      startPage,
      endPage,
      startIndex,
      endIndex,
      pages
    };
  }
}

export interface PagingInfo {
  totalItems: number;
  currentPage: number;
  pageSize: number;
  totalPages: number;
  startPage: number;
  endPage: number;
  startIndex: number;
  endIndex: number;
  pages: number[];
}
