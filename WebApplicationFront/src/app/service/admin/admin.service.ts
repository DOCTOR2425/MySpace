import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment.development';
import { AdminPaidOrder } from '../../data/interfaces/paid-order/admin-paid-order.interface';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  public isAdmin: boolean = false;
  private baseApiUrl = environment.apiUrl + '/api/Admin/';

  constructor(private http: HttpClient) {}

  public getProcessingOrders(): Observable<AdminPaidOrder[]> {
    return this.http.get<AdminPaidOrder[]>(
      `${this.baseApiUrl}get-processing-orders`
    );
  }

  public closeOrder(orderId: string): Observable<object> {
    return this.http.put(`${this.baseApiUrl}close-order${orderId}`, {
      withCredentials: true,
    });
  }

  public cancelOrder(orderId: string): Observable<object> {
    return this.http.put(`${this.baseApiUrl}cancel-order${orderId}`, {
      withCredentials: true,
    });
  }
}
