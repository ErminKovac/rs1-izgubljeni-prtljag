import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

// === QUERIES (READ) ===

/**
 * Query parameters for GET /Cities
 * Corresponds to: ListCitiesQuery.cs
 */
export class ListCitiesRequest extends BasePagedQuery {
  /** naziv ili poštanski broj */
  search?: string | null;
  countryId?: number | null;
}

/**
 * Response item for GET /Cities
 * Corresponds to: ListCitiesQueryDto.cs
 */
export interface ListCitiesQueryDto {
  id: number;
  name: string;
  postalCode: string | null;
  countryId: number;
  countryName: string;
  countryIsoCode: string;
  /** grad sa aerodromima se ne može obrisati */
  airportsCount: number;
}

/**
 * Response for GET /Cities/{id}
 * Corresponds to: GetCityByIdQueryDto.cs
 */
export interface GetCityByIdQueryDto {
  id: number;
  name: string;
  postalCode: string | null;
  countryId: number;
  countryName: string;
}

export type ListCitiesResponse = PageResult<ListCitiesQueryDto>;

// === COMMANDS (WRITE) ===

/**
 * Command for POST /Cities and PUT /Cities/{id}
 * Corresponds to: CreateCityCommand.cs / UpdateCityCommand.cs
 */
export interface UpsertCityCommand {
  name: string;
  postalCode: string | null;
  countryId: number;
}
