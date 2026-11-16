import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { debounceTime, distinctUntilChanged, filter, forkJoin, switchMap, tap } from 'rxjs';
import { AirportsApiService } from '../../../../api-services/airports/airports-api.service';
import {
  ListAirportsQueryDto,
  ListAirportsRequest
} from '../../../../api-services/airports/airports-api.model';
import { CitiesApiService } from '../../../../api-services/cities/cities-api.service';
import { ListCitiesQueryDto, ListCitiesRequest } from '../../../../api-services/cities/cities-api.model';
import { CountriesApiService } from '../../../../api-services/countries/countries-api.service';
import {
  ListCountriesQueryDto,
  ListCountriesRequest
} from '../../../../api-services/countries/countries-api.model';
import { BaseListPagedComponent } from '../../../../core/components/base-classes/base-list-paged-component';
import { getErrorMessage } from '../../../../core/interceptors/error-logging-interceptor.service';
import { allItemsPaging } from '../../../../core/models/paging/paging-utils';
import { ToasterService } from '../../../../core/services/toaster.service';
import { DialogHelperService } from '../../../shared/services/dialog-helper.service';
import { DialogButton, DialogType } from '../../../shared/models/dialog-config.model';
import { AirportUpsertComponent, AirportUpsertDialogData } from './airport-upsert/airport-upsert.component';

/**
 * Šifarnik aerodroma (/admin/airports): lista sa pretragom, filterima po državi i gradu i paging-om,
 * dodavanje, uređivanje (dijalog AirportUpsertComponent) i brisanje.
 */
@Component({
  selector: 'app-airports',
  standalone: false,
  templateUrl: './airports.component.html',
  styleUrl: '../location-list.scss'
})
export class AirportsComponent
  extends BaseListPagedComponent<ListAirportsQueryDto, ListAirportsRequest>
  implements OnInit
{
  private api = inject(AirportsApiService);
  private countriesApi = inject(CountriesApiService);
  private citiesApi = inject(CitiesApiService);
  private dialog = inject(MatDialog);
  private dialogHelper = inject(DialogHelperService);
  private toaster = inject(ToasterService);
  private translate = inject(TranslateService);
  private destroyRef = inject(DestroyRef);

  readonly displayedColumns = ['iataCode', 'name', 'city', 'country', 'actions'];

  readonly searchControl = new FormControl('', { nonNullable: true });
  readonly countryFilter = new FormControl<number | null>(null);
  readonly cityFilter = new FormControl<number | null>(null);

  countries: ListCountriesQueryDto[] = [];
  private allCities: ListCitiesQueryDto[] = [];

  /** gradovi za filter: samo gradovi odabrane države (ili svi) */
  get filterCities(): ListCitiesQueryDto[] {
    const countryId = this.countryFilter.value;
    return countryId ? this.allCities.filter((c) => c.countryId === countryId) : this.allCities;
  }

  constructor() {
    super();
    this.request = new ListAirportsRequest();
  }

  ngOnInit(): void {
    this.initList();
    this.loadFilterOptions();

    this.searchControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe((term) => {
        this.request.search = term;
        this.reloadFromFirstPage();
      });

    this.countryFilter.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((countryId) => {
        this.request.countryId = countryId;
        // odabrani grad možda nije u novoj državi → filter grada se poništava
        // (emitEvent: false – lista se učita samo jednom, ovdje)
        this.cityFilter.setValue(null, { emitEvent: false });
        this.request.cityId = null;
        this.reloadFromFirstPage();
      });

    this.cityFilter.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((cityId) => {
        this.request.cityId = cityId;
        this.reloadFromFirstPage();
      });
  }

  protected loadPagedData(): void {
    this.startLoading();

    this.api
      .list(this.request)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          this.handlePageResult(response);
          this.stopLoading();
        },
        error: (err) => this.stopLoading(getErrorMessage(err))
      });
  }

  private reloadFromFirstPage(): void {
    this.request.paging.page = 1;
    this.loadPagedData();
  }

  onCreate(): void {
    this.openUpsert({ cityId: this.cityFilter.value ?? undefined });
  }

  onEdit(airport: ListAirportsQueryDto): void {
    this.openUpsert({ airportId: airport.id });
  }

  private openUpsert(data: AirportUpsertDialogData): void {
    this.dialog
      .open<AirportUpsertComponent, AirportUpsertDialogData, boolean>(AirportUpsertComponent, {
        width: '540px',
        maxWidth: '90vw',
        autoFocus: true,
        data
      })
      .afterClosed()
      .pipe(filter((saved) => saved === true))
      .subscribe(() => {
        const key = data.airportId ? 'LOCATIONS.AIRPORTS.UPDATED' : 'LOCATIONS.AIRPORTS.CREATED';
        this.toaster.success(this.translate.instant(key));
        this.loadPagedData();
      });
  }

  onDelete(airport: ListAirportsQueryDto): void {
    this.dialogHelper
      .confirmDelete(`${airport.iataCode} – ${airport.name}`)
      .pipe(
        filter((result) => result?.button === DialogButton.DELETE),
        tap(() => this.startLoading()),
        switchMap(() => this.api.delete(airport.id))
      )
      .subscribe({
        next: () => {
          this.toaster.success(this.translate.instant('LOCATIONS.AIRPORTS.DELETED'));
          this.loadPagedData();
        },
        error: (err: HttpErrorResponse) => {
          this.stopLoading();
          this.dialogHelper
            .open({
              type: err.status === 409 ? DialogType.WARNING : DialogType.ERROR,
              titleKey: 'LOCATIONS.DELETE_FAILED',
              message: getErrorMessage(err),
              buttons: [{ type: DialogButton.OK, color: 'primary' }]
            })
            .subscribe();
        }
      });
  }

  private loadFilterOptions(): void {
    const countriesRequest = new ListCountriesRequest();
    countriesRequest.paging = allItemsPaging;
    const citiesRequest = new ListCitiesRequest();
    citiesRequest.paging = allItemsPaging;

    forkJoin({
      countries: this.countriesApi.list(countriesRequest),
      cities: this.citiesApi.list(citiesRequest)
    })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: ({ countries, cities }) => {
          this.countries = countries.items;
          this.allCities = cities.items;
        },
        error: (err) => this.toaster.error(getErrorMessage(err))
      });
  }
}
