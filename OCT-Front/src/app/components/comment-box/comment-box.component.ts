import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { MatExpansionPanel } from '@angular/material/expansion';
import { catchError, finalize } from 'rxjs';
import { AccountInfoModel } from 'src/app/models/AccountInfoModel';
import { AccountRole } from 'src/app/models/authentication/AccountRole';
import { CommentModel } from 'src/app/models/comment/CommentModel';
import { ErrorModel } from 'src/app/models/infrastructure/ErrorModel';
import { CommentService } from 'src/app/services/comment/comment.service';
import { createImageFromBlob, errorHandler, getAccountFromCookies, Nullable } from 'src/app/services/infrastructure/common';
import { Validators as CustomValidators} from 'src/app/services/infrastructure/validators';
import { UserService } from 'src/app/services/user/user.service';

@Component({
  selector: 'app-comment-box',
  templateUrl: './comment-box.component.html',
  styleUrls: ['./comment-box.component.scss']
})
export class CommentBoxComponent implements OnInit {
  @Input()
  public comment!: CommentModel

  @Output()
  public onCommentWasDeleted = new EventEmitter<CommentModel>()

  public commentTextArea = new FormControl(this.comment?.text ?? '', [Validators.required, CustomValidators.notWhitespace])

  public account: Nullable<AccountInfoModel> = null
  public deleteIconWasClicked = false
  public userPhoto: any
  public requestInProgress = false
  public isChanging = false 

  public lastError: Nullable<ErrorModel> = null

  constructor(
    private userService: UserService,
    private commentService: CommentService
  ) { }

  ngOnInit(): void {
    this.account = getAccountFromCookies()
    this.getUserPhoto()
    this.commentTextArea.setValue(this.comment.text)
  }

  getUserPhoto() {
    this.userService.getPicture(this.comment.userId)
      .pipe(
        catchError(async (_err, _caught) => null)
      )
      .subscribe(image => {
        if (image) {
          createImageFromBlob(image, (imageString) => {
            this.userPhoto = imageString
          })
        }
      })
  }

  getUserName(): string {
    return `${this.comment.user.name} ${this.comment.user.surname}`
  }

  userCanMakeActions(comment: CommentModel): boolean {
    return this.account?.role !== AccountRole.User || this.account?.userId == comment.userId
  }

  onDeleteIconWasClicked(panel?: MatExpansionPanel) {
    panel?.toggle()
    this.deleteIconWasClicked = !this.deleteIconWasClicked
  }

  onCommentDeleteBtnWasClicked(): void {
    this.requestInProgress = true

    this.commentService.delete(this.comment)
      .pipe(
        catchError((err, caught) => errorHandler(err, caught, () => this.handleError(err))),
        finalize(() => this.requestInProgress = false)
      )
      .subscribe(_ => {
        this.onCommentWasDeleted.emit(this.comment)
      })
  }

  onCancelBtnWasClicked(): void {
    this.isChanging = false
  }

  commentCanBeModified(): boolean {
    return this.account?.role === "User" && !this.isChanging
  }

  commentCanBeDeleted(): boolean {
    return !this.deleteIconWasClicked && !this.isChanging
  }

  commentCanBeSaved(): boolean {
    return this.isChanging
  }

  onChangeBtnWasClicked(): void {
    this.isChanging = true
  }

  getErrorMessage(control: FormControl): string {
    return control.errors ? 'Comment is empty' : ''
  }

  onSaveCommentBtnWasClicked(): void {
    this.requestInProgress = true

    const commentCopy = {...this.comment}
    commentCopy.text = this.commentTextArea.value!

    this.commentService.update(commentCopy)
      .pipe(
        catchError(async (err, _) => this.handleError(err)),
        finalize(() => this.requestInProgress = false)
      )
      .subscribe(_ => {
        this.comment = commentCopy
        this.isChanging = false
      })
  }

  private handleError(error: ErrorModel) {
      this.lastError = error
  }
}
