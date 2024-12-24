import { Component } from '@angular/core';
import { CartService } from '../../service/cart/cart.service';
import { CartItem } from '../../data/interfaces/cartItem.interface';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { CartItemComponent } from "./cart-item/cart-item.component";

@Component({
  selector: 'app-cart-page',
  imports: [CommonModule, CartItemComponent],
  templateUrl: './cart-page.component.html',
  styleUrl: './cart-page.component.scss',
})
export class CartPageComponent {
  items: CartItem[] = [];
  constructor(private router: Router, private cartService: CartService) {}
  async ngOnInit() {
    this.items = await this.cartService.getCartItems();
    console.log('Cart Items:', this.items)
  }
}
