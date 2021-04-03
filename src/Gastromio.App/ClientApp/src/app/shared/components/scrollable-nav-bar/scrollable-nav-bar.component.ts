import {
  Component,
  Input,
  Output,
  EventEmitter,
  OnDestroy,
  OnInit,
  ViewChild,
  ElementRef,
  SimpleChanges
} from '@angular/core';

@Component({
  selector: 'scrollable-nav-bar',
  templateUrl: './scrollable-nav-bar.component.html',
  styleUrls: ['./scrollable-nav-bar.component.css', '../../../../assets/css/backend_v2.min.css']
})
export class ScrollableNavBarComponent implements OnInit, OnDestroy {

  @Input("links") links;
  @Input("current") current;
  @Output("selected") onSelected = new EventEmitter<string>();

  @ViewChild('left') leftElement: ElementRef;
  @ViewChild('right') rightElement: ElementRef;
  @ViewChild('nav') navElement: ElementRef;

  ngOnInit(): void {
  }

  ngOnChanges(changes: SimpleChanges): void {
    const currentChange = changes['current'];
    if (currentChange && currentChange.previousValue !== currentChange.currentValue) {
      this.selectLink(this.current);
    }
  }

  ngAfterViewInit() {
    this.navElement.nativeElement.addEventListener('scroll', this.onScroll.bind(this));
  }

  ngAfterViewChecked() {
    this.updateScrollControls();
  }

  ngOnDestroy(): void {
  }

  selectLink(id: string): void {
    this.onSelected.emit(id);
  }

  onScroll(evt): void {
    this.updateScrollControls();
  }

  clickedLeft(): void {
    const element = this.navElement.nativeElement;
    let newScrollLeft = element.scrollLeft - element.clientWidth + 20;
    if (newScrollLeft < 0)
      newScrollLeft = 0;
    element.scrollLeft = newScrollLeft;
  }

  clickedRight(): void {
    const element = this.navElement.nativeElement;
    let newScrollLeft = element.scrollLeft + element.clientWidth - 20;
    if (newScrollLeft > element.scrollWidth)
      newScrollLeft = element.scrollWidth;
    element.scrollLeft = newScrollLeft;
  }

  updateScrollControls() {
    const element = this.navElement.nativeElement;

    if (element.scrollWidth !== element.clientWidth) {
      const percent = Math.round((element.scrollLeft / (element.scrollWidth - element.clientWidth)) * 100);
      this.leftElement.nativeElement.className = percent !== 0 ? 'show' : 'hide';
      this.rightElement.nativeElement.className = percent !== 100 ? 'show' : 'hide';
    } else {
      this.leftElement.nativeElement.className = 'hide';
      this.rightElement.nativeElement.className = 'hide';
    }
  }

}


export class LinkInfo {

  constructor(init?: Partial<LinkInfo>) {
    if (init) {
      Object.assign(this, init);
    }
  }

  id: string;
  name: string;

}
