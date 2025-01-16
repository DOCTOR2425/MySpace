import { CollectionFilter } from "./collection-filter.interface";
import { RangeFilter } from "./range-filter.interface";

export interface FilterRequest {
  rangeFilters: RangeFilter[];
  collectionFilters: CollectionFilter[];
}