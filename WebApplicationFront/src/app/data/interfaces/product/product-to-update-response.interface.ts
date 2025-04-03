import { ProductPropertyValuesResponse } from './product-property-value-respone.interface';

export interface ProductToUpdate {
  productId: string;
  name: string;
  description: string;
  price: number;
  quantity: number;
  productCategory: string;
  brand: string;
  country: string;
  isArchive: boolean;

  images: string[];
  productPropertyValues: ProductPropertyValuesResponse[];
}
