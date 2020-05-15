import { Component, HostListener, ViewChild, ElementRef } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { OrderHomeComponent } from './order-home/order-home.component';
import { Title } from '@angular/platform-browser';
import { ControlContainer } from '@angular/forms';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  routerOutlet: RouterOutlet;
  showBottomBar = true;

  currentDivId: string;

  constructor(private element: ElementRef, private titleService: Title) {
    titleService.setTitle('Gastromio.de - Einfach, Bestellen, Unterstützen');
  }

  onActivate(component: any): void {
    console.log('component: ', event);
    this.showBottomBar = !(component instanceof OrderHomeComponent);
  }
}
