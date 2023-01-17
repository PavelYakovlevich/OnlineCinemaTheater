import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Operation } from 'rfc6902';
import { catchError, Observable } from 'rxjs';
import { MediaFiltersModel, MediaModel } from 'src/app/models/media/media';
import { errorHandler } from '../infrastructure/common';
import { Routes } from '../infrastructure/routes';

@Injectable({
  providedIn: 'root'
})
export class MediaInfoService {
  constructor(
    private httpClient: HttpClient,
  ) { }

  getAll(offset: number, limit: number, filters?: MediaFiltersModel): Observable<MediaModel[]> {
    const params = this.createHttpParamsObject(offset, limit, filters)

    return this.httpClient.get<MediaModel[]>(`${Routes.APIHost}/api/media-infos`, { params: params })
      .pipe(
        catchError(errorHandler)
      );
  }

  get(id: string): Observable<MediaModel> {
    return this.httpClient.get<MediaModel>(`${Routes.APIHost}/api/media-infos/${id}`)
      .pipe(
        catchError(errorHandler)
      );
  }

  getPicture(id: string): Observable<Blob> {
    return this.httpClient.get(`${Routes.APIHost}/api/media-infos/${id}/picture`, { responseType: "blob" })
      .pipe(
        catchError(errorHandler)
      );
  }

  delete(id: string): Observable<any> {
    return this.httpClient.delete(`${Routes.APIHost}/api/media-infos/${id}`)
      .pipe(
        catchError(errorHandler)
      );
  }

  deletePicture(id: string) {
    return this.httpClient.delete(`${Routes.APIHost}/api/media-infos/${id}/picture`)
    .pipe(
      catchError(errorHandler)
    );
  }

  uploadPicture(id: string, file: File) {
    const formData = new FormData(); 
    formData.append("picture", file, file.name);
      
    return this.httpClient.post(`${Routes.APIHost}/api/media-infos/${id}/picture`, formData)
    .pipe(
      catchError(errorHandler)
    );
  }

  patch(id: string, patchDocument: Operation[]): Observable<any> {
    return this.httpClient.patch(`${Routes.APIHost}/api/media-infos/${id}`, patchDocument)
    .pipe(
      catchError(errorHandler)
    );
  }

  create(mediaModel: MediaModel): Observable<string> {
      return this.httpClient.post<string>(`${Routes.APIHost}/api/media-infos`, mediaModel)
      .pipe(
        catchError(errorHandler)
      );
  }

  private createHttpParamsObject(offset: number, limit: number, filters?: MediaFiltersModel): HttpParams {
    let params = new HttpParams({
      fromObject: {
        offset: offset,
        limit: limit,
      }
    });

    if (filters?.country) params = params.append('country', filters!.country)
    if (filters?.isTvSerias) params =params.append('isTVSerias', filters!.isTvSerias)
    if (filters?.nameStartsWith) params =params.append('nameStartsWith', filters!.nameStartsWith)
    if (filters?.genres.length !== 0) filters?.genres.forEach(g => params = params.append('genres', g))

    return params;
  }
}
