import { Component, Input, Output, EventEmitter} from '@angular/core';

@Component({
  selector: 'app-server-pagination',
  template: `
    <ul *ngIf='pager.pages && pager.pages.length' class='pagination justify-content-center'>
      <li [ngClass]='{disabled:pager.currentPage === 1}' class='page-item first-item'>
        <a (click)='triggerFetchPage(1)' class='page-link' [routerLink]=''><i class='fas fa-angle-double-left'></i></a>
      </li>
      <li [ngClass]='{disabled:pager.currentPage === 1}' class='page-item previous-item'>
        <a (click)='triggerFetchPage(pager.currentPage - 1)' class='page-link' [routerLink]=''><i class='fas fa-angle-left'></i></a>
      </li>
      <li *ngFor='let page of pager.pages' [ngClass]='{active:pager.currentPage === page}'
          class='page-item number-item'>
        <a (click)='triggerFetchPage(page)' class='page-link' [routerLink]=''>{{page}}</a>
      </li>
      <li [ngClass]='{disabled:pager.currentPage === pager.totalPages}' class='page-item next-item'>
        <a (click)='triggerFetchPage(pager.currentPage + 1)' class='page-link' [routerLink]=''><i class='fas fa-angle-right'></i></a>
      </li>
      <li [ngClass]='{disabled:pager.currentPage === pager.totalPages}' class='page-item last-item'>
        <a (click)='triggerFetchPage(pager.totalPages)' class='page-link' [routerLink]=''><i
          class='fas fa-angle-double-right'></i></a>
      </li>
    </ul>`
})

export class ServerPaginationComponent {
  @Output() private fetchPage = new EventEmitter<FetchPageInfo>(true);
  @Input() private _pageSize = 10; // fetch default: 'take = 10'
  @Input() private _maxPages = 5;

  private readonly emptyPager: PagingInfo = {
    totalItems: 0,
    currentPage: 0,
    totalPages: 0,
    startPage: 0,
    endPage: 0,
    startIndex: 0, // fetch default: 'skip = 0'
    endIndex: 0,
    pages: []
  };

  pager: PagingInfo = this.emptyPager;

  triggerFetchPage(page: number = this.pager.currentPage) {
    // update startIndex if page has changed
    if (page !== this.pager.currentPage) {
      this.pager = this.paginate(this.pager.totalItems, page, this._pageSize, this._maxPages);
    }

    // trigger page fetch in parent component
    return this.fetchPage.emit({
      skip: this.pager.startIndex,
      take: this._pageSize
    });
  }

  updatePaging(totalItems: number, pageItems: number) {
    // fallback to first page when current page is empty (eg. if last item on page was deleted)
    if (!pageItems && totalItems) {
      this.triggerFetchPage(1);
    }
    else {
      this.pager = this.paginate(totalItems);
    }
  }

  private paginate(
    totalItems: number,
    currentPage: number = this.pager.currentPage,
    pageSize: number = this._pageSize,
    maxPages: number = this._maxPages
  ): PagingInfo {
    // return empty pager to skip rendering empty pagination component
    if (!totalItems || !pageSize || !maxPages) {
      return this.emptyPager;
    }

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
      totalPages,
      startPage,
      endPage,
      startIndex,
      endIndex,
      pages
    };
  }
}

export interface FetchPageInfo {
  skip: number;
  take: number;
}

export interface PagingInfo {
  totalItems: number;
  currentPage: number;
  totalPages: number;
  startPage: number;
  endPage: number;
  startIndex: number;
  endIndex: number;
  pages: number[];
}
