export interface CreateProductRequest {
  name: string;
  description: string;
  price: number;
  quantity: number;
  images: File[];
  propertyValues: { [key: string]: string };
  productCategoryId: string;
  brandId: string;
  countryId: string;
}