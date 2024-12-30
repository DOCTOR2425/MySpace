import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ProductCard } from '../data/interfaces/productCard.interface';
import { Product } from '../data/interfaces/product.interface';
import { environment } from '../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  constructor(private http: HttpClient) {}

  baseApiUrl = environment.apiUrl + '/api/Product/';

  public getProductCards(): Observable<ProductCard[]> {
    return this.http.get<ProductCard[]>(`${this.baseApiUrl}`);
  }

  public getProductById(id: string): Observable<Product> {
    return this.http.get<Product>(`${this.baseApiUrl}${id}`);
  }
}
