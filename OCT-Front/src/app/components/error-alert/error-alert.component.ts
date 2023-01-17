import { Component, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { ErrorModel } from 'src/app/models/infrastructure/ErrorModel';
import { Nullable } from 'src/app/services/infrastructure/common';
import { ErrorService } from 'src/app/services/infrastructure/error.service';

@Component({
  selector: 'app-error-alert',
  templateUrl: './error-alert.component.html',
  styleUrls: ['./error-alert.component.scss']
})
export class ErrorAlertComponent implements OnInit {
  public error$: Subject<Nullable<ErrorModel>>; 

  constructor(
    public errorService: ErrorService
  ) {
    this.error$ = errorService.error$
  }

  ngOnInit(): void {
  }

  onCloseErrorAlertBtnClicked() {
    this.errorService.clear()
  }

}
