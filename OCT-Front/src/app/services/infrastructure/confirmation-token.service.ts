import { Injectable } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { catchError, filter, map, Observable, of } from 'rxjs';
import { Nullable } from './common';

@Injectable({
  providedIn: 'root'
})
export class TokenService {

  constructor(
    private route: ActivatedRoute
  ) {}

  getToken(): Observable<Nullable<string>> {
    return this.route.queryParams
      .pipe(
        catchError((_err, _caught) => of(null)),
        filter(params => params?.['token']),
        map((params, _) => params?.['token'])
      )
  }
}
