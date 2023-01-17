import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl } from '@angular/forms';
import { map, Observable, startWith } from 'rxjs';
import { CountryModel } from 'src/app/models/infrastructure/country';
import { GenreModel } from 'src/app/models/media/genre';
import { MediaFiltersModel } from 'src/app/models/media/media';
import { CountryService } from 'src/app/services/infrastructure/country.service';
import { GenreService } from 'src/app/services/media/genre.service';

@Component({
  selector: 'app-media-filter',
  templateUrl: './media-filter.component.html',
  styleUrls: ['./media-filter.component.scss']
})
export class MediaFilterComponent implements OnInit {
  public genres: GenreModel[] = []

  public countries: CountryModel[] = []
  public filteredCountries!: Observable<CountryModel[]>

  public genreControl = new FormControl()
  public countryControl = new FormControl()

  @Output() public onFiltersChanged = new EventEmitter<MediaFiltersModel>();
  
  public filters: MediaFiltersModel = {
    country: null,
    genres: [],
    nameStartsWith: null,
    isTvSerias: null,
  };

  constructor(
    private genreService: GenreService,
    private countriesService: CountryService
  ) { }

  ngOnInit(): void {
    this.getAllGenres()
    this.getAllCountries()

    this.filteredCountries = this.countryControl.valueChanges.pipe(
      map(value => this.filter(value)),
    );
  }

  onGenreWasClicked(genre: GenreModel): void {
    this.filters.genres = [];
    this.filters.genres.push(genre.name)

    this.onFiltersChanged.emit(this.filters)
  }

  onCountryWasChagned(country: CountryModel): void {
    this.filters.genres = [];
    this.filters.country = country.cioc

    this.onFiltersChanged.emit(this.filters)
  }

  private getAllCountries() {
    this.countriesService.getAll()
      .subscribe(countries => this.countries = countries)
  }

  private getAllGenres() {
    this.genreService.getAll()
      .subscribe(genres => this.genres = genres)
  }

  private filter(value: string): CountryModel[] {
    const filterValue = value.toLowerCase();

    return this.countries.filter(country => country.name.toLowerCase().includes(filterValue));
  }
}
