import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, map, Observable } from 'rxjs';
import { CountryModel } from '../../models/infrastructure/country';
import { errorHandler } from './common';
import { Routes } from './routes';

@Injectable({
  providedIn: 'root'
})
export class CountryService {
  constructor(
    private httpClient: HttpClient,
  ) { }

  get(cioc: string): Observable<CountryModel> {
    return this.httpClient.get<CountryModel[]>(`${Routes.CountryAPIHost}/v2/alpha?codes=${cioc}`)
      .pipe(
        catchError(errorHandler),
        map(values => values.pop()!)
      );
  }

  getAll(): Observable<CountryModel[]> {
    return this.httpClient.get<CountryModel[]>(`${Routes.CountryAPIHost}/v2/all?fields=name,flag,cioc`)
      .pipe(
        catchError(errorHandler)
      );
  }
}
