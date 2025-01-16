import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ProductCard } from '../data/interfaces/product/product-card.interface';
import { environment } from '../../environments/environment.development';
import { Product } from '../data/interfaces/product/product.interface';
import { FilterRequest } from '../data/interfaces/filters/filter-request.interface';
import { CategoryFilters } from '../data/interfaces/filters/category-filters.intervace';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  baseApiUrl = environment.apiUrl + '/api/Product/';

  constructor(private http: HttpClient) {}

  public getProductCards(): Observable<ProductCard[]> {
    return this.http.get<ProductCard[]>(`${this.baseApiUrl}`);
  }

  public getProductById(id: string): Observable<Product> {
    return this.http.get<Product>(`${this.baseApiUrl}${id}`);
  }

  public getAllProductsCardsByCategory(
    categoryName: string
  ): Observable<ProductCard[]> {
    return this.http.get<ProductCard[]>(
      `${this.baseApiUrl}category/${categoryName}`
    );
  }

  public getAllProductsCardsByCategoryWithFilters(
    categoryName: string,
    filter: FilterRequest
  ): Observable<ProductCard[]> {
    let serializedFilter = encodeURIComponent(JSON.stringify(filter));
    let url = `category/${categoryName}?filters=${serializedFilter}`;
    console.log(serializedFilter);

    return this.http.get<ProductCard[]>(`${this.baseApiUrl}${url}`);
  }

  public getCategoryFilters(categoryName: string): Observable<CategoryFilters> {
    return this.http.get<CategoryFilters>(
      `${this.baseApiUrl}category-filters/${categoryName}`
    );
  }
}
