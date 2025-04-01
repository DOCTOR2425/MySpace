import { DeliveryMethod } from '../order-options/delivery-method.interface';
import { UserOrderInfo } from '../user/user-order-info.interface';
import { PaidOrderItem } from './paid-order-item.interface';

export interface AdminPaidOrder {
  paidOrderId: string;
  orderDate: Date;
  receiptDate: Date;
  paymentMethod: string;
  deliveryMethod: DeliveryMethod;
  userOrderInfo: UserOrderInfo;
  paidOrderItems: PaidOrderItem[];
}
