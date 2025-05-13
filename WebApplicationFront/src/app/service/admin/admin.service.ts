import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment.development';
import { AdminPaidOrder } from '../../data/interfaces/paid-order/admin-paid-order.interface';
import { ProductProperty } from '../../data/interfaces/product/product-property.interface';
import { OptionsForProduct } from '../../data/interfaces/some/options-for-order.interface';
import { Brand } from '../../data/interfaces/some/brand.interface';
import { Country } from '../../data/interfaces/some/country.interface';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  public isAdmin: boolean = false;
  private baseApiUrl = environment.apiUrl + '/api/';

  constructor(private http: HttpClient) {}

  public getProcessingOrders(): Observable<AdminPaidOrder[]> {
    return this.http.get<AdminPaidOrder[]>(
      `${this.baseApiUrl}Admin/get-processing-orders`,
      {
        withCredentials: true,
      }
    );
  }

  public getOrderById(id: string): Observable<AdminPaidOrder> {
    return this.http.get<AdminPaidOrder>(
      `${this.baseApiUrl}PaidOrder/get-order-by-id/${id}`,
      {
        withCredentials: true,
      }
    );
  }

  public closeOrder(orderId: string): Observable<object> {
    return this.http.put(`${this.baseApiUrl}Admin/close-order${orderId}`, {
      withCredentials: true,
    });
  }

  public cancelOrder(orderId: string): Observable<object> {
    return this.http.put(`${this.baseApiUrl}Admin/cancel-order${orderId}`, {
      withCredentials: true,
    });
  }

  public getOptionsForProduct(): Observable<OptionsForProduct> {
    return this.http.get<OptionsForProduct>(
      `${this.baseApiUrl}Admin/get-options-for-product`,
      { withCredentials: true }
    );
  }

  public getProductPropertiesByCategory(
    categoryId: string
  ): Observable<ProductProperty[]> {
    return this.http.get<ProductProperty[]>(
      `${this.baseApiUrl}ProductCategory/get-properties-by-category/${categoryId}`,
      {
        withCredentials: true,
      }
    );
  }

  public getAllOrders(page: number): Observable<AdminPaidOrder[]> {
    return this.http.get<AdminPaidOrder[]>(
      `${this.baseApiUrl}PaidOrder/get-all-orders?page=${page}`,
      {
        withCredentials: true,
      }
    );
  }

  public createBrand(brandName: string): Observable<any> {
    return this.http.post(
      `${this.baseApiUrl}Admin/create-brand/${brandName}`,
      null,
      { withCredentials: true }
    );
  }

  public createCountry(countryName: string): Observable<any> {
    return this.http.post(
      `${this.baseApiUrl}Admin/create-country/${countryName}`,
      null,
      { withCredentials: true }
    );
  }

  public getAllBrands(): Observable<Brand[]> {
    return this.http.get<Brand[]>(`${this.baseApiUrl}Admin/get-all-brands`, {
      withCredentials: true,
    });
  }

  public getAllCountries(): Observable<Country[]> {
    return this.http.get<Country[]>(
      `${this.baseApiUrl}Admin/get-all-countries`,
      { withCredentials: true }
    );
  }

  public updateBrand(brandId: string, newName: string): Observable<any> {
    return this.http.put(
      `${this.baseApiUrl}Admin/update-brand/${brandId}/${newName}`,
      null,
      { withCredentials: true }
    );
  }

  public updateCountry(countryId: string, newName: string): Observable<any> {
    return this.http.put(
      `${this.baseApiUrl}Admin/update-country/${countryId}/${newName}`,
      null,
      { withCredentials: true }
    );
  }

  public uploadImage(name: string): Observable<Blob> {
    return this.http.get(`${this.baseApiUrl}Admin/upload-image/${name}`, {
      responseType: 'blob',
      withCredentials: true,
    });
  }
}
