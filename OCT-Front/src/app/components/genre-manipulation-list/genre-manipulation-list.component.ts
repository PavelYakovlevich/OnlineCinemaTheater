import { Component, Input, OnInit } from '@angular/core';
import { FormControl, Validators as NgValidators } from '@angular/forms';
import { catchError, finalize, of } from 'rxjs';
import { ErrorModel } from 'src/app/models/infrastructure/ErrorModel';
import { GenreModel } from 'src/app/models/media/genre';
import { Validators } from 'src/app/services/infrastructure/validators';
import { GenreService } from 'src/app/services/media/genre.service';

@Component({
  selector: 'app-genre-manipulation-list',
  templateUrl: './genre-manipulation-list.component.html',
  styleUrls: ['./genre-manipulation-list.component.scss']
})
export class GenreManipulationListComponent implements OnInit {
  genres: GenreModel[] = []
  requestInProgress = false

  genreControl = new FormControl('', [NgValidators.required, Validators.notWhitespace])

  @Input()
  canDeleteGenres = false

  @Input()
  canCreateGenres = false
  
  constructor(
    private genreService: GenreService
  ) {
   }

  ngOnInit(): void {
    this.loadGenres()
  }

  onDeleteBtnWasClicked(genre: GenreModel) {
    this.requestInProgress = true

    this.genreService.delete(genre.id)
    .pipe(
      catchError((err, _caught) => of(err)),
      finalize(() => this.requestInProgress = false)
    )
    .subscribe(result => {
      if (result instanceof ErrorModel) return

      this.genres = this.genres.filter(g => g.id !== genre.id)
    })
  }

  onCreateGenreBtnWasClicked() {
    const genreName = this.genreControl.value?.trim()

    this.requestInProgress = true

    this.genreService.create({ name: genreName!})
    .pipe(
      catchError((err, _caught) => of(err)),
      finalize(() => this.requestInProgress = false)
    )
    .subscribe(result => {
      if (result instanceof ErrorModel) return

      this.genres.push({
        id: result,
        name: genreName!
      })

      this.genreControl.reset()
    })
  }

  getErrorMessage(): string {
    if(this.genreControl.hasError('required')) return 'Value is required'

    return 'Value is invalid'
  }

  private loadGenres() {
    this.requestInProgress = true

    this.genreService.getAll()
      .pipe(
        catchError((err, _caught) => of(err)),
        finalize(() => this.requestInProgress = false)
      )
      .subscribe(result => {
        if (result instanceof ErrorModel) return

        this.genres = result
      })
  }
}
