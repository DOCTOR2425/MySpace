import { Component } from '@angular/core';
import { CartService } from '../../service/cart/cart.service';

@Component({
  selector: 'app-cart-page',
  imports: [],
  templateUrl: './cart-page.component.html',
  styleUrl: './cart-page.component.scss',
})
export class CartPageComponent {
  constructor(private cartService: CartService) {}

  public view() {
    this.cartService.getCartItems().subscribe(t =>
    {
      alert(t[0].cartItemId);
    }
    );
  }
}
