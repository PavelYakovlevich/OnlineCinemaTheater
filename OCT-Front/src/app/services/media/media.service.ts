import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError } from 'rxjs';
import { errorHandler, Nullable } from '../infrastructure/common';
import { Routes } from '../infrastructure/routes';

@Injectable({
  providedIn: 'root'
})
export class MediaService {
  constructor(
    private httpClient: HttpClient
  ) { }

  getTrailer(id: string): Observable<Nullable<Blob>> {
    return this.httpClient.get(`${Routes.APIHost}/api/medias/${id}/trailer`, { responseType: "blob" })
      .pipe(
        catchError(errorHandler)
      );
  }

  uploadTrailer(id: string, file: File): Observable<any> {
    const formData = new FormData()
    formData.append('contentFile', file, file.name)

    return this.httpClient.post(`${Routes.APIHost}/api/medias/${id}/trailer`, formData)
    .pipe(
      catchError(errorHandler)
    );
  }

  deleteTrailer(id: string) {
    return this.httpClient.delete(`${Routes.APIHost}/api/medias/${id}/trailer`)
    .pipe(
      catchError(errorHandler)
    );
  }
}
