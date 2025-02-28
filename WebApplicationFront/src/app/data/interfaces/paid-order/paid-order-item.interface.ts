import { ProductData } from '../product/product-data.interface';

export interface PaidOrderItem {
  paidOrderItemId: string;
  quantity: number;
  price: number;
  productData: ProductData;
}
