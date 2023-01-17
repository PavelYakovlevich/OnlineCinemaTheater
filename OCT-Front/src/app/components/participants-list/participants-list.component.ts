import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { catchError, of } from 'rxjs';
import { ParticipantFilters, ParticipantModel, ParticipantRole } from 'src/app/models/media/participant';
import { createImageFromBlob } from 'src/app/services/infrastructure/common';
import { ParticipantService } from 'src/app/services/media/participant.service';

@Component({
  selector: 'app-participants-list',
  templateUrl: './participants-list.component.html',
  styleUrls: ['./participants-list.component.scss']
})
export class ParticipantsListComponent implements OnInit {
  @Input()
  participants: ParticipantModel[] = []

  @Input()
  linksDisabled: boolean = true

  @Input()
  modifiable: boolean = false

  @Output()
  onParticipantWasDeleted = new EventEmitter<ParticipantModel>()

  @Output()
  onParticipantWasAdded = new EventEmitter<ParticipantModel>()

  participantsPictures: { [id: string]: any } = {}
  participantControl = new FormControl('')
  selectableParticipants: ParticipantModel[] = []
  participantsCore: ParticipantModel[] = [] 

  constructor(
    private participantsService: ParticipantService,
    private router: Router
  ) {
  }

  ngOnInit(): void {
    this.participantsCore = this.participantsCore.concat(this.participants)
    this.participantControl.valueChanges
      .subscribe(_ => this.loadParticipants())
    this.getParticipantsPictures()
  }
    
  get producers(): ParticipantModel[] {
    return this.filterParticipants(p => p.role == ParticipantRole.Producer)
  }

  get actors(): ParticipantModel[] {
    return this.filterParticipants(p => p.role == ParticipantRole.Actor)
  }

  onParticipantWasClicked(participant: ParticipantModel): void {
    if (this.linksDisabled) return

    this.router.navigate([`/participant/${participant.id}`], {
      state: { participant: { info: participant, photo: this.participantsPictures[participant.id] } }
    })
  }

  onParticipantOptionWasClicked(participant: ParticipantModel): void {
    if (!this.participantsCore.every(p => p.id !== participant.id)) return

    this.participantsCore.push(participant)
    this.onParticipantWasAdded.emit(participant)

    this.participantsService.getPicture(participant.id)
    .pipe(
      catchError((_err, _caught) => of(null))
    )
    .subscribe(picture => {
      if (!picture) return

      createImageFromBlob(picture, (pictureUrl) => this.participantsPictures[participant.id] = pictureUrl)
    })
  }

  onDeleteParticipantBtnWasClicked(participant: ParticipantModel): void {
    this.participantsCore = this.participantsCore.filter(p => p.id !== participant.id)
    this.onParticipantWasDeleted.emit(participant)
  }

  getFullName(participant: ParticipantModel): string {
    if (participant) {
      return `${participant.name} ${participant.surname}`
    }

    return ''
  }

  getParticipantsPictures() {
    this.participants.forEach(p => 
      this.participantsService.getPicture(p.id)
        .subscribe(picture => {
          if (picture) {
            createImageFromBlob(picture, (pictureString) => this.participantsPictures[p.id] = pictureString)
          }
        }))
  }

  private loadParticipants(): void {
    const controlValue = this.participantControl.value?.trim() ?? ''

    if (controlValue.length < 2) return 

    const participantCredentials = controlValue.split(' ')
    const name = participantCredentials[0]
    const surname = participantCredentials[1] ?? ''

    const filters: ParticipantFilters = {
      nameStartsWith: name,
      surnameStartsWith: surname,
      limit: null,
      offset: null,
      role: null
    }

    this.participantsService.get(filters)
      .subscribe(participants => {
        this.selectableParticipants = participants
      })
  }

  private filterParticipants(predicate: (participant: ParticipantModel) => boolean): ParticipantModel[] {
    return this.participantsCore
      .filter(predicate)
  }
}
