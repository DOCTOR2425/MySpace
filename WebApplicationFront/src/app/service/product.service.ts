import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ProductCard } from '../data/interfaces/product/product-card.interface';
import { environment } from '../../environments/environment.development';
import { FilterRequest } from '../data/interfaces/filters/filter-request.interface';
import { CategoryFilters } from '../data/interfaces/filters/category-filters.intervace';
import { FullProductInfoResponse } from '../data/interfaces/product/product-to-update-response.interface';
import { CreateCommentRequest } from '../data/interfaces/Comment/create-comment-request.interface';
import { CommentResponse } from '../data/interfaces/Comment/comment-response.interface';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  baseApiUrl = environment.apiUrl + '/api/Product/';

  constructor(private http: HttpClient) {}

  public getSpecialProductsForUser(): Observable<ProductCard[]> {
    return this.http.get<ProductCard[]>(
      `${this.baseApiUrl}get-special-products-for-user`,
      {
        withCredentials: true,
      }
    );
  }

  public getProductById(id: string): Observable<FullProductInfoResponse> {
    return this.http.get<FullProductInfoResponse>(`${this.baseApiUrl}${id}`, {
      withCredentials: true,
    });
  }

  public getAllProductsCardsByCategory(
    categoryId: string,
    page: number = 1
  ): Observable<{ items: ProductCard[]; totalCount: number }> {
    return this.http.get<{ items: ProductCard[]; totalCount: number }>(
      `${this.baseApiUrl}category/${categoryId}/page${page}`,
      {
        withCredentials: true,
      }
    );
  }

  public getAllProductsCardsByCategoryWithFilters(
    categoryId: string,
    filter: FilterRequest,
    page: number = 1
  ): Observable<{ items: ProductCard[]; totalCount: number }> {
    let serializedFilter = encodeURIComponent(JSON.stringify(filter));
    let url = `category/${categoryId}/page${page}?filters=${serializedFilter}`;
    return this.http.get<{ items: ProductCard[]; totalCount: number }>(
      `${this.baseApiUrl}${url}`,
      {
        withCredentials: true,
      }
    );
  }

  public getCategoryFilters(categoryId: string): Observable<CategoryFilters> {
    return this.http.get<CategoryFilters>(
      `${this.baseApiUrl}category-filters/${categoryId}`,
      {
        withCredentials: true,
      }
    );
  }

  public getProductToUpdate(
    productId: string
  ): Observable<FullProductInfoResponse> {
    return this.http.get<FullProductInfoResponse>(
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

  public getSimmularToProduct(productId: string): Observable<ProductCard[]> {
    return this.http.get<ProductCard[]>(
      `${this.baseApiUrl}get-simmular-to-product/${productId}`,
      { withCredentials: true }
    );
  }

  public getMostPopularProducts(page: number): Observable<ProductCard[]> {
    return this.http.get<ProductCard[]>(
      `${this.baseApiUrl}get-most-popular-products/page${page}`
    );
  }
}
