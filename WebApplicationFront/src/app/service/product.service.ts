import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Product } from '../data/interfaces/product.interface';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  constructor (private http: HttpClient) {}

  baseApiUrl = 'https://localhost:7295/api/Product'

  public getTestProduct(): Observable<Product[]>
  {
    return this.http.get<Product[]>(`${this.baseApiUrl}`)
  }
}
