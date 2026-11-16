import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpErrorResponse } from '@angular/common/http';
import { FormControl } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { debounceTime, distinctUntilChanged, filter, switchMap, tap } from 'rxjs';
import { CountriesApiService } from '../../../../api-services/countries/countries-api.service';
import {
  ListCountriesQueryDto,
  ListCountriesRequest
} from '../../../../api-services/countries/countries-api.model';
import { BaseListPagedComponent } from '../../../../core/components/base-classes/base-list-paged-component';
import { getErrorMessage } from '../../../../core/interceptors/error-logging-interceptor.service';
import { ToasterService } from '../../../../core/services/toaster.service';
import { DialogHelperService } from '../../../shared/services/dialog-helper.service';
import { DialogButton, DialogType } from '../../../shared/models/dialog-config.model';
import { CountryUpsertComponent, CountryUpsertDialogData } from './country-upsert/country-upsert.component';

/**
 * Šifarnik država (/admin/countries): lista sa pretragom i paging-om, dodavanje, uređivanje i brisanje.
 * Dodavanje i uređivanje su u dijalogu (CountryUpsertComponent).
 */
@Component({
  selector: 'app-countries',
  standalone: false,
  templateUrl: './countries.component.html',
  styleUrl: '../location-list.scss'
})
export class CountriesComponent
  extends BaseListPagedComponent<ListCountriesQueryDto, ListCountriesRequest>
  implements OnInit
{
  private api = inject(CountriesApiService);
  private dialog = inject(MatDialog);
  private dialogHelper = inject(DialogHelperService);
  private toaster = inject(ToasterService);
  private translate = inject(TranslateService);
  private destroyRef = inject(DestroyRef);

  readonly displayedColumns = ['name', 'isoCode', 'citiesCount', 'actions'];

  /** pretraga dok korisnik kuca (300 ms pauze), bez dugmeta "Traži" */
  readonly searchControl = new FormControl('', { nonNullable: true });

  constructor() {
    super();
    this.request = new ListCountriesRequest();
  }

  ngOnInit(): void {
    this.initList();

    this.searchControl.valueChanges
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe((term) => {
        this.request.search = term;
        this.request.paging.page = 1; // novi filter → od prve stranice
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
    this.openUpsert({});
  }

  onEdit(country: ListCountriesQueryDto): void {
    this.openUpsert({ countryId: country.id });
  }

  private openUpsert(data: CountryUpsertDialogData): void {
    this.dialog
      .open<CountryUpsertComponent, CountryUpsertDialogData, boolean>(CountryUpsertComponent, {
        width: '500px',
        maxWidth: '90vw',
        autoFocus: true,
        data
      })
      .afterClosed()
      .pipe(filter((saved) => saved === true))
      .subscribe(() => {
        const key = data.countryId ? 'LOCATIONS.COUNTRIES.UPDATED' : 'LOCATIONS.COUNTRIES.CREATED';
        this.toaster.success(this.translate.instant(key));
        this.loadPagedData();
      });
  }

  /** brisanje je nepovratno → prvo modal za potvrdu; 409 (država ima gradove) → upozorenje sa porukom backenda */
  onDelete(country: ListCountriesQueryDto): void {
    this.dialogHelper
      .confirmDelete(country.name)
      .pipe(
        filter((result) => result?.button === DialogButton.DELETE),
        tap(() => this.startLoading()),
        switchMap(() => this.api.delete(country.id))
      )
      .subscribe({
        next: () => {
          this.toaster.success(this.translate.instant('LOCATIONS.COUNTRIES.DELETED'));
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
}
