import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ProductCard } from '../data/interfaces/product/product-card.interface';
import { environment } from '../../environments/environment.development';
import { Product } from '../data/interfaces/product/product.interface';
import { FilterRequest } from '../data/interfaces/filters/filter-request.interface';
import { CategoryFilters } from '../data/interfaces/filters/category-filters.intervace';
import { ProductData } from '../data/interfaces/product/product-data.interface';
import { ProductToUpdate } from '../data/interfaces/product/product-to-update-response.interface';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  baseApiUrl = environment.apiUrl + '/api/Product/';

  constructor(private http: HttpClient) {}

  public getProductCards(page: number = 1): Observable<ProductCard[]> {
    return this.http.get<ProductCard[]>(`${this.baseApiUrl}page${page}`, {
      withCredentials: true,
    });
  }

  public getProductById(id: string): Observable<Product> {
    return this.http.get<Product>(`${this.baseApiUrl}${id}`, {
      withCredentials: true,
    });
  }

  public getAllProductsCardsByCategory(
    categoryName: string,
    page: number = 1
  ): Observable<ProductCard[]> {
    return this.http.get<ProductCard[]>(
      `${this.baseApiUrl}category/${categoryName}/page${page}`,
      {
        withCredentials: true,
      }
    );
  }

  public getAllProductsCardsByCategoryWithFilters(
    categoryName: string,
    filter: FilterRequest,
    page: number = 1
  ): Observable<ProductCard[]> {
    let serializedFilter = encodeURIComponent(JSON.stringify(filter));
    let url = `category/${categoryName}/page${page}?filters=${serializedFilter}`;
    return this.http.get<ProductCard[]>(`${this.baseApiUrl}${url}`, {
      withCredentials: true,
    });
  }

  public getCategoryFilters(categoryName: string): Observable<CategoryFilters> {
    return this.http.get<CategoryFilters>(
      `${this.baseApiUrl}category-filters/${categoryName}`,
      {
        withCredentials: true,
      }
    );
  }

  public getProductToUpdate(productId: string): Observable<ProductToUpdate> {
    return this.http.get<ProductToUpdate>(
      `${this.baseApiUrl}get-product-to-update/${productId}`,
      { withCredentials: true }
    );
  }

  public createProduct(product: FormData): Observable<object> {
    return this.http.post(`${this.baseApiUrl}create-product`, product, {
      withCredentials: true,
    });
  }

  public getProductsForAdmin(page: number): Observable<ProductCard[]> {
    return this.http.get<ProductCard[]>(
      `${this.baseApiUrl}get-products-for-admin${page}`
    );
  }

  public updateProduct(
    productId: string,
    product: FormData
  ): Observable<object> {
    return this.http.put(
      `${this.baseApiUrl}update-product/${productId}`,
      product,
      {
        withCredentials: true,
      }
    );
  }

  public searchByName(name: string, page: number): Observable<ProductCard[]> {
    return this.http.get<ProductCard[]>(
      `${this.baseApiUrl}search/page${page}?name=${name}`,
      { withCredentials: true }
    );
  }

  public searchByNameWithArchive(
    name: string,
    page: number
  ): Observable<ProductCard[]> {
    return this.http.get<ProductCard[]>(
      `${this.baseApiUrl}search-with-archive/page${page}?name=${name}`,
      { withCredentials: true }
    );
  }

  public changeArchiveStatus(
    productId: string,
    archiveStatus: boolean
  ): Observable<object> {
    return this.http.put(
      `${this.baseApiUrl}change-archive-status/${productId}?archiveStatus=${archiveStatus}`,
      {
        withCredentials: true,
      }
    );
  }
}
