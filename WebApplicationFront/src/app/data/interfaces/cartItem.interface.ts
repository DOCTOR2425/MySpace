import { ProductData } from "./product/productData.interface";

export interface CartItem {
  cartItemId: string,
  product: ProductData,
  quantity: number
}
