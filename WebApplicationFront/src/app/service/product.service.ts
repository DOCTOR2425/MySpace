import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ProductCard } from '../data/interfaces/product/product-card.interface';
import { environment } from '../../environments/environment.development';
import { Product } from '../data/interfaces/product/product.interface';

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
}
