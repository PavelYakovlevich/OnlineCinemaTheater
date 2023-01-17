import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable } from 'rxjs';
import { CommentModel } from 'src/app/models/comment/CommentModel';
import { CreateCommentModel } from 'src/app/models/comment/CreateCommentModel';
import { errorHandler } from '../infrastructure/common';
import { Routes } from '../infrastructure/routes';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  constructor(
    private httpClient: HttpClient,
  ) { }

  getAll(offset: number = 0, limit: number = 20, mediaId: string): Observable<CommentModel[]> {
    let params = new HttpParams({
      fromObject: {
        offset: offset,
        limit: limit,
      }
    })

    return this.httpClient.get<CommentModel[]>(`${Routes.APIHost}/api/medias/${mediaId}/comments`, { params: params })
      .pipe(
        catchError(errorHandler)
      );
  }

  delete(comment: CommentModel): Observable<any> {
    return this.httpClient.delete(`${Routes.APIHost}/api/medias/${comment.mediaId}/comments/${comment.id}`)
      .pipe(
        catchError(errorHandler)
      );
  }

  update(comment: CommentModel): Observable<any> {
    return this.httpClient.put(`${Routes.APIHost}/api/medias/${comment.mediaId}/comments/${comment.id}`, {
      text: comment.text,
      userId: comment.userId
    })
      .pipe(
        catchError(errorHandler)
      );
  }

  post(mediaId: string, commentModel: CreateCommentModel): Observable<any> {
    return this.httpClient.post(`${Routes.APIHost}/api/medias/${mediaId}/comments`, commentModel)
      .pipe(
        catchError(errorHandler)
      );
  }
}
