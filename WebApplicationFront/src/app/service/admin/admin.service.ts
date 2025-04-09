import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment.development';
import { AdminPaidOrder } from '../../data/interfaces/paid-order/admin-paid-order.interface';
import { ProductProperty } from '../../data/interfaces/product/product-property.interface';
import { OptionsForProduct } from '../../data/interfaces/some/options-for-order.interface';
import { ProductCategory } from '../../data/interfaces/some/product-category.interface';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  public isAdmin: boolean = false;
  private baseApiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  public getProcessingOrders(): Observable<AdminPaidOrder[]> {
    return this.http.get<AdminPaidOrder[]>(
      `${this.baseApiUrl}/api/Admin/get-processing-orders`,
      {
        withCredentials: true,
      }
    );
  }

  public getOrderById(id: string): Observable<AdminPaidOrder> {
    return this.http.get<AdminPaidOrder>(
      `${this.baseApiUrl}/api/PaidOrder/get-order-by-id/${id}`,
      {
        withCredentials: true,
      }
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

  public generateWordReport(from: string, to: string): Observable<Blob> {
    return this.http.get(
      `${this.baseApiUrl}/api/Admin/generate-word-report` +
        `?from=${from}&to=${to}`,
      {
        withCredentials: true,
        responseType: 'blob',
      }
    );
  }

  public generateReportSalesByCategoryOverTime(
    from: string,
    to: string
  ): Observable<Blob> {
    return this.http.get(
      `${this.baseApiUrl}/api/Admin/generate-report-sales-by-category-over-time` +
        `?from=${from}&to=${to}`,
      {
        withCredentials: true,
        responseType: 'blob',
      }
    );
  }

  public generateExcelStockReport(from: string, to: string): Observable<Blob> {
    return this.http.get(
      `${this.baseApiUrl}/api/Admin/generate-stock-report-over-time` +
        `?from=${from}&to=${to}`,
      {
        withCredentials: true,
        responseType: 'blob',
      }
    );
  }

  public getAllOrders(page: number): Observable<AdminPaidOrder[]> {
    return this.http.get<AdminPaidOrder[]>(
      `${this.baseApiUrl}/api/PaidOrder/get-all-orders?page=${page}`,
      {
        withCredentials: true,
      }
    );
  }
}
