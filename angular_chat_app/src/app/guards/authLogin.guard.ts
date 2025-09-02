import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthLoginGuard implements CanActivate {

  constructor(private router: Router) {}

  canActivate(): Observable<boolean | UrlTree> {
    const token = localStorage.getItem('auth-token');
    const refreshToken = localStorage.getItem('refresh-token');

    if (token && refreshToken) {
      // Có token => về trang home
      return of(this.router.createUrlTree(['/home']));
    }

    // Không có token => về login
    return of(this.router.createUrlTree(['/login']));
  }
}
