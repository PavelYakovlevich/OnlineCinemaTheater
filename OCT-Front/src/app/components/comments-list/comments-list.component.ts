import { Component, Input, OnInit } from '@angular/core';
import { FormControl, Validators,  } from '@angular/forms';
import { catchError, finalize } from 'rxjs';
import { AccountInfoModel } from 'src/app/models/AccountInfoModel';
import { CommentModel } from 'src/app/models/comment/CommentModel';
import { CreateCommentModel } from 'src/app/models/comment/CreateCommentModel';
import { ErrorModel } from 'src/app/models/infrastructure/ErrorModel';
import { MediaModel } from 'src/app/models/media/media';
import { UserModel } from 'src/app/models/user/user';
import { CommentService } from 'src/app/services/comment/comment.service';
import { getAccountFromCookies, Nullable, userFinishedSetup } from 'src/app/services/infrastructure/common';
import { LocalStorageService } from 'src/app/services/infrastructure/local-storage.service';
import { Validators as CustomValidators } from 'src/app/services/infrastructure/validators';

@Component({
  selector: 'app-comments-list',
  templateUrl: './comments-list.component.html',
  styleUrls: ['./comments-list.component.scss']
})
export class CommentsListComponent implements OnInit {
  @Input()
  public media!: MediaModel

  public commentTextControl = new FormControl('', [Validators.required, CustomValidators.notWhitespace])

  public account!: AccountInfoModel
  public comments: CommentModel[] = []

  public commentsAreLoading = false;
  public createCommentInProgress = false

  public canLoadComments = true;

  private readonly commentsLoadCount = 5;

  public lastError: Nullable<ErrorModel> = null

  private user!: UserModel
  
  constructor(
    private commentService: CommentService,
    private storageService: LocalStorageService
  ) { }

  ngOnInit(): void {
    this.user = this.storageService.getUser()!
    this.setAccountFromCookies()
    this.getComments()
  }

  getComments() {
    if (!this.canLoadComments) return

    this.commentsAreLoading = true
    this.commentService.getAll(this.comments.length, this.commentsLoadCount, this.media.id)
    .subscribe(comments => {
      this.comments = this.comments.concat(comments)
      this.canLoadComments = comments?.length === this.commentsLoadCount
      this.commentsAreLoading = false
    })
  }

  getErrorMessage(control: FormControl): string {
    return control.errors ? 'Comment is empty' : ''
  }

  onCommentWasDeleted(comment: CommentModel) {
    this.comments = this.comments.filter(c => c.id !== comment.id)
  }

  onCreateCommentBtnClicked(): void {
    if (this.commentTextControl.invalid) return

    const commentModel: CreateCommentModel = {
      userId: this.account.userId,
      text: this.commentTextControl.value!
    }

    this.commentService.post(this.media.id, commentModel)
      .pipe(
        catchError(async (err, _) => this.handleError(err)),
        finalize(() => this.createCommentInProgress = false)
      )
      .subscribe(_ => {
        this.comments = []
        this.canLoadComments = true
        this.getComments()
        this.commentTextControl.reset()
      })
  }

  userFinishedSetup(): boolean {
    return userFinishedSetup(this.user)
  }

  private handleError(error: ErrorModel) {
    this.lastError = error
  }

  private setAccountFromCookies(): void {
    const account = getAccountFromCookies();

    if (account) {
      this.account = account
    }
  }
}
