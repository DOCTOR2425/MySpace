import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CartItem } from '../../data/interfaces/cartItem.interface';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  constructor(private http: HttpClient) {}

  baseApiUrl = 'https://localhost:7295/api/Cart';

  public getCartItems() {
    return this.http.get<CartItem[]>(this.baseApiUrl, { withCredentials: true });
  }
}
