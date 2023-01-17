import { Injectable } from '@angular/core';
import { UserModel } from 'src/app/models/user/user';
import { Nullable } from './common';

@Injectable({
  providedIn: 'root'
})
export class LocalStorageService {
  constructor() { }

  setUser(user: UserModel) {
    localStorage.setItem('user', JSON.stringify(user))
  }

  getUser(): Nullable<UserModel> {
    const userJson = localStorage.getItem('user')

    if (!userJson) return null

    return JSON.parse(userJson)
  }
}
