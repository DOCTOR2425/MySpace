import { CollectionPropertyForFilter } from './collection-property-for-filter.intervace';
import { RangePropertyForFilter } from './range-property-for-filter.intervace';

export interface CategoryFilters {
  rangePropertyForFilters: RangePropertyForFilter[];
  collectionPropertyForFilters: CollectionPropertyForFilter[];
}
