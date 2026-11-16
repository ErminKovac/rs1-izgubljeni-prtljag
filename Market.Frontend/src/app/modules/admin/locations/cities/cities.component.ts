import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { debounceTime, distinctUntilChanged, filter, switchMap, tap } from 'rxjs';
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
import { CityUpsertComponent, CityUpsertDialogData } from './city-upsert/city-upsert.component';

/**
 * Šifarnik gradova (/admin/cities): lista sa pretragom, filterom po državi i paging-om,
 * dodavanje, uređivanje (dijalog CityUpsertComponent) i brisanje.
 */
@Component({
  selector: 'app-cities',
  standalone: false,
  templateUrl: './cities.component.html',
  styleUrl: '../location-list.scss'
})
export class CitiesComponent
  extends BaseListPagedComponent<ListCitiesQueryDto, ListCitiesRequest>
  implements OnInit
{
  private api = inject(CitiesApiService);
  private countriesApi = inject(CountriesApiService);
  private dialog = inject(MatDialog);
  private dialogHelper = inject(DialogHelperService);
  private toaster = inject(ToasterService);
  private translate = inject(TranslateService);
  private destroyRef = inject(DestroyRef);

  readonly displayedColumns = ['name', 'postalCode', 'country', 'airportsCount', 'actions'];

  readonly searchControl = new FormControl('', { nonNullable: true });
  /** null = sve države */
  readonly countryFilter = new FormControl<number | null>(null);

  /** opcije za filter po državi */
  countries: ListCountriesQueryDto[] = [];

  constructor() {
    super();
    this.request = new ListCitiesRequest();
  }

  ngOnInit(): void {
    this.initList();
    this.loadCountries();

    this.searchControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe((term) => {
        this.request.search = term;
        this.request.paging.page = 1;
        this.loadPagedData();
      });

    this.countryFilter.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((countryId) => {
        this.request.countryId = countryId;
        this.request.paging.page = 1;
        this.loadPagedData();
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

  onCreate(): void {
    // nova stavka dobije državu iz filtera (često se unosi više gradova iste države)
    this.openUpsert({ countryId: this.countryFilter.value ?? undefined });
  }

  onEdit(city: ListCitiesQueryDto): void {
    this.openUpsert({ cityId: city.id });
  }

  private openUpsert(data: CityUpsertDialogData): void {
    this.dialog
      .open<CityUpsertComponent, CityUpsertDialogData, boolean>(CityUpsertComponent, {
        width: '500px',
        maxWidth: '90vw',
        autoFocus: true,
        data
      })
      .afterClosed()
      .pipe(filter((saved) => saved === true))
      .subscribe(() => {
        const key = data.cityId ? 'LOCATIONS.CITIES.UPDATED' : 'LOCATIONS.CITIES.CREATED';
        this.toaster.success(this.translate.instant(key));
        this.loadPagedData();
      });
  }

  onDelete(city: ListCitiesQueryDto): void {
    this.dialogHelper
      .confirmDelete(city.name)
      .pipe(
        filter((result) => result?.button === DialogButton.DELETE),
        tap(() => this.startLoading()),
        switchMap(() => this.api.delete(city.id))
      )
      .subscribe({
        next: () => {
          this.toaster.success(this.translate.instant('LOCATIONS.CITIES.DELETED'));
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

  private loadCountries(): void {
    const request = new ListCountriesRequest();
    request.paging = allItemsPaging; // sve države za dropdown (backend maksimum 1000)

    this.countriesApi
      .list(request)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => (this.countries = response.items),
        error: (err) => this.toaster.error(getErrorMessage(err))
      });
  }
}
