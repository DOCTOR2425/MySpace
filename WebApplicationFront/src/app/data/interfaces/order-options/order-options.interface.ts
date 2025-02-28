import { DeliveryMethod } from './delivery-method.interface';

export interface OrderOptions {
  deliveryMethods: DeliveryMethod[];
  paymentMethods: string[];
}
