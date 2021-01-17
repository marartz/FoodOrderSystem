import {Injectable} from '@angular/core';
import {Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot} from '@angular/router';
import {AuthService} from './auth.service';

@Injectable()
export class LoginAuthGuardService implements CanActivate {

  constructor(
    public auth: AuthService,
    public router: Router
  ) {
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    const loginUrl = '/login?returnUrl=' + encodeURIComponent(state.url);

    if (!this.auth.isAuthenticated()) {
      this.router.navigateByUrl(loginUrl);
      return false;
    }

    return true;
  }
}
