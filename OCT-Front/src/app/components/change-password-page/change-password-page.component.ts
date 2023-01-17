import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormControl, ValidationErrors } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { catchError, finalize, of } from 'rxjs';
import { ErrorModel } from 'src/app/models/infrastructure/ErrorModel';
import { AuthenticationService } from 'src/app/services/authentication/authentication.service';
import { getErrorMessage, Nullable } from 'src/app/services/infrastructure/common';
import { TokenService } from 'src/app/services/infrastructure/confirmation-token.service';
import { Validators } from 'src/app/services/infrastructure/validators';

@Component({
  selector: 'app-change-password-page',
  templateUrl: './change-password-page.component.html',
  styleUrls: ['./change-password-page.component.scss']
})
export class ChangePasswordPageComponent implements OnInit {
  requestInProgress = false
  emailWasSent = false
  lastError: Nullable<ErrorModel> = null
  hidePassword = true

  passwordControl = new FormControl('', [ Validators.password(5, 80) ]);
  repeatPasswordControl = new FormControl('', [this.repeatPasswordValueValidator.bind(this)]);
  
  constructor(
    private authService: AuthenticationService,
    private tokenService: TokenService,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
  }

  allControlsAreValid(): boolean {
    return (!this.passwordControl.invalid && this.passwordControl.touched) && (!this.repeatPasswordControl.invalid && this.passwordControl.touched)
  }

  onChangePasswordBtnWasClicked() {
    const userId = this.route.snapshot.paramMap.get('userId')!
    
    this.tokenService.getToken()
      .subscribe(token => {
        if (!token) {
          this.router.navigate(['/'])
          return
        }
      
        this.requestInProgress = true

        const password = this.passwordControl.value!

        this.authService.changePassword(userId, token, password)
          .pipe(
            catchError((err, _caught) => {
              this.lastError = err
              return of(err)
            }),
            finalize(() => this.requestInProgress = false)
          )
          .subscribe(result => {
            console.log(result)
            if (result instanceof ErrorModel) return

            this.router.navigate(['/login'])
          })
      })
  }

  getErrorMessage(control: FormControl) {
    return getErrorMessage(control)
  }

  private repeatPasswordValueValidator(control: AbstractControl):  ValidationErrors | null {
    const value: string = control.value;

    if(value !== this.passwordControl.value) {
      return {
        "repeatPasswordUnmatch": "Passwords did not match"
      }
    }

    return null;
  }
}
