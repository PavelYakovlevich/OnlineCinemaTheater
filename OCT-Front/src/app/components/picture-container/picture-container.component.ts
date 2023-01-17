import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Nullable } from 'src/app/services/infrastructure/common';

@Component({
  selector: 'app-picture-container',
  templateUrl: './picture-container.component.html',
  styleUrls: ['./picture-container.component.scss']
})
export class PictureContainerComponent implements OnInit {
  @Input()
  pictureUrl: Nullable<string> = null

  @Input()
  canDelete = false

  @Input()
  canModify = false

  @Input()
  requestInProgress = false
  
  @Output()
  onFileSelected = new EventEmitter<Event>()

  @Output()
  onDeleteFile = new EventEmitter()

  pictureIsFocused = false

  constructor() { }

  ngOnInit(): void {
  }

  onUploadFileBtnWasClicked() {
    document.getElementById('uploadPicture')!.click()
  }

  onDeletePictureBtnWasClicked() {
    this.onDeleteFile.emit()
  }

  onPictureWasSelected(event: Event) {
    this.onFileSelected.emit(event)
  }

  onPictureFocusEvent() {
    this.pictureIsFocused = !this.pictureIsFocused
  }
}
