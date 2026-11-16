import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

// === QUERIES (READ) ===

/**
 * Query parameters for GET /Airports
 * Corresponds to: ListAirportsQuery.cs
 */
export class ListAirportsRequest extends BasePagedQuery {
  /** naziv aerodroma, IATA oznaka ili naziv grada */
  search?: string | null;
  countryId?: number | null;
  cityId?: number | null;
}

/**
 * Response item for GET /Airports
 * Corresponds to: ListAirportsQueryDto.cs
 */
export interface ListAirportsQueryDto {
  id: number;
  name: string;
  /** IATA, npr. "SJJ" */
  iataCode: string;
  cityId: number;
  cityName: string;
  countryId: number;
  countryName: string;
}

/**
 * Response for GET /Airports/{id}
 * Corresponds to: GetAirportByIdQueryDto.cs
 */
export interface GetAirportByIdQueryDto {
  id: number;
  name: string;
  iataCode: string;
  cityId: number;
  cityName: string;
  countryId: number;
  countryName: string;
}

export type ListAirportsResponse = PageResult<ListAirportsQueryDto>;

// === COMMANDS (WRITE) ===

/**
 * Command for POST /Airports and PUT /Airports/{id}
 * Corresponds to: CreateAirportCommand.cs / UpdateAirportCommand.cs
 */
export interface UpsertAirportCommand {
  name: string;
  iataCode: string;
  cityId: number;
}
