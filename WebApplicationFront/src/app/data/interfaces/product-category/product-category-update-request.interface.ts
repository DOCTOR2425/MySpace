import { ProductPropertyDTOUpdate } from './product-property-update-request.interface';

export interface ProductCategoryDTOUpdate {
  name: string;
  properties: ProductPropertyDTOUpdate[];
}
