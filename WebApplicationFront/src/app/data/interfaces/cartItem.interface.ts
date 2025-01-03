import { Product } from "./product.interface";

export interface CartItem {
  cartItemId: string,
  product: Product,
  quantity: number
}
