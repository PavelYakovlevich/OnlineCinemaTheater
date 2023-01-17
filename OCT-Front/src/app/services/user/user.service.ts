import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/internal/Observable';
import { catchError } from 'rxjs/internal/operators/catchError';
import { UserModel } from 'src/app/models/user/user';
import { errorHandler, Nullable } from '../infrastructure/common';
import { Routes } from '../infrastructure/routes';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  constructor(
    private httpClient: HttpClient,
  ) {
  }

  getPicture(id: string): Observable<Nullable<Blob>> {
    return this.httpClient.get(`${Routes.APIHost}/api/users/${id}/photo`, 
      { 
        responseType: "blob" 
      })
      .pipe(
        catchError(errorHandler)
      );
  }

  get(id: string): Observable<UserModel> {
    return this.httpClient.get<UserModel>(`${Routes.APIHost}/api/users/${id}`)
      .pipe(
        catchError(errorHandler)
      );
  }

  deletePhoto(id: string): Observable<any> {
    return this.httpClient.delete(`${Routes.APIHost}/api/users/${id}/photo`)
      .pipe(
        catchError(errorHandler)
      );
  }

  uploadPicture(id: string, file: File): Observable<any> {
    const formData = new FormData()
    formData.append('photo', file, file.name)

    return this.httpClient.post(`${Routes.APIHost}/api/users/${id}/photo`, formData)
    .pipe(
      catchError(errorHandler)
    );
  }

  update(id: string, user: UserModel): Observable<any> {
    return this.httpClient.put(`${Routes.APIHost}/api/users/${id}`, user)
    .pipe(
      catchError(errorHandler)
    );
  }
}
