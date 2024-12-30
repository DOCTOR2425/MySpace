import { DeliveryMethod } from "./delivery-method.interface";
import { PaymentMethod } from "./payment-method.interface";

export interface OrderOptions {
  deliveryMethods: DeliveryMethod[];
  paymentMethods: PaymentMethod[];
}
