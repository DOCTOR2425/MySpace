import { PaidOrderItem } from './paid-order-item.interface';
import { PromoCode } from './promo-code.interface';

export interface UserPaidOrder {
  paidOrderId: string;
  orderDate: Date;
  receiptDate: Date;
  paidOrderItems: PaidOrderItem[];
  promoCode: PromoCode | null;
}
