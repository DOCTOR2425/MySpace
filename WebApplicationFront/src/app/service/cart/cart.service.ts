import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, OnDestroy } from '@angular/core';
import { CartItem } from '../../data/interfaces/cart/cart-item.interface';
import {
  asapScheduler,
  forkJoin,
  map,
  Observable,
  scheduled,
  Subject,
  takeUntil,
} from 'rxjs';
import { OrderOptions } from '../../data/interfaces/order-options/order-options.interface';
import { UserOrderInfo } from '../../data/interfaces/user/user-order-info.interface';
import { environment } from '../../../environments/environment.development';
import { AuthService } from '../auth/auth.service';
import { RegisterUserFromOrderRequest } from '../../data/interfaces/user/register-user-from-order-request.interface';
import { AddToCartRequest } from '../../data/interfaces/cart/add-to-cart-request.interface';
import { UserDeliveryAddress } from '../../data/interfaces/user/user-delivery-address.interface';
import { ProductMinimalData } from '../../data/interfaces/product/product-minimal-data.interface';

@Injectable({
  providedIn: 'root',
})
export class CartService implements OnDestroy {
  private cartKey = 'cart';
  private baseApiUrl = environment.apiUrl + '/api/Cart/';
  private unsubscribe$ = new Subject<void>();

  constructor(private http: HttpClient, private authService: AuthService) {}

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public getCartItems(): Observable<CartItem[]> {
    if (this.authService.isLoggedIn() == true) {
      return this.http.get<CartItem[]>(`${this.baseApiUrl}`, {
        withCredentials: true,
      });
    } else {
      return this.convertocalStorageDataToCartItems(
        this.getCartItemsFromLocalStorage()
      );
    }
  }

  private convertocalStorageDataToCartItems(
    payload: { productId: string; quantity: number }[]
  ): Observable<CartItem[]> {
    return this.getProductForUnregestereCart(
      payload.map((item) => item.productId)
    ).pipe(
      takeUntil(this.unsubscribe$),
      map((products) => {
        return products.map((product, index) => ({
          productId: product.productId,
          productName: product.name,
          productPrice: product.price,
          productImage: product.image,
          isProductArchive: product.isArchive,
          quantity: payload[index].quantity,
        }));
      })
    );
  }

  private getCartItemsFromLocalStorage(): {
    productId: string;
    quantity: number;
  }[] {
    let cartItemsJson = localStorage.getItem(this.cartKey);
    return cartItemsJson ? JSON.parse(cartItemsJson) : [];
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
    let cartItems = this.getCartItemsFromLocalStorage();

    if (cartItems.filter((i) => i.productId == payload.productId).length > 0) {
      this.removeFromCartLocalStorage(payload.productId);

      if (payload.quantity > 0) {
        cartItems = this.getCartItemsFromLocalStorage();
        cartItems.push(payload);
        localStorage.setItem(this.cartKey, JSON.stringify(cartItems));
      }
      return;
    }

    cartItems.push(payload);
    localStorage.setItem(this.cartKey, JSON.stringify(cartItems));
  }

  public removeFromCart(productId: string): Observable<Object> {
    if (this.authService.isLoggedIn() == true) {
      return this.http.delete(this.baseApiUrl + productId);
    } else {
      this.removeFromCartLocalStorage(productId);
      return scheduled([Object], asapScheduler);
    }
  }

  private removeFromCartLocalStorage(productId: string): void {
    let cartItems = this.getCartItemsFromLocalStorage();
    cartItems = cartItems.filter((item) => item.productId !== productId);
    localStorage.setItem(this.cartKey, JSON.stringify(cartItems));
  }

  public orderCartForRegistered(payload: {
    deliveryMethodId: string;
    paymentMethod: string;
    userDelivaryAddress: UserDeliveryAddress;
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
    paymentMethod: any;
  }): Observable<{ orderId: string }> {
    return this.http.post<{ orderId: string }>(
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
      surname: '',
      email: '',
      telephone: '',
      userDeliveryAddress: {
        city: '',
        street: '',
        houseNumber: '',
        entrance: '',
        flat: '',
      },
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

  private getProductForUnregestereCart(
    productsId: string[]
  ): Observable<ProductMinimalData[]> {
    let params = new HttpParams();

    productsId.forEach((id) => {
      params = params.append('productsId', id);
    });

    return this.http.get<ProductMinimalData[]>(
      `${this.baseApiUrl}get-product-for-unregestered-cart`,
      { params, withCredentials: true }
    );
  }

  public registerCart() {
    let cartItems = this.getCartItemsFromLocalStorage();

    const cartItemsrequest = cartItems.map((item) => this.addToUserCart(item));
    forkJoin(cartItemsrequest)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((items) => {
        this.clearLocalCart();
      });
  }
}
