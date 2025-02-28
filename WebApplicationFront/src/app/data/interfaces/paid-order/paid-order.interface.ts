import { PaidOrderItem } from './paid-order-item.interface';

export interface PaidOrder {
  paidOrderId: string;
  orderDate: Date;
  paidOrderItems: PaidOrderItem[];
}
