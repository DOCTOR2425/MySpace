export interface ProductCategoryCreateRequest {
  name: string;
  properties: { [key: string]: boolean };
}
