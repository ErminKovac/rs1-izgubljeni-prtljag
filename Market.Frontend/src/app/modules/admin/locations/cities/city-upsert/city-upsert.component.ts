import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { forkJoin, Observable, of } from 'rxjs';
import { CitiesApiService } from '../../../../../api-services/cities/cities-api.service';
import { UpsertCityCommand } from '../../../../../api-services/cities/cities-api.model';
import { CountriesApiService } from '../../../../../api-services/countries/countries-api.service';
import {
  ListCountriesQueryDto,
  ListCountriesRequest
} from '../../../../../api-services/countries/countries-api.model';
import { applyServerErrors } from '../../../../../core/forms/server-errors';
import { getErrorMessage } from '../../../../../core/interceptors/error-logging-interceptor.service';
import { allItemsPaging } from '../../../../../core/models/paging/paging-utils';

/** cityId postoji → uređivanje; countryId = unaprijed odabrana država kod dodavanja */
export interface CityUpsertDialogData {
  cityId?: number;
  countryId?: number;
}

interface CityForm {
  name: FormControl<string>;
  postalCode: FormControl<string>;
  countryId: FormControl<number | null>;
}

/**
 * Dijalog za dodavanje i uređivanje grada (država se bira iz padajuće liste).
 */
@Component({
  selector: 'app-city-upsert',
  standalone: false,
  templateUrl: './city-upsert.component.html',
  styleUrl: '../../location-upsert.scss'
})
export class CityUpsertComponent implements OnInit {
  private dialogRef = inject<MatDialogRef<CityUpsertComponent, boolean>>(MatDialogRef);
  private data = inject<CityUpsertDialogData>(MAT_DIALOG_DATA);
  private api = inject(CitiesApiService);
  private countriesApi = inject(CountriesApiService);
  private destroyRef = inject(DestroyRef);

  /** isto kao CityEntity.Constraints na backendu */
  readonly nameMaxLength = 100;
  readonly postalCodeMaxLength = 20;

  readonly form = new FormGroup<CityForm>({
    name: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(this.nameMaxLength)]
    }),
    // opciono: prazno polje backend snimi kao null
    postalCode: new FormControl('', {
      nonNullable: true,
      validators: [Validators.maxLength(this.postalCodeMaxLength)]
    }),
    countryId: new FormControl<number | null>(this.data.countryId ?? null, [Validators.required])
  });

  readonly isEditMode = !!this.data.cityId;
  countries: ListCountriesQueryDto[] = [];
  isLoading = true;
  isSaving = false;
  errorMessage: string | null = null;

  ngOnInit(): void {
    const countriesRequest = new ListCountriesRequest();
    countriesRequest.paging = allItemsPaging;

    // države i (kod uređivanja) grad se učitavaju paralelno; forma se prikaže kad stigne oboje
    forkJoin({
      countries: this.countriesApi.list(countriesRequest),
      city: this.isEditMode ? this.api.getById(this.data.cityId!) : of(null)
    })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: ({ countries, city }) => {
          this.countries = countries.items;
          if (city) {
            this.form.setValue({
              name: city.name,
              postalCode: city.postalCode ?? '',
              countryId: city.countryId
            });
          }
          this.isLoading = false;
        },
        error: (err) => {
          this.isLoading = false;
          this.errorMessage = getErrorMessage(err);
        }
      });
  }

  onSubmit(): void {
    if (this.form.invalid || this.isSaving) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    const command: UpsertCityCommand = {
      name: value.name.trim(),
      postalCode: value.postalCode.trim() || null,
      countryId: value.countryId!
    };

    this.isSaving = true;
    this.errorMessage = null;

    // create vraća { id }, update ništa (204) – dijalogu je bitno samo da je uspjelo
    const request$: Observable<unknown> = this.isEditMode
      ? this.api.update(this.data.cityId!, command)
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
