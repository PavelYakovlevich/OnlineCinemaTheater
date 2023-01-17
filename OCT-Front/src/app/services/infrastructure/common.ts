import { AccountInfoModel } from "src/app/models/AccountInfoModel";
import jwt_decode from 'jwt-decode';
import { Observable, ObservableInput, throwError } from "rxjs";
import { ErrorModel } from "src/app/models/infrastructure/ErrorModel";
import { HttpErrorResponse } from "@angular/common/http";
import { UserModel } from "src/app/models/user/user";
import { FormControl } from "@angular/forms";

export type Nullable<T> = T | null;

export function createImageFromBlob(image: Blob, imageWasLoaded: (value: string | ArrayBuffer | null) => void) {
    let reader = new FileReader();
    reader.addEventListener("load", () => {
      imageWasLoaded(reader.result)
    }, false);
  
    if (image) {
       reader.readAsDataURL(image);
    }
}

export function getAccountFromCookies(): Nullable<AccountInfoModel> {
    const accessToken = getCookie("accessToken")
    const decodedJwtData = getDecodedAccessToken(accessToken);

    if (!decodedJwtData) {
        return null
    }

    return new AccountInfoModel(decodedJwtData.email, decodedJwtData.nameid, decodedJwtData.role);
}

function getDecodedAccessToken(token: string): any {
    try {
        return jwt_decode<AccountInfoModel>(token);
    } catch(Error) {
        return null;
    }
}

export function getCookie(name: string): string {
    let ca: Array<string> = document.cookie.split(';');
    let caLen: number = ca.length;
    let cookieName = `${name}=`;
    let c: string;

    for (let i: number = 0; i < caLen; i += 1) {
        c = ca[i].replace(/^\s+/g, '');
        if (c.indexOf(cookieName) == 0) {
            return c.substring(cookieName.length, c.length);
        }
    }
    
    return '';
}

export function errorHandler<T>(response: HttpErrorResponse, _: Observable<T>, hook?: () => any): ObservableInput<never> {
    hook?.()

    return throwError(() => new ErrorModel(response.status, response.message, response.error));
}

export function logout() {
    deleteCookie('accessToken')
    deleteCookie('refreshToken')
    localStorage.clear()
}

export function setCookie(name: string, value: string) {
    document.cookie = name +'='+ value +'; Path=/;';
}

export function deleteCookie(name: string) {
    document.cookie = name +'=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
}

export function userFinishedSetup(user: UserModel): boolean {
    return user.name !== null || user.surname !== null 
}

export function getErrorMessage(control: FormControl): string {
    let firstError = Object.keys(control.errors!).pop()!
    return control.getError(firstError)
}