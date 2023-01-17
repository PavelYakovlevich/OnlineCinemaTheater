import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { catchError, debounceTime, finalize, of } from 'rxjs';
import { ErrorModel } from 'src/app/models/infrastructure/ErrorModel';
import { ParticipantFilters, ParticipantModel, ParticipantRole } from 'src/app/models/media/participant';
import { createImageFromBlob, Nullable } from 'src/app/services/infrastructure/common';
import { ParticipantService } from 'src/app/services/media/participant.service';

@Component({
  selector: 'app-search-participant',
  templateUrl: './search-participant.component.html',
  styleUrls: ['./search-participant.component.scss']
})
export class SearchParticipantComponent implements OnInit {
  fullNameControl: FormControl = new FormControl('')

  participants: ParticipantModel[] = []

  lastError: Nullable<ErrorModel> = null
  requestInProgress = false

  participantsPictures: { [id: string]: any } = {}

  constructor(
    private dialogRef: MatDialogRef<SearchParticipantComponent>,
    private participantService: ParticipantService
  ) {
    this.fullNameControl.valueChanges.pipe(
      debounceTime(1000)
    )
    .subscribe(fullName => {
      this.loadParticipants(fullName)
    })
  }

  ngOnInit(): void {
  }

  onParticipantWasClicked(participant: ParticipantModel) {
    this.dialogRef.close({ participant: { info: participant, photo: this.participantsPictures[participant.id] } })
  }

  getFullName(participant: ParticipantModel): string {
    return `${participant.name} ${participant.surname}`
  }

  private loadParticipants(fullName: string) {
    if(fullName.trim().length < 3) return

    const [name, lastName] = fullName.split(' ')

    const filtersObject = this.createFiltersObject(name, lastName)

    this.requestInProgress = true
    this.participantService.get(filtersObject)
      .pipe(
        catchError((err, _caught) => {
          this.lastError = err
          return of(err)
        }),
        finalize(() => this.requestInProgress = false)
      )
      .subscribe(values => {
        if (values instanceof ErrorModel) return

        this.participants = values
        this.participants.forEach(p => this.loadParticipantPicture(p))        
      })
  }

  private loadParticipantPicture(participant: ParticipantModel): void {
    this.participantService.getPicture(participant.id)
    .pipe(
      catchError((err, _caught) => {
        this.lastError = err
        return of(err)
      })
    )
    .subscribe(result => {
      if (result instanceof ErrorModel) return
      
      createImageFromBlob(result, (image) => this.participantsPictures[participant.id] = image)
    })
  }

  private createFiltersObject(name?: string, lastName?: string, role?: ParticipantRole, limit?: number, offset?: number): ParticipantFilters {
    return {
        nameStartsWith: name ?? null,
        surnameStartsWith: lastName ?? null,
        role: role ?? null,
        limit: limit ?? null,
        offset: offset ?? null
    }
  }
}
