import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AdminLayoutComponent } from './admin-layout/admin-layout.component';
import {MyProfileComponent} from '../account/my-profile/my-profile.component';
import {AdminSettingsComponent} from './admin-settings/admin-settings.component';
import {ExchangeRatesComponent} from './finance/exchange-rates/exchange-rates.component';
import {CountriesComponent} from './locations/countries/countries.component';
import {CitiesComponent} from './locations/cities/cities.component';
import {AirportsComponent} from './locations/airports/airports.component';

const routes: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      // ŠIFARNICI
      {
        path: 'countries',
        component: CountriesComponent,
      },
      {
        path: 'cities',
        component: CitiesComponent,
      },
      {
        path: 'airports',
        component: AirportsComponent,
      },

      // FINANCE
      {
        path: 'exchange-rates',
        component: ExchangeRatesComponent,
      },

      // POSTAVKE APLIKACIJE (prazna stranica – ovdje idu postavke vaše aplikacije)
      {
        path: 'settings',
        component: AdminSettingsComponent,
      },

      // MOJ PROFIL (ista stranica kao /client/profile, deklarisana u AccountModule)
      {
        path: 'profile',
        component: MyProfileComponent,
      },

      // default admin route → /admin/settings
      {
        path: '',
        redirectTo: 'settings',
        pathMatch: 'full',
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule {}
