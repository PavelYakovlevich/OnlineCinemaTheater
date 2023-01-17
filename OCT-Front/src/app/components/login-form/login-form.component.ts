import { Component, HostListener, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { catchError, finalize, Observable, of } from 'rxjs';

import { AccountModel } from 'src/app/models/authentication/AccountModel';
import { ErrorModel } from 'src/app/models/infrastructure/ErrorModel';
import { AuthenticationService } from 'src/app/services/authentication/authentication.service';
import { getAccountFromCookies, Nullable } from 'src/app/services/infrastructure/common';
import { LocalStorageService } from 'src/app/services/infrastructure/local-storage.service';
import { Validators as CustomValidators } from 'src/app/services/infrastructure/validators';
import { UserService } from 'src/app/services/user/user.service';

@Component({
  selector: 'app-authentication-form',
  templateUrl: './login-form.component.html',
  styleUrls: ['./login-form.component.scss']
})
export class LoginFormComponent implements OnInit {
  public email = new FormControl('', [CustomValidators.email]);
  public password = new FormControl('', [CustomValidators.password(8, 50)]);
  public loginForm = new FormGroup({
    "email": this.email,
    "password": this.password
  });

  public hidePassword: boolean = true
  public requestInProgress = false
  public lastError: Nullable<ErrorModel> = null

  constructor(
    private authService: AuthenticationService,
    private userService: UserService,
    private localStorageService: LocalStorageService,
    private router: Router
  ) {}

  ngOnInit(): void {
  }

  @HostListener("keydown.enter") 
  onLoginBtnWasClicked() :void {
    const account: AccountModel = {
      email: this.email.value ?? '',
      password: this.password.value ?? '',
    }

    this.requestInProgress = true

    this.authService.login(account)
    .pipe(
      catchError(this.handleError.bind(this))
    )
    .subscribe(result => {
      if (result instanceof ErrorModel) {
        this.requestInProgress = false
        return
      }

      const accountInfo = getAccountFromCookies()
      if (accountInfo) {
        this.userService.get(accountInfo!.userId)
        .pipe(
          catchError(this.handleError.bind(this)),
          finalize(() => this.requestInProgress = false)
        )
        .subscribe(result => {
          if (result instanceof ErrorModel) return

          this.localStorageService.setUser(result)

          this.router.navigate(['']);
        })
      }
    })
  }

  getErrorMessage(control: FormControl): string {
    let firstError = Object.keys(control.errors!).pop()!
    return control.getError(firstError)
  }

  allInformationIsValid() {
    let informationIsValid = true;

    Object.keys(this.loginForm.controls).forEach(control => {
      informationIsValid = informationIsValid && this.loginForm.get(control)!.valid
    });

    return informationIsValid
  }

  private handleError(err: any, caught: Observable<any>): Observable<any> {
    this.lastError = err
    return of(err)
  }
}
