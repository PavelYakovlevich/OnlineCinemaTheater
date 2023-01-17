import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormControl } from '@angular/forms';
import { map, Observable, of } from 'rxjs';
import { GenreModel } from 'src/app/models/media/genre';
import { GenreService } from 'src/app/services/media/genre.service';

@Component({
  selector: 'app-genre-select',
  templateUrl: './genre-select.component.html',
  styleUrls: ['./genre-select.component.scss']
})
export class GenreSelectComponent implements OnInit {
  public genres: GenreModel[] = []
  public filteredGenres: Observable<GenreModel[]> = of([])
  public genreControl = new FormControl('')
  
  @Output()
  public onGenreWasSelected = new EventEmitter()
  
  constructor(
    private genreService: GenreService
  ) { }

  ngOnInit(): void {
    this.loadAllGenres()
    
    this.filteredGenres = this.genreControl.valueChanges.pipe(
      map(value => this.filter(value ?? '')),
    );
  }

  onGenreWasClicked(genre: GenreModel): void {
    this.onGenreWasSelected.emit(genre)
  }

  private loadAllGenres(): void {
    this.genreService.getAll()
      .subscribe(genres => this.genres = genres)
  }

  private filter(value: string): GenreModel[] {
    const filterValue = value.toLowerCase();
    return this.genres.filter(genre => genre.name.toLowerCase().includes(filterValue));
  }
}
