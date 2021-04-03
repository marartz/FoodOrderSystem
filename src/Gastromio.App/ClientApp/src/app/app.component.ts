import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, NavigationEnd, Router} from '@angular/router';
import {Title} from '@angular/platform-browser';
import AOS from 'aos'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {

  constructor(
    private activatedRoute: ActivatedRoute,
    private titleService: Title,
    private router: Router
  ) {
  }

  ngOnInit() {
    
    AOS.init();

    this.router.events.subscribe((evt) => {
      if (!(evt instanceof NavigationEnd)) {
        return;
      }
      this.titleService.setTitle('Gastromio – Essen lokal bestellen');
      window.scrollTo(0, 0);
    });
  }
}