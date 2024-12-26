import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ProductCard } from '../data/interfaces/productCard.interface';
import { Product } from '../data/interfaces/product.interface';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  constructor(private http: HttpClient) {}

  baseApiUrl = 'https://localhost:7295/api/Product/';

  public getProductCards(): Observable<ProductCard[]> {
    return this.http.get<ProductCard[]>(`${this.baseApiUrl}`);
  }

  public getProductById(id: string): Observable<Product> {
    return this.http.get<Product>(`${this.baseApiUrl}${id}`);
  }
}
