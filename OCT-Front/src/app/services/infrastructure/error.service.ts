import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { ErrorModel } from 'src/app/models/infrastructure/ErrorModel';
import { Nullable } from './common';

@Injectable({
  providedIn: 'root'
})
export class ErrorService {
  public error$ = new Subject<Nullable<ErrorModel>>()

  handleError(error: ErrorModel): void {
    this.error$.next(error)
  }

  clear(): void {
    this.error$.next(null)
  }
}
