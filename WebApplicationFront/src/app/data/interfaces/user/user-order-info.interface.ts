import { UserDeliveryAddress } from './user-delivery-address.interface';

export interface UserOrderInfo {
  userId: string;
  firstName: string;
  surname: string;
  email: string;
  telephone: string;
  userDeliveryAddress: UserDeliveryAddress;
}
