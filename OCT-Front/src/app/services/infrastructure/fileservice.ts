import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { catchError, Observable, ObservableInput, throwError } from "rxjs";
import { Nullable } from "./common";
import { ErrorService } from "./error.service";

export abstract class PictureService {
    constructor(
        protected httpClient: HttpClient,
        protected errorService: ErrorService
    ) {
    }

    protected getPictureCore(url: string): Observable<Nullable<Blob>> {
        return this.httpClient.get(url, { responseType: "blob" })
          .pipe(catchError(async (_error, _caught) => null));
    }
}