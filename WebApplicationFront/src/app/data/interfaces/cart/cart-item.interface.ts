import { ProductData } from "../product/product-data.interface";

export interface CartItem {
  cartItemId: string,
  product: ProductData,
  quantity: number
}
