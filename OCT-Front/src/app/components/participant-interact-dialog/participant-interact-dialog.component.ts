import { Component, Inject, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators as NgValidators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { catchError, debounceTime, finalize, Observable } from 'rxjs';
import { CountryModel } from 'src/app/models/infrastructure/country';
import { ErrorModel } from 'src/app/models/infrastructure/ErrorModel';
import { ModalCloseMode } from 'src/app/models/infrastructure/modal';
import { ParticipantModel, ParticipantRole, ParticipantViewModel, UpdateParticipantModel } from 'src/app/models/media/participant';
import { Nullable } from 'src/app/services/infrastructure/common';
import { Validators } from 'src/app/services/infrastructure/validators';
import { ParticipantService } from 'src/app/services/media/participant.service';

@Component({
  selector: 'app-participant-interact-dialog',
  templateUrl: './participant-interact-dialog.component.html',
  styleUrls: ['./participant-interact-dialog.component.scss']
})
export class ParticipantInteractDialogComponent implements OnInit {
  participant: ParticipantViewModel = {
    id: '',
    name: '',
    surname: '',
    birthday: null,
    description: '',
    role: ParticipantRole.Actor,
    country: null
  }

  isOpenedInModificationMode = false
  requestInProgress = false
  participantFormGroup! :FormGroup

  lastRequestError: Nullable<ErrorModel> = null

  constructor(
    public dialogRef: MatDialogRef<ParticipantInteractDialogComponent>,
    private participantService: ParticipantService,
    private router: Router,
    @Inject(MAT_DIALOG_DATA) public participantModel: Nullable<ParticipantViewModel>,
    ) {
    if (participantModel) {
      this.participant = {...participantModel}
    }

    this.isOpenedInModificationMode = participantModel !== null
    this.initializeForm()
  }

  ngOnInit(): void {
  }

  controlIsInvalid(controlName: string): boolean {
    const control = this.participantFormGroup.get(controlName)!
    return control.invalid
  }

  onSaveBtnWasClicked(): void {
    this.requestInProgress = true

    if (this.isOpenedInModificationMode) {
      this.updateParticipant()
      return
    }
    
    this.createParticipant()
  }

  createParticipant() {
    const participantModel = this.createParticipantRequestModel(this.participant)

    this.participantService.post(participantModel)
    .pipe(
      catchError(this.handleError),
      finalize(() => this.requestInProgress = false) 
    )
    .subscribe(result => {
      if (result instanceof ErrorModel) return

      this.dialogRef.close({ closeMode: ModalCloseMode.ClosedAfterAction, id: result })
    })
  }

  updateParticipant() {
    const participantModel = this.createParticipantRequestModel(this.participant)

    this.participantService.update(this.participant.id, participantModel)
    .pipe(
      catchError(this.handleError),
      finalize(() => this.requestInProgress = false)
    )
    .subscribe(value => {
      if(value instanceof ErrorModel) return

      this.dialogRef.close({ closeMode: ModalCloseMode.ClosedAfterAction } )
    })
  }

  allControlsAreValid(): boolean {
    let result = true

    Object.keys(this.participantFormGroup.controls).forEach(key => {
      result = result && !this.participantFormGroup.get(key)!.invalid
    })

    return result && this.participant.country !== null
  }

  getErrorMessage(controlName: string): string {
    const control = this.participantFormGroup.get(controlName)!

    if (control.hasError('required')) return 'Field value is required'
    if (control.hasError('maxlength')) return 'Field value is too long'
    
    return 'Field value is invalid'
  }

  onCountryWasSelected(country: Nullable<CountryModel>): void {
    if (country) {
      this.participant.country = country
    }
  }

  getParticipantRoles(): ParticipantRole[] {
    const result: ParticipantRole[] = []

    Object.values(ParticipantRole).forEach(v => {
      if (typeof v !== 'string') result.push(v)
    })
  
    return result;
  } 

  getParticipantRoleString(participantType: ParticipantRole): string {
    return ParticipantRole[participantType]
  }

  private initializeForm() {
    this.participantFormGroup = new FormGroup({
      "name": new FormControl(this.participant.name, Validators.participantName),
      "surname": new FormControl(this.participant.surname, Validators.participantSurname),
      "birthday": new FormControl(this.participant.birthday, Validators.participantBirthday),
      "career": new FormControl(this.participant.role),
      "description": new FormControl(this.participant.description, [NgValidators.required, Validators.notWhitespace]),
    });

    const debounceTimeValue = 500;

    this.participantFormGroup.get('name')!.valueChanges
    .pipe(debounceTime(debounceTimeValue))
    .subscribe((changes) => {
      this.participant.name = changes
    })

    this.participantFormGroup.get('surname')!.valueChanges
    .pipe(debounceTime(debounceTimeValue))
    .subscribe((changes) => {
      this.participant.surname = changes
    })

    this.participantFormGroup.get('birthday')!.valueChanges
    .pipe(debounceTime(debounceTimeValue))
    .subscribe((changes) => {
      this.participant.birthday = new Date(changes).toISOString()
    })

    this.participantFormGroup.get('description')!.valueChanges
    .pipe(debounceTime(debounceTimeValue))
    .subscribe((changes) => {
      this.participant.description = changes
    })

    this.participantFormGroup.get('career')!.valueChanges
    .pipe(debounceTime(debounceTimeValue))
    .subscribe((changes) => this.participant.role = changes)
  }

  private createParticipantRequestModel(participantModel: ParticipantViewModel): UpdateParticipantModel {
    return {
      id: this.participant!.id,
      name: this.participant!.name,
      surname: this.participant!.surname,
      birthday: this.participant!.birthday,
      description: this.participant!.description,
      role: this.participant!.role,
      country: this.participant.country!.cioc
    }
  }

  private handleError<T>(error: any, _caught: Observable<T>): Observable<any> {
    this.lastRequestError = error
    return error
  }
}