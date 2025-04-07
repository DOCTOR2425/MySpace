import { Injectable } from '@angular/core';
import { UserProfile } from '../../data/interfaces/user/user-profile';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment.development';
import { UpdateUserRequest } from '../../data/interfaces/user/update-user.interface';
import { UserPaidOrder } from '../../data/interfaces/paid-order/user-paid-order.interface';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private baseApiUrl = environment.apiUrl + '/api/User/';
  public userEMail?: string;
  public userEMailKey: string = 'userEmail';

  constructor(private http: HttpClient) {
    this.userEMail = localStorage.getItem(this.userEMailKey)?.toString();
  }

  public getUser(): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.baseApiUrl}get-user`);
  }

  public updateUser(user: UpdateUserRequest): Observable<UserProfile> {
    return this.http.post<UserProfile>(`${this.baseApiUrl}update-user`, user, {
      withCredentials: true,
    });
  }

  public getPaidOrders(): Observable<UserPaidOrder[]> {
    return this.http.get<UserPaidOrder[]>(`${this.baseApiUrl}get-paid-orders`, {
      withCredentials: true,
    });
  }
}
