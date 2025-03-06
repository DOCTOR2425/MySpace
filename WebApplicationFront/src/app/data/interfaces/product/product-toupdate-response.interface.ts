import { ProductPropertyValuesResponse } from './product-property-value-respone.interface';

export interface ProductToUpdateResponse {
  productId: string;
  name: string;
  description: string;
  price: number;
  quantity: number;
  productCategory: string;
  brand: string;
  country: string;

  images: string[];
  productPropertyValues: ProductPropertyValuesResponse[];
}
