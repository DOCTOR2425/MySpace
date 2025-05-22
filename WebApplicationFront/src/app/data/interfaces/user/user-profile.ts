export interface UserProfile {
  firstName: string;
  surname: string;
  telephone: string;
  email: string;
  city: string;
  street: string;
  houseNumber: string;
  entrance: string;
  flat: string;
  registrationDate: Date;

  commentNumber: number;
  orderNumber: number;
  pendingReviewNumber: number;

  blockDate: Date | null;
  blockDetails: string | null;
}
