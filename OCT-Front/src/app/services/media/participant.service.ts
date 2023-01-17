import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError } from 'rxjs';
import { ParticipantFilters, ParticipantModel, UpdateParticipantModel } from 'src/app/models/media/participant';
import { errorHandler, Nullable } from '../infrastructure/common';
import { Routes } from '../infrastructure/routes';

@Injectable({
  providedIn: 'root'
})
export class ParticipantService {
  constructor(
    private httpClient: HttpClient,
  ) {
  }

  get(filters: ParticipantFilters): Observable<ParticipantModel[]> {
    const params = this.createHttpParamsObject(filters)
    return this.httpClient.get<ParticipantModel[]>(`${Routes.APIHost}/api/participants`, { params: params })
      .pipe(
        catchError(errorHandler)
      );
  }

  getById(id: string): Observable<ParticipantModel> {
    return this.httpClient.get<ParticipantModel>(`${Routes.APIHost}/api/participants/${id}`)
      .pipe(
        catchError(errorHandler)
      );
  }

  getPicture(id: string): Observable<Nullable<Blob>> {
    return this.httpClient.get(`${Routes.APIHost}/api/participants/${id}/photo`, { responseType: "blob" })
      .pipe(
        catchError(errorHandler)
      );
  }

  delete(id: string) {
    return this.httpClient.delete(`${Routes.APIHost}/api/participants/${id}`)
      .pipe(
        catchError(errorHandler)
      );
  }

  uploadPicture(id: string, file: File): Observable<any> {
    const formData = new FormData()
    formData.append("picture", file, file.name)

    return this.httpClient.post(`${Routes.APIHost}/api/participants/${id}/photo`, formData)
      .pipe(
        catchError(errorHandler)
      );
  }

  deletePicture(id: string): Observable<any> {
    return this.httpClient.delete(`${Routes.APIHost}/api/participants/${id}/photo`)
      .pipe(
        catchError(errorHandler)
      );
  }

  update(id: string, participantModel: UpdateParticipantModel) {
    return this.httpClient.put(`${Routes.APIHost}/api/participants/${id}`, participantModel)
      .pipe(
        catchError(errorHandler)
      );
  }

  post(participantModel: UpdateParticipantModel) {
    return this.httpClient.post(`${Routes.APIHost}/api/participants`, participantModel)
      .pipe(
        catchError(errorHandler)
      );
  }
  
  private createHttpParamsObject(filters?: ParticipantFilters): HttpParams {
    let params = new HttpParams();

    if (filters?.limit) params = params.append('limit', filters!.limit)
    if (filters?.offset) params = params.append('offset', filters!.offset)
    if (filters?.nameStartsWith) params = params.append('nameStartsWith', filters!.nameStartsWith)
    if (filters?.surnameStartsWith) params = params.append('surnameStartsWith', filters!.surnameStartsWith)
    if (filters?.role) params = params.append('role', filters!.role)

    return params;
  }
}
