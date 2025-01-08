import { ProductData } from "./productData.interface";

export interface Product {
  productResponseData: ProductData,
  properties: { [key: string]: string };
}