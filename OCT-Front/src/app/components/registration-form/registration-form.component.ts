import { Component, HostListener, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors } from '@angular/forms';
import { Validators as CustomValidators } from 'src/app/services/infrastructure/validators';
import { Router } from '@angular/router';

import { AccountModel } from 'src/app/models/authentication/AccountModel';
import { AuthenticationService } from 'src/app/services/authentication/authentication.service';
import { catchError, finalize, of } from 'rxjs';
import { ErrorModel } from 'src/app/models/infrastructure/ErrorModel';

@Component({
  selector: 'app-registration-form',
  templateUrl: './registration-form.component.html',
  styleUrls: ['./registration-form.component.scss']
})
export class RegistrationFormComponent implements OnInit {
  public emailControl = new FormControl('', [CustomValidators.email]);
  public passwordControl = new FormControl('', [CustomValidators.password(5, 80)]);
  public repeatPasswordControl = new FormControl('', [this.repeatPasswordValueValidator.bind(this)]);

  public registrationForm = new FormGroup({
    "email": this.emailControl,
    "password": this.passwordControl,
    "repeatPassword": this.repeatPasswordControl
  });

  public hidePassword: boolean = true

  public requestInProgress = false

  constructor(
    private authService: AuthenticationService,
    private router: Router
  ) {}

  ngOnInit(): void {
  }

  @HostListener("keydown.enter") 
  onRegisterBtnWasClicked() :void {
    let account: AccountModel = {
      email: this.emailControl.value ?? '',
      password: this.passwordControl.value ?? '',
    }

    this.requestInProgress = true

    this.authService.register(account)
      .pipe(
        catchError((err, _caught) => of(err)),
        finalize(() => this.requestInProgress = false)
      )
      .subscribe(result => {
        if (result instanceof ErrorModel) return
        
        this.router.navigate(['/login']);
      })
  }

  getErrorMessage(control: FormControl): string {
    const firstError = Object.keys(control.errors!).pop()!
    return control.getError(firstError)
  }

  allInformationIsValid() {
    let informationIsValid = true;

    Object.keys(this.registrationForm.controls).forEach(control => {
      informationIsValid = informationIsValid && this.registrationForm.get(control)!.valid
    });

    return informationIsValid
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
