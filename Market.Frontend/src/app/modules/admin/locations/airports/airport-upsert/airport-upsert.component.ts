import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { forkJoin, Observable, of } from 'rxjs';
import { AirportsApiService } from '../../../../../api-services/airports/airports-api.service';
import { UpsertAirportCommand } from '../../../../../api-services/airports/airports-api.model';
import { CitiesApiService } from '../../../../../api-services/cities/cities-api.service';
import { ListCitiesQueryDto, ListCitiesRequest } from '../../../../../api-services/cities/cities-api.model';
import { CountriesApiService } from '../../../../../api-services/countries/countries-api.service';
import {
  ListCountriesQueryDto,
  ListCountriesRequest
} from '../../../../../api-services/countries/countries-api.model';
import { applyServerErrors } from '../../../../../core/forms/server-errors';
import { getErrorMessage } from '../../../../../core/interceptors/error-logging-interceptor.service';
import { allItemsPaging } from '../../../../../core/models/paging/paging-utils';

/** airportId postoji → uređivanje; cityId = unaprijed odabrani grad kod dodavanja */
export interface AirportUpsertDialogData {
  airportId?: number;
  cityId?: number;
}

interface AirportForm {
  name: FormControl<string>;
  iataCode: FormControl<string>;
  /** pomoćno polje (ne šalje se): sužava listu gradova */
  countryId: FormControl<number | null>;
  cityId: FormControl<number | null>;
}

/**
 * Dijalog za dodavanje i uređivanje aerodroma. Kaskadni izbor: prvo država, pa grad te države.
 */
@Component({
  selector: 'app-airport-upsert',
  standalone: false,
  templateUrl: './airport-upsert.component.html',
  styleUrl: '../../location-upsert.scss'
})
export class AirportUpsertComponent implements OnInit {
  private dialogRef = inject<MatDialogRef<AirportUpsertComponent, boolean>>(MatDialogRef);
  private data = inject<AirportUpsertDialogData>(MAT_DIALOG_DATA);
  private api = inject(AirportsApiService);
  private countriesApi = inject(CountriesApiService);
  private citiesApi = inject(CitiesApiService);
  private destroyRef = inject(DestroyRef);

  /** isto kao AirportEntity.Constraints.NameMaxLength na backendu */
  readonly nameMaxLength = 150;

  readonly form = new FormGroup<AirportForm>({
    name: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(this.nameMaxLength)]
    }),
    // isto pravilo kao CreateAirportCommandValidator: tačno 3 slova
    iataCode: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.pattern(/^[A-Za-z]{3}$/)]
    }),
    countryId: new FormControl<number | null>(null, [Validators.required]),
    cityId: new FormControl<number | null>(null, [Validators.required])
  });

  readonly isEditMode = !!this.data.airportId;
  countries: ListCountriesQueryDto[] = [];
  private allCities: ListCitiesQueryDto[] = [];
  isLoading = true;
  isSaving = false;
  errorMessage: string | null = null;

  /** gradovi odabrane države */
  get cities(): ListCitiesQueryDto[] {
    const countryId = this.form.controls.countryId.value;
    return countryId ? this.allCities.filter((c) => c.countryId === countryId) : [];
  }

  ngOnInit(): void {
    const countriesRequest = new ListCountriesRequest();
    countriesRequest.paging = allItemsPaging;
    const citiesRequest = new ListCitiesRequest();
    citiesRequest.paging = allItemsPaging;

    forkJoin({
      countries: this.countriesApi.list(countriesRequest),
      cities: this.citiesApi.list(citiesRequest),
      airport: this.isEditMode ? this.api.getById(this.data.airportId!) : of(null)
    })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: ({ countries, cities, airport }) => {
          this.countries = countries.items;
          this.allCities = cities.items;

          if (airport) {
            this.form.setValue({
              name: airport.name,
              iataCode: airport.iataCode,
              countryId: airport.countryId,
              cityId: airport.cityId
            });
          } else if (this.data.cityId) {
            // dodavanje iz liste filtrirane po gradu: država i grad su već odabrani
            const city = this.allCities.find((c) => c.id === this.data.cityId);
            if (city) {
              this.form.patchValue({ countryId: city.countryId, cityId: city.id });
            }
          }

          this.isLoading = false;
          this.watchCountryChanges();
        },
        error: (err) => {
          this.isLoading = false;
          this.errorMessage = getErrorMessage(err);
        }
      });
  }

  /** nova država → grad iz stare države više ne važi */
  private watchCountryChanges(): void {
    this.form.controls.countryId.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.form.controls.cityId.setValue(null));
  }

  onSubmit(): void {
    if (this.form.invalid || this.isSaving) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    const command: UpsertAirportCommand = {
      name: value.name.trim(),
      iataCode: value.iataCode.trim().toUpperCase(),
      cityId: value.cityId!
    };

    this.isSaving = true;
    this.errorMessage = null;

    // create vraća { id }, update ništa (204) – dijalogu je bitno samo da je uspjelo
    const request$: Observable<unknown> = this.isEditMode
      ? this.api.update(this.data.airportId!, command)
      : this.api.create(command);

    request$.subscribe({
      next: () => this.dialogRef.close(true),
      error: (err) => {
        this.isSaving = false;
        this.errorMessage = applyServerErrors(this.form, err);
      }
    });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
