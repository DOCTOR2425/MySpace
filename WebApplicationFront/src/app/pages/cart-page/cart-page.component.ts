import { Component } from '@angular/core';
import { CartService } from '../../service/cart/cart.service';
import { CartItem } from '../../data/interfaces/cartItem.interface';

@Component({
  selector: 'app-cart-page',
  imports: [],
  templateUrl: './cart-page.component.html',
  styleUrl: './cart-page.component.scss',
})
export class CartPageComponent {
  constructor(private cartService: CartService) {}

  items: CartItem[] = [];

  public async view() {
    try {
      const cartItems = await this.cartService.getCartItems();
      console.log('Cart Items:', cartItems);
      if (cartItems.length > 0) {
        console.log('First Cart Item ID:', cartItems[0].cartItemId);
      } else {
        console.log('No items in the cart.');
      }
    } catch (error) {
      console.error('Error in view method:', error);
    }
  }
}
