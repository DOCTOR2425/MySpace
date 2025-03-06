import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ProductCard } from '../data/interfaces/product/product-card.interface';
import { environment } from '../../environments/environment.development';
import { Product } from '../data/interfaces/product/product.interface';
import { FilterRequest } from '../data/interfaces/filters/filter-request.interface';
import { CategoryFilters } from '../data/interfaces/filters/category-filters.intervace';
import { ProductToUpdateResponse } from '../data/interfaces/product/product-toupdate-response.interface';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  baseApiUrl = environment.apiUrl + '/api/Product/';

  constructor(private http: HttpClient) {}

  public getProductCards(page: number = 1): Observable<ProductCard[]> {
    return this.http.get<ProductCard[]>(`${this.baseApiUrl}page${page}`);
  }

  public getProductById(id: string): Observable<Product> {
    return this.http.get<Product>(`${this.baseApiUrl}${id}`);
  }

  public getAllProductsCardsByCategory(
    categoryName: string,
    page: number = 1
  ): Observable<ProductCard[]> {
    return this.http.get<ProductCard[]>(
      `${this.baseApiUrl}category/${categoryName}/page${page}`
    );
  }

  public getAllProductsCardsByCategoryWithFilters(
    categoryName: string,
    filter: FilterRequest,
    page: number = 1
  ): Observable<ProductCard[]> {
    let serializedFilter = encodeURIComponent(JSON.stringify(filter));
    let url = `category/${categoryName}/page${page}?filters=${serializedFilter}`;
    return this.http.get<ProductCard[]>(`${this.baseApiUrl}${url}`);
  }

  public getCategoryFilters(categoryName: string): Observable<CategoryFilters> {
    return this.http.get<CategoryFilters>(
      `${this.baseApiUrl}category-filters/${categoryName}`
    );
  }

  public getProductToUpdate(
    productId: string
  ): Observable<ProductToUpdateResponse> {
    return this.http.get<ProductToUpdateResponse>(
      `${this.baseApiUrl}get-product-to-update/${productId}`,
      { withCredentials: true }
    );
  }
}
