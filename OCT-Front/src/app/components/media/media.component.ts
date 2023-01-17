import { Component, OnInit } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { catchError, finalize, Observable, of } from 'rxjs';
import { AccountInfoModel } from 'src/app/models/AccountInfoModel';
import { AccountRole } from 'src/app/models/authentication/AccountRole';
import { CountryModel } from 'src/app/models/infrastructure/country';
import { ErrorModel } from 'src/app/models/infrastructure/ErrorModel';
import { ModalCloseMode } from 'src/app/models/infrastructure/modal';
import { AgeRating } from 'src/app/models/media/age-rating';
import { MediaModel } from 'src/app/models/media/media';
import { CountryService } from 'src/app/services/infrastructure/country.service';
import { createImageFromBlob, getAccountFromCookies, Nullable } from 'src/app/services/infrastructure/common';
import { MediaInfoService } from 'src/app/services/media/media-info.service';
import { MediaService } from 'src/app/services/media/media.service';
import { MediaInteractDialogComponent } from '../media-interact-dialog/media-interact-dialog.component';

@Component({
  selector: 'app-media',
  templateUrl: './media.component.html',
  styleUrls: ['./media.component.scss']
})
export class MediaComponent implements OnInit {
  public media!: MediaModel
  public account!: AccountInfoModel
  public picture: any = null

  public trailer: any = null
  public trailerUrl: SafeResourceUrl = '' 

  public countryInfo: Nullable<CountryModel> = null

  public lastLoadMediaPictureError: Nullable<ErrorModel> = null
  public lastLoadMediaError: Nullable<ErrorModel> = null

  public mediaNameFormControl = new FormControl('', [Validators.required, Validators.maxLength(50)])
  public mediaPictureIsFocused = false
  public mediaPictureRequestInProgress = false
  public mediaTrailerRequestInProgress = false

  constructor(
    public changeMediaDialog: MatDialog,
    private mediaInfoService: MediaInfoService,
    private countryService: CountryService,
    private mediaService: MediaService,
    private sanitizer: DomSanitizer,
    private router: Router,
    private route: ActivatedRoute,
  ) { }

  ngOnInit(): void {
    this.setAccountFromCookies()
    this.loadMedia()
  }

  onDeleteMediaMenuItemWasClicked(): void {
    this.mediaInfoService.delete(this.media.id)
    .pipe(
      catchError(this.handleError)
    )
    .subscribe(_ => {
      history.state.media = null
      this.router.navigate(['/'])
    })
  }

  onDeleteMediaPictureBtnWasClicked() {
    this.mediaInfoService.deletePicture(this.media.id)
      .subscribe(_ => {
        this.picture = null
      })
  }

  onUploadMediaItemBtnWasClicked(name: string) {
    document.getElementById(name)!.click()
  }

  onMediaPictureFileWasSelected(event: Event) {
    this.onFileWasSelected(event, (file) => {
      this.mediaPictureRequestInProgress = true

      this.mediaInfoService.uploadPicture(this.media.id, file)
      .pipe(
        catchError(this.handleError),
        finalize(() => this.mediaPictureRequestInProgress = false)
      )
      .subscribe(_ => {
        this.picture = createImageFromBlob(file as Blob, (img) => this.picture = img)
      })
    })
  }

  onMediaTrailerFileWasSelected(event: Event) {
    this.onFileWasSelected(event, (file) => {
      this.mediaTrailerRequestInProgress = true

      this.mediaService.uploadTrailer(this.media.id, file)
      .pipe(
        catchError(this.handleError),
        finalize(() => this.mediaTrailerRequestInProgress = false)
      )
      .subscribe(_ => {
        window.location.reload()
      })
    })
  }

  onDeleteMediaTrailerBtnWasClicked() {
    this.mediaTrailerRequestInProgress = true

    this.mediaService.deleteTrailer(this.media.id)
    .pipe(
      catchError(this.handleError),
      finalize(() => this.mediaTrailerRequestInProgress = false)
    )
    .subscribe(_ => {
      window.location.reload()
    })
  }

  getAgeRating(): string {
    switch(this.media.ageRating) {
      case AgeRating.GeneralAudiences: return 'G';
      case AgeRating.ParentalGuidanceSuggested: return 'PG';
      case AgeRating.ParentsStronglyCautioned: return 'PG-13';
      case AgeRating.Restricted: return 'R';
      default: return 'NC-17';
    }
  }

  openChangeMediaDialog() {
    if (this.account.role !== AccountRole.Moderator) return

    const mediaViewModel =  {
      id: this.media.id,
      name: this.media.name,
      budget: this.media.budget,
      aid: this.media.aid,
      description: this.media.description,
      issueDate: this.media.issueDate,
      isFree: this.media.isFree,
      ageRating: this.media.ageRating,
      isTvSerias: this.media.isTvSerias,
      isVisible: this.media.isVisible,
      country: this.countryInfo,
      participants: this.media.participants,
      genres: this.media.genres
    }

    const dialog = this.changeMediaDialog.open(MediaInteractDialogComponent, {
      closeOnNavigation: true,
      disableClose: true,
      enterAnimationDuration: '500ms',
      data: mediaViewModel
    })

    dialog.afterClosed().subscribe(data => {
      if (data && data.closeMode === ModalCloseMode.ClosedAfterAction) {
        window.location.reload()
      }
    });
  }

  canModifyMedia(): boolean {
    return this.account.role !== "User"
  }

  private onFileWasSelected(event: Event, callback: (file: File) => void) {
    const fileInput = event.target as HTMLInputElement

    if (!fileInput.files?.length) return

    const file = fileInput.files[0]

    callback(file)
  }

  private setAccountFromCookies(): void {
    const account = getAccountFromCookies();

    if (account) {
      this.account = account
    }
  }

  private getCountryInfo(): void {
    this.countryService.get(this.media.country)
      .subscribe(country => {
        this.countryInfo = country
      })
  }

  private loadMedia(): void {
    this.loadMediaInfo()
      .subscribe(value => {
        if (!value) {
          this.router.navigate(['/'])
          return
        }

        this.media = value

        this.getCountryInfo()

        this.loadMediaPicture()
        this.loadTrailer()
      })
  }

  private handleError<T>(error: any, _caught: Observable<T>): Observable<any> {
    this.lastLoadMediaError = error
    return of(null)
  }

  private loadTrailer(): void {
    if (!this.media.id) throw new Error('Media id is not defined')

    this.mediaService.getTrailer(this.media.id)
      .subscribe(trailer => {
        if (trailer) {
          this.trailer = trailer
          this.trailerUrl = this.sanitizer.bypassSecurityTrustResourceUrl(window.URL.createObjectURL(trailer));
        }
      })
  }

  private loadMediaPicture(): void {
    this.mediaPictureRequestInProgress = true;

    if (history.state.media?.picture) {
      this.picture = history.state.media.picture
      this.mediaPictureRequestInProgress = false;
    return
    }

    if (!this.media.id) throw new Error('Media id is not defined')

    this.mediaInfoService.getPicture(this.media.id)
      .pipe(
        catchError(async (err, _caught) => this.lastLoadMediaPictureError = err),
        finalize(() => {
          this.mediaPictureRequestInProgress = false
        })
      )
      .subscribe(img => {
        this.createImageFromBlob(img, (value) => this.picture = value)
      })
  }

  private loadMediaInfo(): Observable<MediaModel> {
    if (history.state.media) return of(history.state.media.media)
      
    const mediaId = this.route.snapshot.paramMap.get('mediaId')
    if (!mediaId) {
      throw Error('Media id was not found')
    }

    return this.mediaInfoService.get(mediaId)
      .pipe(
        catchError(this.handleError)
      )
  }

  private createImageFromBlob(image: Blob, imageWasLoaded: (value: string | ArrayBuffer | null) => void) {
    createImageFromBlob(image, (result) => imageWasLoaded(result))
  }
}
