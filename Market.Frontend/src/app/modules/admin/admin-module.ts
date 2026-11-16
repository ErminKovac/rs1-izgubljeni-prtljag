import {NgModule} from '@angular/core';

import {AdminRoutingModule} from './admin-routing-module';
import {AdminLayoutComponent} from './admin-layout/admin-layout.component';
import {SharedModule} from '../shared/shared-module';
import {AccountModule} from '../account/account-module';
import { ExchangeRatesComponent } from './finance/exchange-rates/exchange-rates.component';
import { AdminSettingsComponent } from './admin-settings/admin-settings.component';
import { CountriesComponent } from './locations/countries/countries.component';
import { CountryUpsertComponent } from './locations/countries/country-upsert/country-upsert.component';
import { CitiesComponent } from './locations/cities/cities.component';
import { CityUpsertComponent } from './locations/cities/city-upsert/city-upsert.component';
import { AirportsComponent } from './locations/airports/airports.component';
import { AirportUpsertComponent } from './locations/airports/airport-upsert/airport-upsert.component';


@NgModule({
  declarations: [
    AdminLayoutComponent,
    ExchangeRatesComponent,
    AdminSettingsComponent,
    // ŠIFARNICI (issue #2)
    CountriesComponent,
    CountryUpsertComponent,
    CitiesComponent,
    CityUpsertComponent,
    AirportsComponent,
    AirportUpsertComponent,
  ],
  imports: [
    AdminRoutingModule,
    SharedModule,
    // "Moj profil" (/admin/profile) – ista stranica kao /client/profile
    AccountModule,
  ]
})
export class AdminModule { }
