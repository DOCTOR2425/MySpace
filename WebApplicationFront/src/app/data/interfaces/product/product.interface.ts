import { ProductData } from "./product-data.interface";

export interface Product {
  productResponseData: ProductData,
  properties: { [key: string]: string };
}