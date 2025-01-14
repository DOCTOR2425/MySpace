import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CartItem } from '../../data/interfaces/cart/cart-item.interface';
import { asapScheduler, Observable, scheduled } from 'rxjs';
import { OrderOptions } from '../../data/interfaces/order-options/order-options.interface';
import { UserOrderInfo } from '../../data/interfaces/user/user-order-info.interface';
import { environment } from '../../../environments/environment.development';
import { AuthService } from '../auth/auth.service';
import { ProductService } from '../product.service';
import { ProductData } from '../../data/interfaces/product/product-data.interface';
import { v4 as uuidv4 } from 'uuid';
import { RegisterUserFromOrderRequest } from '../../data/interfaces/user/register-user-from-order-request.interface';
import { AddToCartRequest } from '../../data/interfaces/cart/add-to-cart-request.interface';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  private cartKey = 'cart';
  constructor(
    private http: HttpClient,
    private authService: AuthService,
    private productService: ProductService
  ) {}

  baseApiUrl = environment.apiUrl + '/api/Cart/';

  public getCartItems(): Observable<CartItem[]> {
    if (this.authService.isLoggedIn() == true) {
      return this.http.get<CartItem[]>(`${this.baseApiUrl}`, {
        withCredentials: true,
      });
    } else {
      return scheduled([this.getCartItemsFromLocalStorage()], asapScheduler);
    }
  }

  private getCartItemsFromLocalStorage(): CartItem[] {
    let cartItemsJson = localStorage.getItem(this.cartKey);
    return cartItemsJson ? JSON.parse(cartItemsJson) : [];
  }

  public cahngeCart(cartItem: CartItem): Observable<Object> {
    if (this.authService.isLoggedIn() == true) {
      return this.http.post(
        `${this.baseApiUrl}change-cart-item-quantity`,
        cartItem,
        {
          withCredentials: true,
        }
      );
    } else {
      this.cahngeCartLocalStorage(cartItem);
      return scheduled([Object], asapScheduler);
    }
  }

  private cahngeCartLocalStorage(cartItem: CartItem): void {
    this.removeFromCartLocalStorage(cartItem.cartItemId);
    if (cartItem.quantity != 0) {
      let cartItems = this.getCartItemsFromLocalStorage();
      cartItems.push(cartItem);
      localStorage.setItem(this.cartKey, JSON.stringify(cartItems));
    }
  }

  public addToUserCart(payload: {
    productId: string;
    quantity: number;
  }): Observable<Object> {
    if (this.authService.isLoggedIn() == true) {
      return this.http.post(`${this.baseApiUrl}add-to-cart`, payload, {
        withCredentials: true,
      });
    } else {
      this.addToUserCartLocalStorage(payload);
      return scheduled([Object], asapScheduler);
    }
  }

  private addToUserCartLocalStorage(payload: {
    productId: string;
    quantity: number;
  }): void {
    const cartItems = this.getCartItemsFromLocalStorage();
    let productData: ProductData;
    this.productService.getProductById(payload.productId).subscribe((val) => {
      productData = val.productResponseData;
      let cartItem = {
        cartItemId: uuidv4(),
        product: productData,
        quantity: 1,
      };
      cartItems.push(cartItem);
      localStorage.setItem(this.cartKey, JSON.stringify(cartItems));
    });
  }

  public removeFromCart(cartItemId: string): Observable<Object> {
    if (this.authService.isLoggedIn() == true) {
      return this.http.delete(this.baseApiUrl + cartItemId);
    } else {
      this.removeFromCartLocalStorage(cartItemId);
      return scheduled([Object], asapScheduler);
    }
  }

  private removeFromCartLocalStorage(itemId: string): void {
    let cartItems = this.getCartItemsFromLocalStorage();
    cartItems = cartItems.filter((item) => item.cartItemId !== itemId);
    localStorage.setItem(this.cartKey, JSON.stringify(cartItems));
  }

  public orderCartForRegistered(payload: {
    deliveryMethodId: string;
    paymentMethodId: string;
  }): Observable<Object> {
    return this.http.post(
      `${this.baseApiUrl}order-cart-for-registered`,
      payload,
      {
        withCredentials: true,
      }
    );
  }

  public orderCartForUnregistered(payload: {
    user: RegisterUserFromOrderRequest;
    cartItems: AddToCartRequest[];
    deliveryMethodId: any;
    paymentMethodId: any;
  }): Observable<Object> {
    return this.http.post(
      `${this.baseApiUrl}order-cart-for-unregistered`,
      payload,
      {
        withCredentials: true,
      }
    );
  }

  public getUserOrderInfo(): Observable<UserOrderInfo> {
    if (this.authService.isLoggedIn() == true) {
      return this.http.get<UserOrderInfo>(
        `${this.baseApiUrl}get-user-order-info`,
        {
          withCredentials: true,
        }
      );
    } else {
      return scheduled([this.getUserOrderInfoLocalStorage()], asapScheduler);
    }
  }

  private getUserOrderInfoLocalStorage(): UserOrderInfo {
    return {
      userId: '',
      firstName: '',
      eMail: '',
      telephone: '',
      city: '',
      street: '',
      houseNumber: '',
      entrance: '',
      flat: '',
    };
  }

  public getOrderOptions(): Observable<OrderOptions> {
    return this.http.get<OrderOptions>(`${this.baseApiUrl}get-order-options`, {
      withCredentials: true,
    });
  }

  public clearLocalCart(): void {
    localStorage.setItem(this.cartKey, '');
  }
}
