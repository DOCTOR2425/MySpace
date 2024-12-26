import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CartItem } from '../../data/interfaces/cartItem.interface';
import { firstValueFrom, Observable } from 'rxjs';
import { OrderOptions } from '../../data/interfaces/orderOptions.interface';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  constructor(private http: HttpClient) {}

  baseApiUrl = 'https://localhost:7295/api/Cart/';

  public getCartItems(): Observable<CartItem[]> {
    return this.http.get<CartItem[]>(`${this.baseApiUrl}`, {
      withCredentials: true,
    });
  }

  public getOrderOptions(): Observable<OrderOptions> {
    return this.http.get<OrderOptions>(`${this.baseApiUrl}get-order-options`, {
      withCredentials: true,
    });
  }

  public async cahngeCart(payload: { productId: string; quantity: number }) {
    this.http
      .post(`${this.baseApiUrl}add-to-cart`, payload, {
        withCredentials: true,
      })
      .subscribe();
  }

  public async removeFromCart(cartItemId: string) {
    this.http.delete(this.baseApiUrl + cartItemId).subscribe();
  }

  public async orderCart(payload: {
    deliveryMethodId: string;
    paymentMethodId: string;
  }) {
    this.http
      .post(`${this.baseApiUrl}order-cart`, payload, {
        withCredentials: true,
      })
      .subscribe();
  }
}
