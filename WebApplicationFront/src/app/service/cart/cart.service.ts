import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CartItem } from '../../data/interfaces/cartItem.interface';
import { Observable } from 'rxjs';
import { OrderOptions } from '../../data/interfaces/order-options/order-options.interface';
import { UserOrderInfo } from '../../data/interfaces/user/user-order-info.interface';
import { environment } from '../../../environments/environment.development';


@Injectable({
  providedIn: 'root',
})
export class CartService {
  constructor(private http: HttpClient) {}

  baseApiUrl = environment.apiUrl + '/api/Cart/';

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

  public cahngeCart(payload: { productId: string; quantity: number }): void {
    this.http
      .post(`${this.baseApiUrl}add-to-cart`, payload, {
        withCredentials: true,
      })
      .subscribe();
  }

  public removeFromCart(cartItemId: string): void {
    this.http.delete(this.baseApiUrl + cartItemId).subscribe();
  }

  public orderCartForRegistered(payload: {
    deliveryMethodId: string;
    paymentMethodId: string;
  }): void {
    this.http
      .post(`${this.baseApiUrl}order-cart-for-registered`, payload, {
        withCredentials: true,
      })
      .subscribe();
  }

  public getUserOrderInfo(): Observable<UserOrderInfo> {
    return this.http.get<UserOrderInfo>(
      `${this.baseApiUrl}get-user-order-info`,
      {
        withCredentials: true,
      }
    );
  }
}
