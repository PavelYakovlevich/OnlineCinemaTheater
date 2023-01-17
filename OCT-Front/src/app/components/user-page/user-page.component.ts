import { STEPPER_GLOBAL_OPTIONS } from '@angular/cdk/stepper';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { catchError, finalize, of } from 'rxjs';
import { ErrorModel } from 'src/app/models/infrastructure/ErrorModel';
import { UserModel } from 'src/app/models/user/user';
import { createImageFromBlob, Nullable } from 'src/app/services/infrastructure/common';
import { LocalStorageService } from 'src/app/services/infrastructure/local-storage.service';
import { Validators } from 'src/app/services/infrastructure/validators';
import { UserService } from 'src/app/services/user/user.service';

@Component({
  selector: 'app-user-page',
  templateUrl: './user-page.component.html',
  styleUrls: ['./user-page.component.scss'],
  providers: [
    {
      provide: STEPPER_GLOBAL_OPTIONS,
      useValue: { showError: true },
    },
  ],
})
export class UserPageComponent implements OnInit {
  user!: UserModel
  photo: any = null

  initialSetup = true
  changeUserBtnWasPressed = false
  
  pictureRequestIsInProgres = false
  requestInProgres = false

  creadentialsFormGroup!: FormGroup
  birthdayFormGroup!: FormGroup

  constructor(
    private userService: UserService,
    private localStorageService: LocalStorageService
  ) { }

  ngOnInit(): void {
    this.loadUser()
    this.initializeForm()
  }

  onDeleteFile() {
    if (!this.user) return

    this.pictureRequestIsInProgres = true
    this.userService.deletePhoto(this.user.id)
      .pipe(
        catchError((err, _caught) => of(err)),
        finalize(() => this.pictureRequestIsInProgres = false)
      )
      .subscribe(value => {
        if (value instanceof ErrorModel) return
        this.photo = null
      })
  }

  onFileSelected(event: Event) {
    if(!this.user) return

    const fileInput = event.target as HTMLInputElement

    if (!fileInput.files?.length) return

    const file = fileInput.files[0]

    this.pictureRequestIsInProgres = true

    this.userService.uploadPicture(this.user.id, file)
      .pipe(
        catchError((err, _caught) => of(err)),
        finalize(() => this.pictureRequestIsInProgres = false)
      )
      .subscribe(value => {
        if (value instanceof ErrorModel) return
        this.photo = createImageFromBlob(file as Blob, (img) => this.photo = img)
      })
  }

  onSaveUserAccountBtnWasClicked() {
    this.requestInProgres = true

    this.userService.update(this.user!.id, this.user!)
      .pipe(
        catchError((err, _caught) => of(err)),
        finalize(() => this.requestInProgres = false)
      )
      .subscribe(value => {
        if (value instanceof ErrorModel) return

        this.initialSetup = false
        this.changeUserBtnWasPressed = false

        this.localStorageService.setUser(this.user)
      })
  }

  onChangeBtnWasClicked() {
    this.changeUserBtnWasPressed = true
  }

  getErrorMessage(formGroup: FormGroup, controlName: string): string {
    const control = formGroup.get(controlName)

    if (control && control.invalid) {
      const errorValue = Object.values(control.errors!).pop()

      if (errorValue instanceof String) return errorValue as string

      if (control.hasError('required')) return 'This value is required' 

      return 'Value is invalid'
    }

    return ''
  }

  userCanSave(): boolean {
    return !this.birthdayFormGroup.invalid && !this.creadentialsFormGroup.invalid
  }

  setupIsRequired(): boolean {
    return this.initialSetup || this.changeUserBtnWasPressed
  }

  private loadUser() {
    this.user = this.localStorageService.getUser()!

    this.initialSetup = !this.user.name || !this.user.surname

    this.loadPhoto()
  }

  private loadPhoto() {
    if (!this.user) return

    this.pictureRequestIsInProgres = true
    this.userService.getPicture(this.user?.id)
      .pipe(
        catchError((err, _caught) => of(err)),
        finalize(() => this.pictureRequestIsInProgres = false)
      )
      .subscribe(value => {
        if (value instanceof ErrorModel) return

        createImageFromBlob(value, imageUrl => this.photo = imageUrl)
      })
  }

  private initializeForm() {
    this.creadentialsFormGroup = new FormGroup({
      'name': new FormControl(this.user.name ?? '', Validators.personName),
      'lastName': new FormControl(this.user.surname ?? '', Validators.personName),
      'description': new FormControl(this.user.description ?? '')
    });

    this.creadentialsFormGroup.get('name')!.valueChanges
      .subscribe(value => this.user.name = value)

    this.creadentialsFormGroup.get('lastName')!.valueChanges
      .subscribe(value => this.user.surname = value)

    this.creadentialsFormGroup.get('description')!.valueChanges
      .subscribe(value => this.user.description = value)
  
    this.birthdayFormGroup = new FormGroup({
      'birthday': new FormControl(this.initialSetup ? '' : this.user.birthday, Validators.personBirthday)
    });

    this.birthdayFormGroup.get('birthday')!.valueChanges
      .subscribe(value => this.user.birthday = value)
  }
}
