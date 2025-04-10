import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ProductCategory } from '../../data/interfaces/some/product-category.interface';
import { environment } from '../../../environments/environment.development';
import { ProductCategoryForAdmin } from '../../data/interfaces/product-category/product-category-for-admin.interface';
import { ProductCategoryCreateRequest } from '../../data/interfaces/product-category/product-category-create-request.interface';
import { ProductCategoryDTOUpdate } from '../../data/interfaces/product-category/product-category-update-request.interface';

@Injectable({
  providedIn: 'root',
})
export class ProductCategoryService {
  private baseApiUrl = environment.apiUrl + '/api/ProductCategory/';

  constructor(private http: HttpClient) {}

  public getAllCategories(): Observable<ProductCategory[]> {
    return this.http.get<ProductCategory[]>(
      `${this.baseApiUrl}/api/ProductCategory/get-all-categories`,
      { withCredentials: true }
    );
  }

  public getTopCategoriesBySales(): Observable<ProductCategory[]> {
    return this.http.get<ProductCategory[]>(
      `${this.baseApiUrl}get-top-categories-by-sales`
    );
  }

  public getProductCategoriesForAdmin(): Observable<ProductCategoryForAdmin[]> {
    return this.http.get<ProductCategoryForAdmin[]>(
      `${this.baseApiUrl}get-categories-for-admin`,
      { withCredentials: true }
    );
  }

  public changeVisibilityStatus(
    categoryId: string,
    visibilityStatus: boolean
  ): Observable<object> {
    return this.http.post(
      `${this.baseApiUrl}change-visibility-status/${categoryId}?visibilityStatus=${visibilityStatus}`,
      { withCredentials: true }
    );
  }

  public createCategory(
    payload: ProductCategoryCreateRequest
  ): Observable<object> {
    return this.http.post(`${this.baseApiUrl}create-category`, payload, {
      withCredentials: true,
    });
  }

  public updateCategory(
    categoryId: string,
    payload: ProductCategoryDTOUpdate
  ): Observable<object> {
    return this.http.post(
      `${this.baseApiUrl}update-category/${categoryId}`,
      payload,
      {
        withCredentials: true,
      }
    );
  }

  public getProductCategoryForUpdate(
    categoryId: string
  ): Observable<ProductCategoryDTOUpdate> {
    return this.http.get<ProductCategoryDTOUpdate>(
      `${this.baseApiUrl}get-category-for-update/${categoryId}`,
      { withCredentials: true }
    );
  }
}
