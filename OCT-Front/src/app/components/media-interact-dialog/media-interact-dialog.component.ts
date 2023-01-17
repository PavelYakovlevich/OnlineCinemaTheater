import { Component, Inject, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators as NgValidators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CountryModel } from 'src/app/models/infrastructure/country';
import { AgeRating } from 'src/app/models/media/age-rating';
import { GenreModel } from 'src/app/models/media/genre';
import { ParticipantModel } from 'src/app/models/media/participant';
import { Nullable } from 'src/app/services/infrastructure/common';
import { Validators } from 'src/app/services/infrastructure/validators';
import { createPatch, Operation } from 'rfc6902'
import { catchError, debounceTime, finalize, Observable, of, throwError } from 'rxjs';
import { MediaInfoService } from 'src/app/services/media/media-info.service';
import { ErrorModel } from 'src/app/models/infrastructure/ErrorModel';
import { ModalCloseMode } from 'src/app/models/infrastructure/modal';
import { Router } from '@angular/router';
import { MediaViewModel } from 'src/app/models/media/media';

@Component({
  selector: 'app-media-interact-dialog',
  templateUrl: './media-interact-dialog.component.html',
  styleUrls: ['./media-interact-dialog.component.scss']
})
export class MediaInteractDialogComponent implements OnInit {
  media: MediaViewModel = this.createNewMediaModel()
  private sourceMedia!: MediaViewModel

  private lastRequestError: Nullable<ErrorModel> = null
  
  isOpenedInModificationMode = true;
  requestInProgress = false
  mediaDialogFormGroup! :FormGroup

  constructor(
    public dialogRef: MatDialogRef<MediaInteractDialogComponent>,
    private mediaInfoService: MediaInfoService,
    private router: Router,
    @Inject(MAT_DIALOG_DATA) public mediaModel: Nullable<MediaViewModel>,
    ) {
    if (mediaModel) {
      this.sourceMedia = mediaModel
      this.media = this.createMediaCopy(mediaModel)
    }

    this.isOpenedInModificationMode = mediaModel !== null
    this.initializeForm();
  }
  
  ngOnInit(): void {
  }

  private initializeForm() {
    this.mediaDialogFormGroup = new FormGroup({
      "name": new FormControl(this.media.name, Validators.mediaName),
      "issueDate": new FormControl(this.media.issueDate, NgValidators.required),
      "budget": new FormControl(this.media.budget, Validators.mediaBudget),
      "aid": new FormControl(this.media.aid, Validators.mediaAid),
      "ageRating": new FormControl(this.media.ageRating),
    });

    const debounceTimeValue = 500;

    this.mediaDialogFormGroup.get('name')!.valueChanges
    .pipe(debounceTime(debounceTimeValue))
    .subscribe((changes) => {
      this.media.name = changes
    })

    this.mediaDialogFormGroup.get('issueDate')!.valueChanges
    .pipe(debounceTime(debounceTimeValue))
    .subscribe((changes) => {
      this.media.issueDate = new Date(changes).toISOString()
    })

    this.mediaDialogFormGroup.get('budget')!.valueChanges
    .pipe(debounceTime(debounceTimeValue))
    .subscribe((changes) => {
      const number = Number.parseInt(changes)

      if (!Number.isNaN(number)) this.media.budget = Number(changes)
      else this.media.budget = null
    })

    this.mediaDialogFormGroup.get('aid')!.valueChanges
    .pipe(debounceTime(debounceTimeValue))
    .subscribe((changes) => {
      const number = Number.parseInt(changes)

      if (!Number.isNaN(number)) this.media.aid = Number(changes)
      else this.media.aid = null
    })

    this.mediaDialogFormGroup.get('ageRating')!.valueChanges
    .pipe(debounceTime(debounceTimeValue))
    .subscribe((changes) => this.media.ageRating = changes)
  }
  
  onCountryWasSelected(country: Nullable<CountryModel>): void {
    this.media.country = country
  }

  onDeleteGenreBtnWasClicked(genre: GenreModel): void {
    this.media.genres = this.media.genres.filter(g => g !== genre)
  }

  onGenreWasSelected(genre: GenreModel): void {
    const genreName = genre.name.toUpperCase()

    if (this.media.genres.every(g => g.name !== genreName)) {
      this.media.genres.push(genre)
    }
  }

  onParticipantWasAdded(participant: ParticipantModel): void {
    this.media.participants.push(participant)
  } 

  onParticipantWasDeleted(participant: ParticipantModel): void {
    this.media.participants = this.media.participants.filter(p => p.id !== participant.id)
  }

  onSaveBtnWasClicked(): void {
    this.requestInProgress = true

    if (this.isOpenedInModificationMode) {
      this.patchMedia()
      return
    }
    
    this.createMedia()
  }

  controlIsInvalid(controlName: string): boolean {
    const control = this.mediaDialogFormGroup.get(controlName)!
    return control.invalid
  }

  allControlsAreValid(): boolean {
    let result = true

    Object.keys(this.mediaDialogFormGroup.controls).forEach(key => {
      result = result && !this.mediaDialogFormGroup.get(key)!.invalid
    })

    return result && this.media.country != null && this.media.participants.length !== 0 && this.media.genres.length !== 0
  }

  getAgeRatingString(ageRating: AgeRating) {
    return AgeRating[ageRating]
  }

  getErrorMessage(controlName: string): string {
    const control = this.mediaDialogFormGroup.get(controlName)!

    if (control.hasError('required')) return 'Field value is required'
    if (control.hasError('maxlength')) return 'Field value is too long'
    
    return 'Field value is invalid'
  }

  getAgeRatings(): AgeRating[] {
    const result: AgeRating[] = []

    Object.values(AgeRating).forEach(v => {
      if (typeof v !== 'string') result.push(v)
    })
  
    return result;
  }

  private createMediaCopy(mediaModel: MediaViewModel): MediaViewModel {
    return {...mediaModel, participants: mediaModel.participants.map(p => {return {...p}}), genres: mediaModel.genres.map(g => {return {...g}})}
  }

  private createNewMediaModel(): MediaViewModel {
    return {
      id: '',
      name: '',
      budget: null,
      aid: null,
      description: '',
      issueDate: '',
      isFree: false,
      ageRating: AgeRating.GeneralAudiences,
      isTvSerias: false,
      isVisible: false,
      country: null,
      participants: [],
      genres: []
    }
  }
    
  private patchMedia() {
    const patchDocument = this.createPatchDocument()

    if (patchDocument.length === 0) {
      this.requestInProgress = false
      return
    }

    this.mediaInfoService.patch(this.media.id, patchDocument)
    .pipe(
      catchError(this.handleError),
      finalize(() => this.requestInProgress = false)
    )
    .subscribe(result => {
      if (result instanceof ErrorModel) return
      this.dialogRef.close({ closeMode: ModalCloseMode.ClosedAfterAction })
    })
  }

  private createPatchDocument(): Operation[] {
    const preparedUpdatedMedia = this.createMediaRequestModel(this.media)
    const preparedSourceMedia = this.createMediaRequestModel(this.sourceMedia)

    const patchDocument = createPatch(preparedSourceMedia, preparedUpdatedMedia)

    return patchDocument
  }

  private createMediaRequestModel(media: MediaViewModel): any {
    const mediaCopy: any = {...media}
    mediaCopy.participants = media.participants.map(p => p.id)
    mediaCopy.genres = media.genres.map(g => g.id)
    mediaCopy.country = media.country?.cioc

    return mediaCopy
  }
  
  private createMedia() {
    const mediaModel = this.createMediaRequestModel(this.media)

    this.mediaInfoService.create(mediaModel)
    .pipe(
      catchError(this.handleError),
      finalize(() => this.requestInProgress = false) 
    )
    .subscribe(result => {
      if (result instanceof ErrorModel) return

      this.dialogRef.close({ closeMode: ModalCloseMode.ClosedAfterAction, id: result })
    })
  }

  private handleError<T>(error: any, _caught: Observable<T>): Observable<any> {
    this.lastRequestError = error
    return error
  }
}
