import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';

import { AccountInfoModel } from 'src/app/models/AccountInfoModel';
import { ModalCloseMode } from 'src/app/models/infrastructure/modal';
import { UserModel } from 'src/app/models/user/user';
import { getAccountFromCookies, logout, userFinishedSetup } from 'src/app/services/infrastructure/common';
import { LocalStorageService } from 'src/app/services/infrastructure/local-storage.service';
import { MediaInteractDialogComponent } from '../media-interact-dialog/media-interact-dialog.component';
import { ParticipantInteractDialogComponent } from '../participant-interact-dialog/participant-interact-dialog.component';
import { SearchParticipantComponent } from '../search-participant/search-participant.component';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  public accountInfo!: AccountInfoModel
  public user!: UserModel

  public searchByNameControl = new FormControl();

  constructor(
    private storageService: LocalStorageService,
    private router: Router,
    private modal: MatDialog
  ) {
    this.router.routeReuseStrategy.shouldReuseRoute = () => false
    this.user = this.storageService.getUser()!
  }

  ngOnInit(): void {
    this.setAccountFromCookies();
  }

  onLogoutBtnWasClicked() {
    logout()
    this.router.navigate(['/login'])
  }

  onSearchBtnWasClicked() {
    this.router.navigate(['./'], {
      queryParams: { mediaName: this.searchByNameControl.value }
    })
    .then(() => {
      window.location.reload();
    });
  }

  onCreateMediaMenuItemWasClicked() {
    this.openModal(MediaInteractDialogComponent, null, (data) => {
      if (data?.closeMode !== ModalCloseMode.ClosedAfterAction) return
      if (data.id) this.router.navigate([`/media/${data.id}`])
    })
  }

  onCreateParticipantMenuItemWasClicked() {
    this.openModal(ParticipantInteractDialogComponent, null, (data) => {
      if (data?.closeMode !== ModalCloseMode.ClosedAfterAction) return
      if (data.id) this.router.navigate([`/participant/${data.id}`])
    })
  }

  onSearchParticipantMenuItemWasClicked() {
    this.openModal(SearchParticipantComponent, null, (data) => {
      if (!data?.participant) return

      this.router.navigate([`/participant/${data.participant.info.id}`], {
        state: { participant: { info: data.participant.info, photo: data.participant.photo } }
      })
    })
  }

  getUserAccountMenuLabel(): string {
    if (userFinishedSetup(this.user)) return `${this.user.name} ${this.user.surname}`
    
    return this.accountInfo!.email
  }

  private openModal(modalPageContent: any, data: any, onClose?: (data: any) => any) {
    const dialog = this.modal.open(modalPageContent, {
      closeOnNavigation: true,
      disableClose: true,
      enterAnimationDuration: '500ms',
      data: data,
    })

    dialog.afterClosed().subscribe(data => {
      onClose?.(data)
    });
  }

  private setAccountFromCookies(): void {
    let account = getAccountFromCookies();

    if (!account) {
      this.router.navigate(['/login'])
      return;
    }

    this.accountInfo = account
  }
}
