import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment.development';
import { AdminPaidOrder } from '../../data/interfaces/paid-order/admin-paid-order.interface';
import { ProductProperty } from '../../data/interfaces/product/product-property.interface';
import { OptionsForProduct } from '../../data/interfaces/some/options-for-order.interface';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  public isAdmin: boolean = false;
  private baseApiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  public getProcessingOrders(): Observable<AdminPaidOrder[]> {
    return this.http.get<AdminPaidOrder[]>(
      `${this.baseApiUrl}/api/Admin/get-processing-orders`
    );
  }

  public closeOrder(orderId: string): Observable<object> {
    return this.http.put(`${this.baseApiUrl}/api/Admin/close-order${orderId}`, {
      withCredentials: true,
    });
  }

  public cancelOrder(orderId: string): Observable<object> {
    return this.http.put(
      `${this.baseApiUrl}/api/Admin/cancel-order${orderId}`,
      {
        withCredentials: true,
      }
    );
  }

  public getOptionsForProduct(): Observable<OptionsForProduct> {
    return this.http.get<OptionsForProduct>(
      `${this.baseApiUrl}/api/Admin/get-options-for-product`,
      { withCredentials: true }
    );
  }

  public getProductPropertiesByCategory(
    categoryId: string
  ): Observable<ProductProperty[]> {
    return this.http.get<ProductProperty[]>(
      `${this.baseApiUrl}/api/ProductCategory/get-properties-by-category/${categoryId}`,
      {
        withCredentials: true,
      }
    );
  }
}
