import { UserDeliveryAddress } from './UserDeliveryAddress.interface';

export interface UserOrderInfo {
  userId: string;
  firstName: string;
  surname: string;
  email: string;
  telephone: string;
  userDeliveryAddress: UserDeliveryAddress;
}
