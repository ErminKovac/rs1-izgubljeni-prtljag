import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

// === QUERIES (READ) ===

/**
 * Query parameters for GET /Countries
 * Corresponds to: ListCountriesQuery.cs
 */
export class ListCountriesRequest extends BasePagedQuery {
  /** naziv ili ISO oznaka (sadrži, bez obzira na velika/mala slova) */
  search?: string | null;
}

/**
 * Response item for GET /Countries
 * Corresponds to: ListCountriesQueryDto.cs
 */
export interface ListCountriesQueryDto {
  id: number;
  name: string;
  /** ISO 3166-1 alpha-2, npr. "BA" */
  isoCode: string;
  /** država sa gradovima se ne može obrisati */
  citiesCount: number;
}

/**
 * Response for GET /Countries/{id}
 * Corresponds to: GetCountryByIdQueryDto.cs
 */
export interface GetCountryByIdQueryDto {
  id: number;
  name: string;
  isoCode: string;
}

export type ListCountriesResponse = PageResult<ListCountriesQueryDto>;

// === COMMANDS (WRITE) ===

/**
 * Command for POST /Countries and PUT /Countries/{id}
 * Corresponds to: CreateCountryCommand.cs / UpdateCountryCommand.cs
 */
export interface UpsertCountryCommand {
  name: string;
  isoCode: string;
}
