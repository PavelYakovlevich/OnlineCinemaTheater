import { Component, Input, OnInit } from '@angular/core';
import { catchError, finalize } from 'rxjs';
import { AgeRating } from 'src/app/models/media/age-rating';
import { MediaModel } from 'src/app/models/media/media';
import { createImageFromBlob } from 'src/app/services/infrastructure/common';
import { MediaInfoService } from 'src/app/services/media/media-info.service';

@Component({
  selector: 'app-media-card',
  templateUrl: './media-card.component.html',
  styleUrls: ['./media-card.component.scss']
})
export class MediaCardComponent implements OnInit {
  @Input() media: MediaModel = {
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
    country: '',
    participants: [],
    genres: []
  }

  picture: any = null
  isPictureLoading = true

  constructor(
    private mediaService: MediaInfoService
  ) { }

  ngOnInit(): void {
    this.getMediaPicture()
  } 

  getIssueDate(): Date {
    return new Date(this.media.issueDate)
  }

  getAgeRating() {
    return AgeRating[this.media.ageRating]
  }

  getAgeRatingColor() {
    switch(this.media.ageRating) {
      case AgeRating.GeneralAudiences: return 'primary'
      case AgeRating.ParentalGuidanceSuggested: return 'warning'
      case AgeRating.ParentsStronglyCautioned: return 'red'
      case AgeRating.AdultsOnly: return 'red'
      case AgeRating.GeneralAudiences: return 'green'
    }

    return 'primary'
  }

  private getMediaPicture() {
    this.isPictureLoading = true;

    this.mediaService.getPicture(this.media.id)
      .pipe(
        catchError(async (_err, _caught) => null),
        finalize(() => {
          this.isPictureLoading = false;
        })
      )
      .subscribe(data => {
        if (data) {
          createImageFromBlob(data, (image) => {
            this.picture = image
          });
        }
      });
  }
}
