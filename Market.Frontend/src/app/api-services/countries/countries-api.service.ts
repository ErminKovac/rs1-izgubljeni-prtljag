import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import { CreatedResponse } from '../../core/models/created-response';
import {
  GetCountryByIdQueryDto,
  ListCountriesRequest,
  ListCountriesResponse,
  UpsertCountryCommand
} from './countries-api.model';

/**
 * Šifarnik država. Čitanje: svaki prijavljeni korisnik; izmjene: samo Admin.
 */
@Injectable({
  providedIn: 'root'
})
export class CountriesApiService {
  private readonly baseUrl = `${environment.apiUrl}/Countries`;
  private http = inject(HttpClient);

  /** GET /Countries?search=&paging... */
  list(request?: ListCountriesRequest): Observable<ListCountriesResponse> {
    const params = request ? buildHttpParams(request) : undefined;
    return this.http.get<ListCountriesResponse>(this.baseUrl, { params });
  }

  /** GET /Countries/{id} */
  getById(id: number): Observable<GetCountryByIdQueryDto> {
    return this.http.get<GetCountryByIdQueryDto>(`${this.baseUrl}/${id}`);
  }

  /** POST /Countries (Admin) → 201 { id } */
  create(payload: UpsertCountryCommand): Observable<CreatedResponse> {
    return this.http.post<CreatedResponse>(this.baseUrl, payload);
  }

  /** PUT /Countries/{id} (Admin) → 204 */
  update(id: number, payload: UpsertCountryCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
  }

  /** DELETE /Countries/{id} (Admin) → 204; 409 ako država ima gradove */
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
