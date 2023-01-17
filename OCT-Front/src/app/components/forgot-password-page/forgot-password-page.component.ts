import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { catchError, finalize, of } from 'rxjs';
import { ErrorModel } from 'src/app/models/infrastructure/ErrorModel';
import { AuthenticationService } from 'src/app/services/authentication/authentication.service';
import { getErrorMessage, Nullable } from 'src/app/services/infrastructure/common';
import { ErrorService } from 'src/app/services/infrastructure/error.service';
import { Validators } from 'src/app/services/infrastructure/validators';

@Component({
  selector: 'app-forgot-password-page',
  templateUrl: './forgot-password-page.component.html',
  styleUrls: ['./forgot-password-page.component.scss']
})
export class ForgotPasswordPageComponent implements OnInit {
  email = new FormControl('', [Validators.email]);

  requestInProgress = false
  emailWasSent = false
  lastError: Nullable<ErrorModel> = null

  constructor(
    private authService: AuthenticationService
  ) { }

  ngOnInit(): void {
  }

  getErrorMessage(control: FormControl): string {
    return getErrorMessage(control)
  }

  onSendRestorationEmailBtnWasClicked() {

    const email = this.email.value!

    this.requestInProgress = true
    this.authService.requestPasswordChangeMail(email)
    .pipe(
      catchError((err, _caught) => of(err)),
      finalize(() => this.requestInProgress = false)
    )
    .subscribe(result => {
      if (result instanceof ErrorModel) {
        this.lastError = result
        return
      }

      this.emailWasSent = true
    })
  }
}
