import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MediaFiltersModel, MediaModel } from 'src/app/models/media/media';
import { ErrorService } from 'src/app/services/infrastructure/error.service';
import { MediaInfoService } from 'src/app/services/media/media-info.service';

@Component({
  selector: 'app-media-list',
  templateUrl: './media-list.component.html',
  styleUrls: ['./media-list.component.scss']
})
export class MediaListComponent implements OnInit {
  public medias: MediaModel[] = []
  public mediasAreLoading: boolean = true
  public loadErrorRaised: boolean = false
  public scrolledOnTop: boolean = true

  public lastMediasLoadedCount = -1;
  private readonly itemsInPage = 20;

  @Input() public filters: MediaFiltersModel = {
    country: null,
    genres: [],
    nameStartsWith: null,
    isTvSerias: null,
  };

  constructor(
    public errorService: ErrorService,
    private mediaService: MediaInfoService,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    this.medias = []
    this.parseQueryParams();
    this.loadMedias()
  }

  onScroll(): void {
    this.scrolledOnTop = false
    this.loadMedias()
  }

  onScrollOnTopBtrWasClicked(): void {
    window.scroll({ 
      top: 0, 
      left: 0, 
      behavior: 'smooth' 
    });

    this.scrolledOnTop = true
  }

  onFiltersWereChanged(filters: MediaFiltersModel): void {
    this.medias = []
    this.search(filters)
  }

  onLoadMoreLinkWasClicked() {
    this.loadMedias()
  }

  getSearchCriteriasString(): string {
    let result = 'All available media';

    if (this.filters.genres.length !== 0) {
      return this.filters.genres[0];
    }

    if (this.filters.nameStartsWith && this.filters.nameStartsWith !== '') {
      return `Search results for '${this.filters.nameStartsWith}'`;
    }

    return result;
  }

  canLoadMore(): boolean {
    return this.lastMediasLoadedCount == this.itemsInPage
  }
  
  private search(filters: MediaFiltersModel): void {
    this.filters = filters
    this.clearMediaSearchData()
    this.loadMedias()
  }

  private parseQueryParams() {
    this.route.queryParams.subscribe(params => {
      if (params['mediaName']) {
        this.filters.nameStartsWith = params['mediaName']
      }
    })
  }

  private loadMedias() {
    if (this.lastMediasLoadedCount === 0) return

    this.mediasAreLoading = true
    this.mediaService.getAll(this.medias.length, this.itemsInPage, this.filters)
      .subscribe((values: MediaModel[]) => this.updateMediaList(values))
  }

  private updateMediaList(medias: MediaModel[]) {
    this.mediasAreLoading = false;
    this.medias = this.medias.concat(medias)
    this.lastMediasLoadedCount = medias.length
  }

  private clearMediaSearchData(){
    this.scrolledOnTop = true
    this.lastMediasLoadedCount = -1
    this.medias = []
    this.loadErrorRaised = false
  }
}
