import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UserProductCard } from '../data/interfaces/product/user-product-card.interface';
import { environment } from '../../environments/environment.development';
import { FilterRequest } from '../data/interfaces/filters/filter-request.interface';
import { CategoryFilters } from '../data/interfaces/filters/category-filters.intervace';
import { FullProductInfoResponse } from '../data/interfaces/product/product-to-update-response.interface';
import { CreateCommentRequest } from '../data/interfaces/comment/create-comment-request.interface';
import { CommentResponse } from '../data/interfaces/comment/comment-response.interface';
import { ProductMinimalData } from '../data/interfaces/product/product-minimal-data.interface';
import { AdminProductCard } from '../data/interfaces/product/admin-product-card.interface';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  baseApiUrl = environment.apiUrl + '/api/Product/';

  constructor(private http: HttpClient) {}

  public getSpecialProductsForUser(): Observable<UserProductCard[]> {
    return this.http.get<UserProductCard[]>(
      `${this.baseApiUrl}get-special-products-for-user`,
      { withCredentials: true }
    );
  }

  public getProductById(id: string): Observable<FullProductInfoResponse> {
    return this.http.get<FullProductInfoResponse>(
      `${this.baseApiUrl}get-product/${id}`,
      { withCredentials: true }
    );
  }

  public getProductMinimalData(id: string): Observable<ProductMinimalData> {
    return this.http.get<ProductMinimalData>(
      `${this.baseApiUrl}get-product-minimal-data/${id}`,
      { withCredentials: true }
    );
  }

  public getAllProductsCardsByCategory(
    categoryId: string,
    page: number = 1
  ): Observable<{ items: UserProductCard[]; totalCount: number }> {
    return this.http.get<{ items: UserProductCard[]; totalCount: number }>(
      `${this.baseApiUrl}category/${categoryId}/page${page}`,
      { withCredentials: true }
    );
  }

  public getAllProductsCardsByCategoryWithFilters(
    categoryId: string,
    filter: FilterRequest,
    page: number = 1
  ): Observable<{ items: UserProductCard[]; totalCount: number }> {
    let serializedFilter = encodeURIComponent(JSON.stringify(filter));
    let url = `category/${categoryId}/page${page}?filters=${serializedFilter}`;
    return this.http.get<{ items: UserProductCard[]; totalCount: number }>(
      `${this.baseApiUrl}${url}`,
      { withCredentials: true }
    );
  }

  public getCategoryFilters(categoryId: string): Observable<CategoryFilters> {
    return this.http.get<CategoryFilters>(
      `${this.baseApiUrl}category-filters/${categoryId}`,
      { withCredentials: true }
    );
  }

  public createProduct(product: FormData): Observable<object> {
    return this.http.post(`${this.baseApiUrl}create-product`, product, {
      withCredentials: true,
    });
  }

  public getProductsForAdmin(page: number): Observable<AdminProductCard[]> {
    return this.http.get<AdminProductCard[]>(
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
      { withCredentials: true }
    );
  }

  public searchByName(
    name: string,
    page: number
  ): Observable<UserProductCard[]> {
    return this.http.get<UserProductCard[]>(
      `${this.baseApiUrl}search/page${page}?name=${name}`,
      { withCredentials: true }
    );
  }

  public searchByNameWithArchive(
    name: string,
    page: number
  ): Observable<AdminProductCard[]> {
    return this.http.get<AdminProductCard[]>(
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
      { withCredentials: true }
    );
  }

  public addComment(request: CreateCommentRequest): Observable<object> {
    return this.http.post(`${this.baseApiUrl}add-comment`, request, {
      withCredentials: true,
    });
  }

  public getCommentsByProduct(
    productId: string
  ): Observable<CommentResponse[]> {
    return this.http.get<CommentResponse[]>(
      `${this.baseApiUrl}get-comments-by-product/${productId}`,
      { withCredentials: true }
    );
  }

  public getSimmularToProduct(
    productId: string
  ): Observable<UserProductCard[]> {
    return this.http.get<UserProductCard[]>(
      `${this.baseApiUrl}get-simmular-to-product/${productId}`,
      { withCredentials: true }
    );
  }

  public getMostPopularProducts(page: number): Observable<UserProductCard[]> {
    return this.http.get<UserProductCard[]>(
      `${this.baseApiUrl}get-most-popular-products/page${page}`
    );
  }
}
