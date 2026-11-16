import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import { CreatedResponse } from '../../core/models/created-response';
import {
  GetAirportByIdQueryDto,
  ListAirportsRequest,
  ListAirportsResponse,
  UpsertAirportCommand
} from './airports-api.model';

/**
 * Šifarnik aerodroma. Čitanje: svaki prijavljeni korisnik; izmjene: samo Admin.
 */
@Injectable({
  providedIn: 'root'
})
export class AirportsApiService {
  private readonly baseUrl = `${environment.apiUrl}/Airports`;
  private http = inject(HttpClient);

  /** GET /Airports?search=&countryId=&cityId=&paging... */
  list(request?: ListAirportsRequest): Observable<ListAirportsResponse> {
    const params = request ? buildHttpParams(request) : undefined;
    return this.http.get<ListAirportsResponse>(this.baseUrl, { params });
  }

  /** GET /Airports/{id} */
  getById(id: number): Observable<GetAirportByIdQueryDto> {
    return this.http.get<GetAirportByIdQueryDto>(`${this.baseUrl}/${id}`);
  }

  /** POST /Airports (Admin) → 201 { id } */
  create(payload: UpsertAirportCommand): Observable<CreatedResponse> {
    return this.http.post<CreatedResponse>(this.baseUrl, payload);
  }

  /** PUT /Airports/{id} (Admin) → 204 */
  update(id: number, payload: UpsertAirportCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
  }

  /** DELETE /Airports/{id} (Admin) → 204 */
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
