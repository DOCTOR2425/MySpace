export interface AdminUser {
  userId: string;
  firstName: string;
  surname: string;
  telephone: string;
  email: string;
  orderCount: number;
  registrationDate: Date;
  blockDate: Date | undefined;
  blockDetails: string | undefined;
}
