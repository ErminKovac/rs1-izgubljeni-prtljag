import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { CountriesApiService } from '../../../../../api-services/countries/countries-api.service';
import { UpsertCountryCommand } from '../../../../../api-services/countries/countries-api.model';
import { applyServerErrors } from '../../../../../core/forms/server-errors';
import { getErrorMessage } from '../../../../../core/interceptors/error-logging-interceptor.service';

/** countryId postoji → uređivanje, inače dodavanje */
export interface CountryUpsertDialogData {
  countryId?: number;
}

/** Tipizirana forma: form.controls.name.value je string, ne any. */
interface CountryForm {
  name: FormControl<string>;
  isoCode: FormControl<string>;
}

/**
 * Dijalog za dodavanje i uređivanje države. Zatvara se sa true kad je snimanje uspjelo
 * (lista tada prikaže poruku i ponovo se učita).
 */
@Component({
  selector: 'app-country-upsert',
  standalone: false,
  templateUrl: './country-upsert.component.html',
  styleUrl: '../../location-upsert.scss'
})
export class CountryUpsertComponent implements OnInit {
  private dialogRef = inject<MatDialogRef<CountryUpsertComponent, boolean>>(MatDialogRef);
  private data = inject<CountryUpsertDialogData>(MAT_DIALOG_DATA);
  private api = inject(CountriesApiService);
  private destroyRef = inject(DestroyRef);

  /** isto kao CountryEntity.Constraints.NameMaxLength na backendu */
  readonly nameMaxLength = 100;

  readonly form = new FormGroup<CountryForm>({
    name: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(this.nameMaxLength)]
    }),
    // isto pravilo kao CreateCountryCommandValidator: tačno 2 slova
    isoCode: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.pattern(/^[A-Za-z]{2}$/)]
    })
  });

  readonly isEditMode = !!this.data.countryId;
  isLoading = false;
  isSaving = false;
  /** opšta poruka greške sa backenda (prikazuje se iznad polja) */
  errorMessage: string | null = null;

  ngOnInit(): void {
    if (this.isEditMode) {
      this.isLoading = true;
      this.api
        .getById(this.data.countryId!)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: (country) => {
            this.form.setValue({ name: country.name, isoCode: country.isoCode });
            this.isLoading = false;
          },
          error: (err) => {
            this.isLoading = false;
            this.errorMessage = getErrorMessage(err);
          }
        });
    }
  }

  onSubmit(): void {
    if (this.form.invalid || this.isSaving) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    const command: UpsertCountryCommand = {
      name: value.name.trim(),
      isoCode: value.isoCode.trim().toUpperCase()
    };

    this.isSaving = true;
    this.errorMessage = null;

    // create vraća { id }, update ništa (204) – dijalogu je bitno samo da je uspjelo
    const request$: Observable<unknown> = this.isEditMode
      ? this.api.update(this.data.countryId!, command)
      : this.api.create(command);

    request$.subscribe({
      next: () => this.dialogRef.close(true),
      error: (err) => {
        this.isSaving = false;
        // greške polja idu u mat-error, opšta poruka (npr. 409 duplikat) iznad forme; dijalog ostaje otvoren
        this.errorMessage = applyServerErrors(this.form, err);
      }
    });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
