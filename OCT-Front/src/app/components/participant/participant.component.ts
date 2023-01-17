import { Component, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { catchError, finalize, Observable } from 'rxjs';
import { AccountInfoModel } from 'src/app/models/AccountInfoModel';
import { CountryModel } from 'src/app/models/infrastructure/country';
import { ErrorModel } from 'src/app/models/infrastructure/ErrorModel';
import { ModalCloseMode } from 'src/app/models/infrastructure/modal';
import { ParticipantModel, ParticipantRole, ParticipantViewModel } from 'src/app/models/media/participant';
import { CountryService } from 'src/app/services/infrastructure/country.service';
import { createImageFromBlob, getAccountFromCookies, Nullable } from 'src/app/services/infrastructure/common';
import { ParticipantService } from 'src/app/services/media/participant.service';
import { ParticipantInteractDialogComponent } from '../participant-interact-dialog/participant-interact-dialog.component';

@Component({
  selector: 'app-participant',
  templateUrl: './participant.component.html',
  styleUrls: ['./participant.component.scss']
})
export class ParticipantComponent implements OnInit {
  public account!: AccountInfoModel

  public participant: Nullable<ParticipantModel> = null
  public countryInfo: Nullable<CountryModel> = null
  public photo: any

  public pictureRequestIsInProgress = false 
  public participantRequestInProgress = false

  public lastRequestError: Nullable<ErrorModel> = null;

  public participantFormGroup!: FormGroup

  constructor(
    private participantDialog: MatDialog,
    private participantService: ParticipantService,
    private countryService: CountryService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.account = getAccountFromCookies()!
    this.loadParticipant()
  }

  onDeleteFile() {
    this.pictureRequestIsInProgress = true

    this.participantService.deletePicture(this.participant!.id)
    .pipe(
      catchError(this.handleError),
      finalize(() => this.pictureRequestIsInProgress = false)
    )
    .subscribe(value => {
      if (value instanceof ErrorModel) return
      this.photo = null
    })
  }

  onFileSelected(event: Event) {
    const fileInput = event.target as HTMLInputElement

    if (!fileInput.files?.length) return

    const file = fileInput.files[0]

    this.pictureRequestIsInProgress = true

    this.participantService.uploadPicture(this.participant!.id, file)
    .pipe(
      catchError(this.handleError),
      finalize(() => this.pictureRequestIsInProgress = false)
    )
    .subscribe(value => {
      if (value instanceof ErrorModel) return
      this.photo = createImageFromBlob(file as Blob, (img) => this.photo = img)
    })
  }

  onDeleteParticipantMenuItemWasClicked() {
    this.pictureRequestIsInProgress = true

    this.participantService.delete(this.participant!.id)
    .pipe(
      catchError(this.handleError),
      finalize(() => this.pictureRequestIsInProgress = false)
    )
    .subscribe(value => {
      if (value instanceof ErrorModel) return
      this.router.navigate(['/'])
    })
  }

  onChangeParticipantBtnWasClicked() {
    const participantViewModel: ParticipantViewModel = {
      id: this.participant!.id,
      name: this.participant!.name,
      surname: this.participant!.surname,
      birthday: this.participant!.birthday,
      description: this.participant!.description,
      role: this.participant!.role,
      country: this.countryInfo
    }

    const dialog = this.participantDialog.open(ParticipantInteractDialogComponent, {
      closeOnNavigation: true,
      disableClose: true,
      enterAnimationDuration: '500ms',
      data: participantViewModel
    })

    dialog.afterClosed().subscribe(data => {
      if (data && data.closeMode === ModalCloseMode.ClosedAfterAction) {
        window.location.reload()
      }
    });
  }

  handleError(err: any, _caught: Observable<any>): any {
    this.lastRequestError = err;
    return err;
  }

  getFullName(): string {
    return `${this.participant?.name} ${this.participant?.surname}`
  }

  getAge(): number {
    if (!this.participant) return 0

    const now = new Date();
    const participantBirthday = new Date(this.participant!.birthday ?? '')

    const milliSecondsInYear = 365 * 24 * 60 * 60 * 1000

    return Math.floor(((now.getTime() - participantBirthday.getTime())) / milliSecondsInYear)
  }

  getParticipantRoleString(): string {
    return ParticipantRole[this.participant!.role]
  }

  accountCanModify() {
    return this.account.role === 'Moderator'
  }
  
  private getCountryInfo(): void {
    this.countryService.get(this.participant!.country)
      .pipe(
        catchError(this.handleError)
      )
      .subscribe(country => {
        this.countryInfo = country
      })
  }

  private loadParticipant() {
    if (history.state.participant) {

      this.participant = history.state.participant.info
      this.photo = history.state.participant.photo
      this.getCountryInfo()
    } else this.getParticipant()
  }

  private getParticipant() {
    const id = this.route.snapshot.paramMap.get('participantId')!

    this.participantRequestInProgress = true
    this.participantService.getById(id)
    .pipe(
      catchError(this.handleError),
      finalize(() => this.participantRequestInProgress = false)
    )
    .subscribe(value => {
      if (value instanceof ErrorModel) return
      
      this.participant = value
      this.loadPicture()
      this.getCountryInfo()
    })
  }

  private loadPicture() {
    this.pictureRequestIsInProgress = true

    this.participantService.getPicture(this.participant!.id)
    .pipe(
      catchError(this.handleError),
      finalize(() => this.pictureRequestIsInProgress = false)
    )
    .subscribe(value => {
      if (value instanceof ErrorModel) return

      createImageFromBlob(value, (imageUrl) => this.photo = imageUrl)
    })
  }

}
