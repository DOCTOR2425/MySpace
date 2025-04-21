import { PaidOrderItem } from './paid-order-item.interface';

export interface UserPaidOrder {
  paidOrderId: string;
  orderDate: Date;
  receiptDate: Date;
  paidOrderItems: PaidOrderItem[];
}
