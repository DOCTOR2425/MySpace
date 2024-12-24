import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CartItem } from '../../data/interfaces/cartItem.interface';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  constructor(private http: HttpClient) {}

  baseApiUrl = 'https://localhost:7295/api/Cart/';

  public async getCartItems(): Promise<CartItem[]> {
    try {
      const response = await firstValueFrom(
        this.http.get<CartItem[]>(`${this.baseApiUrl}`, {
          withCredentials: true,
        })
      );
      return response;
    } catch (error) {
      console.error('Error fetching cart items:', error);
      return [];
    }
  }

  public async addToCart(payload: { productId: string; quantity: number }) {
    this.http
      .post(`${this.baseApiUrl}add-to-cart`, payload, { withCredentials: true })
      .subscribe();
  }

  public async removeFromCart(cartItemId: string){
    this.http.delete(this.baseApiUrl + cartItemId).subscribe();
  }
}
