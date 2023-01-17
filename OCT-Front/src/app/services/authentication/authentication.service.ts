import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AccountModel } from 'src/app/models/authentication/AccountModel';

import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ErrorService } from '../infrastructure/error.service';
import { errorHandler, Nullable } from '../infrastructure/common';
import { Routes } from '../infrastructure/routes';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  constructor(
    private httpClient: HttpClient,
    private errorService: ErrorService
  ) { }

  login(model: AccountModel): Observable<AccountModel> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      }),
      withCredentials: true}
    
    return this.httpClient.post<AccountModel>(`${Routes.APIHost}/api/auth/login`, model, httpOptions)
      .pipe(
        catchError(errorHandler)
      );
  }

  register(model: AccountModel): Observable<AccountModel> {
    const httpOptions = {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })}
    
    return this.httpClient.post<AccountModel>(`${Routes.APIHost}/api/auth`, model, httpOptions)
      .pipe(
        catchError((err, caught) => errorHandler(err, caught, () => this.errorService.handleError(err)))
      ); 
  }

  refresh(): Observable<void> {
    const httpOptions = {
      withCredentials: true
    }

    return this.httpClient.post<void>(`${Routes.APIHost}/api/auth/refresh`, null, httpOptions)
  }

  verifyEmail(token: Nullable<string>): Observable<any> {
    return this.httpClient.get(`${Routes.APIHost}/api/auth/confirm?token=${token}`)
      .pipe(
        catchError(errorHandler)
      );
  }

  requestPasswordChangeMail(email: string): Observable<any> {
    return this.httpClient.post(`${Routes.APIHost}/api/auth/change-password`, { email })
      .pipe(
        catchError(errorHandler)
      );
  }

  changePassword(userId: string, token: string, password: string) {
    return this.httpClient.put(`${Routes.APIHost}/api/auth/${userId}/password?token=${token}`, { password })
      .pipe(
        catchError(errorHandler)
      );
  }
}
