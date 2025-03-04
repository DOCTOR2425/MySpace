import { UserOrderInfo } from '../user/user-order-info.interface';
import { PaidOrderItem } from './paid-order-item.interface';

export interface AdminPaidOrder {
  paidOrderId: string;
  orderDate: Date;
  userOrderInfo: UserOrderInfo;
  paidOrderItems: PaidOrderItem[];
}
