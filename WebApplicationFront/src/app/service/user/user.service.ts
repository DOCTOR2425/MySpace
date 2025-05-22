import { Injectable } from '@angular/core';
import { UserProfile } from '../../data/interfaces/user/user-profile';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment.development';
import { UpdateUserRequest } from '../../data/interfaces/user/update-user.interface';
import { UserPaidOrder } from '../../data/interfaces/paid-order/user-paid-order.interface';
import { UserProductCard } from '../../data/interfaces/product/user-product-card.interface';
import { UserProductStats } from '../../data/interfaces/product/user-product-stats.interface';
import { UserCommentResponse } from '../../data/interfaces/comment/user-comment-response.interface';
import { AdminUser } from '../../data/interfaces/user/admin-user.interface';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private baseApiUrl = environment.apiUrl + '/api/User/';
  public userEMailKey: string = 'userEmail';

  constructor(private http: HttpClient) {}

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

  public getUserComments(
    userId: string | null = null
  ): Observable<UserCommentResponse[]> {
    const params = new URLSearchParams({
      userId: userId ? userId : '',
    });

    return this.http.get<UserCommentResponse[]>(
      `${this.baseApiUrl}get-user-comments?${params}`,
      { withCredentials: true }
    );
  }

  public getOrderedProductsPendingReviews(): Observable<UserProductCard[]> {
    return this.http.get<UserProductCard[]>(
      `${this.baseApiUrl}get-ordered-products-pending-reviews`,
      { withCredentials: true }
    );
  }

  public getUserProductStats(productId: string): Observable<UserProductStats> {
    return this.http.get<UserProductStats>(
      `${this.baseApiUrl}get-user-product-stats/${productId}`,
      { withCredentials: true }
    );
  }

  public getUsersForAdmin(
    page: number,
    searchQuery: string | undefined,
    dateFrom: Date | undefined,
    dateTo: Date | undefined,
    isBlocked: boolean | undefined,
    hasOrders: boolean | undefined
  ): Observable<AdminUser[]> {
    const params = new URLSearchParams({
      page: page.toString(),
      searchQuery: searchQuery ? searchQuery : '',
      dateFrom: dateFrom ? dateFrom.toString() : '',
      dateTo: dateTo ? dateTo.toString() : '',
      isBlocked: isBlocked !== undefined ? isBlocked.toString() : '',
      hasOrders: hasOrders !== undefined ? hasOrders.toString() : '',
    });

    return this.http.get<AdminUser[]>(
      `${this.baseApiUrl}get-users-for-admin?${params}`,
      { withCredentials: true }
    );
  }

  public getUserForAdmin(userId: string): Observable<AdminUser> {
    return this.http.get<AdminUser>(
      `${this.baseApiUrl}get-user-for-admin/${userId}`,
      { withCredentials: true }
    );
  }

  public blockUser(userId: string, blockDetails: string): Observable<any> {
    return this.http.put(
      `${this.baseApiUrl}block-user/${userId}?` +
        `blockDetails=${blockDetails}`,
      null,
      { withCredentials: true }
    );
  }

  public unblockUser(userId: string): Observable<any> {
    return this.http.put(`${this.baseApiUrl}unblock-user/${userId}?`, null, {
      withCredentials: true,
    });
  }
}
