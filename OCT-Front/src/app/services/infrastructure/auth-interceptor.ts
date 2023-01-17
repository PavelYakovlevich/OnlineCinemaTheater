import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, Observable, ObservableInput, switchMap, tap, throwError } from 'rxjs';
import { AuthenticationService } from '../authentication/authentication.service';
import { getCookie } from './common';

@Injectable({
  providedIn: 'root'
})
export class AuthInterceptor implements HttpInterceptor{

  constructor(
    private router: Router,
    private authService: AuthenticationService
  ) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const reqWithHeaders = this.setAuthHeaders(req)

    return next.handle(reqWithHeaders)
      .pipe(
        catchError((err, caught) => this.handleError(err, caught, req, next))
      )
  }

  private handleError(err: HttpErrorResponse, _caught: Observable<any>, req: HttpRequest<any>, next: HttpHandler): ObservableInput<any> {
    if (err.status === 401) {
      return this.authService.refresh()
        .pipe(
          catchError((err, _) => {
            localStorage.clear()
            this.router.navigate(['/login'])
            return throwError(() => err)
          }),
          switchMap(() => next.handle(this.setAuthHeaders(req)))
        )
    }

    return throwError(() => err)
  }

  private setAuthHeaders(req: HttpRequest<any>): HttpRequest<any> {
    const token = getCookie('accessToken')

    return req.clone({
      headers: req.headers.set('Authorization', `Bearer ${token}`)
    })
  }
}

