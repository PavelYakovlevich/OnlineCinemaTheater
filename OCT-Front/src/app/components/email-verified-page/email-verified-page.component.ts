import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, finalize, of } from 'rxjs';
import { ErrorModel } from 'src/app/models/infrastructure/ErrorModel';
import { AuthenticationService } from 'src/app/services/authentication/authentication.service';
import { Nullable } from 'src/app/services/infrastructure/common';
import { TokenService } from 'src/app/services/infrastructure/confirmation-token.service';

@Component({
  selector: 'app-email-verified-page',
  templateUrl: './email-verified-page.component.html',
  styleUrls: ['./email-verified-page.component.scss']
})
export class EmailVerifiedPageComponent implements OnInit {
  requestInProgress = false
  lastError: Nullable<ErrorModel> = null

  constructor(
    private authService: AuthenticationService,
    private tokenService: TokenService,
    private router: Router,
  ) { }

  ngOnInit(): void {
    this.tokenService.getToken()
    .subscribe(value => {
      if (!value) {
        this.router.navigate(['/'])
        return
      }

      this.requestInProgress = true
      this.authService.verifyEmail(value)
        .pipe(
          catchError((err, _caught) => {
            this.lastError = err
            return of(err)
          }),
          finalize(() => this.requestInProgress = false)
        )
        .subscribe()
    })
  }
}
