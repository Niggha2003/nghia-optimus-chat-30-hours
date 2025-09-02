import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { Observable, of, throwError } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private router: Router, private _authService: AuthService) {}

  canActivate(): Observable<boolean | UrlTree> {
    const token = localStorage.getItem('auth-token');
    const refreshToken = localStorage.getItem('refresh-token');

    if (!token || !refreshToken) {
      return of(this.router.createUrlTree(['/login']));
    }

    return this._authService.isValidAccess().pipe(
      catchError((error) => {
        if (error.status === 401) {
          console.log('Access token expired, attempting to refresh...');
          // Gọi API refresh token
          return this._authService.refresh({ accessToken: token, refreshToken }).pipe(
            map((response) => {
              if (response) {
                localStorage.setItem('auth-token', response.AccessToken);
                localStorage.setItem('refresh-token', response.RefreshToken);
                return true;
              } else {
                return this.router.createUrlTree(['/login']);
              }
            }),
            catchError(() => of(this.router.createUrlTree(['/login'])))
          );
        }
        return throwError(() => error);
      }),
      switchMap((isValid) => {
        if (isValid) {
          return of(true);
        } 
        return of(this.router.createUrlTree(['/login']));
      }),
      catchError(() => of(this.router.createUrlTree(['/login'])))
    );
  }
}
