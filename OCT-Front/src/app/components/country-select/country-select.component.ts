import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormControl } from '@angular/forms';
import { map, Observable } from 'rxjs';
import { CountryModel } from 'src/app/models/infrastructure/country';
import { CountryService } from 'src/app/services/infrastructure/country.service';
import { Nullable } from 'src/app/services/infrastructure/common';

@Component({
  selector: 'app-country-select',
  templateUrl: './country-select.component.html',
  styleUrls: ['./country-select.component.scss']
})
export class CountrySelectComponent implements OnInit {
  public countries: CountryModel[] = []
  public filteredCountries!: Observable<CountryModel[]>
  public countryControl = new FormControl('')
  
  @Input()
  public country: Nullable<CountryModel> = null

  @Output()
  public onCountryWasSelected = new EventEmitter()

  constructor(
    private countriesService: CountryService
  ) { }

  ngOnInit(): void {
    this.getAllCountries()
    
    this.filteredCountries = this.countryControl.valueChanges.pipe(
      map(value => this.filter(value ?? '')),
    );
  }

  onCountrySelectionValueWasClicked(): void {
    this.country = null
    this.onCountryWasSelected.emit(null)
  }

  onCountryWasChagned(country: CountryModel): void {
    this.country = country
    this.countryControl.setValue('')
    this.onCountryWasSelected.emit(country)
  }

  private getAllCountries() {
    this.countriesService.getAll()
      .subscribe(countries => this.countries = countries)
  }

  private filter(value: string): CountryModel[] {
    const filterValue = value.toLowerCase();
    return this.countries.filter(country => country.name.toLowerCase().includes(filterValue));
  }
}
