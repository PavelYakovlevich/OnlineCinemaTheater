import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable } from 'rxjs';
import { GenreModel } from 'src/app/models/media/genre';
import { errorHandler } from '../infrastructure/common';
import { ErrorService } from '../infrastructure/error.service';
import { Routes } from '../infrastructure/routes';

@Injectable({
  providedIn: 'root'
})
export class GenreService {
  constructor(
    private httpClient: HttpClient,
    private errorService: ErrorService
  ) {}

  getAll(): Observable<GenreModel[]> {
    return this.httpClient.get<GenreModel[]>(`${Routes.APIHost}/api/genres`)
      .pipe(
        catchError((err, caught) => errorHandler(err, caught, () => this.errorService.handleError(err)))
      )
  }

  delete(id: string): Observable<any> {
    return this.httpClient.delete(`${Routes.APIHost}/api/genres/${id}`)
      .pipe(
        catchError((err, caught) => errorHandler(err, caught, () => this.errorService.handleError(err)))
      )
  }

  create(model: { name: string; }) {
    return this.httpClient.post(`${Routes.APIHost}/api/genres`, model)
      .pipe(
        catchError((err, caught) => errorHandler(err, caught, () => this.errorService.handleError(err)))
      )
  }
}
