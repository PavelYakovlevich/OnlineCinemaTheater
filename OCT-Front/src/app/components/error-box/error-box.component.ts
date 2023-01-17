import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ErrorModel } from 'src/app/models/infrastructure/ErrorModel';
import { Nullable } from 'src/app/services/infrastructure/common';

@Component({
  selector: 'app-error-box',
  templateUrl: './error-box.component.html',
  styleUrls: ['./error-box.component.scss']
})
export class ErrorBoxComponent implements OnInit {
  @Input('error')
  public errorModel: Nullable<ErrorModel> = null

  @Output('onClose')
  public onCloseBtnWasClicked = new EventEmitter()

  constructor() { }

  ngOnInit(): void {
  }

  onCloseErrorAlertBtnClicked() {
    this.errorModel = null
    this.onCloseBtnWasClicked.emit()
  }
}
