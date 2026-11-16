import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import { CreatedResponse } from '../../core/models/created-response';
import {
  GetCityByIdQueryDto,
  ListCitiesRequest,
  ListCitiesResponse,
  UpsertCityCommand
} from './cities-api.model';

/**
 * Šifarnik gradova. Čitanje: svaki prijavljeni korisnik; izmjene: samo Admin.
 */
@Injectable({
  providedIn: 'root'
})
export class CitiesApiService {
  private readonly baseUrl = `${environment.apiUrl}/Cities`;
  private http = inject(HttpClient);

  /** GET /Cities?search=&countryId=&paging... */
  list(request?: ListCitiesRequest): Observable<ListCitiesResponse> {
    const params = request ? buildHttpParams(request) : undefined;
    return this.http.get<ListCitiesResponse>(this.baseUrl, { params });
  }

  /** GET /Cities/{id} */
  getById(id: number): Observable<GetCityByIdQueryDto> {
    return this.http.get<GetCityByIdQueryDto>(`${this.baseUrl}/${id}`);
  }

  /** POST /Cities (Admin) → 201 { id } */
  create(payload: UpsertCityCommand): Observable<CreatedResponse> {
    return this.http.post<CreatedResponse>(this.baseUrl, payload);
  }

  /** PUT /Cities/{id} (Admin) → 204 */
  update(id: number, payload: UpsertCityCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
  }

  /** DELETE /Cities/{id} (Admin) → 204; 409 ako grad ima aerodrome */
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
