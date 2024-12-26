import { Component, OnInit } from '@angular/core';
import { CartService } from '../../service/cart/cart.service';
import { CartItem } from '../../data/interfaces/cartItem.interface';
import { CommonModule } from '@angular/common';
import { CartItemComponent } from './cart-item/cart-item.component';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-cart-page',
  imports: [CommonModule, CartItemComponent, FormsModule],
  templateUrl: './cart-page.component.html',
  styleUrls: ['./cart-page.component.scss'],
})
export class CartPageComponent implements OnInit {
  items: CartItem[] = [];
  totalPrice: number = 0;
  selectedDeliveryMethod: string = '';
  selectedPaymentMethod: string = '';

  constructor(private cartService: CartService) {}

  public ngOnInit(): void {
    this.cartService.getCartItems().subscribe({
      next: (cartItems) => {
        this.items = cartItems;
        this.updateTotalPrice();
      },
      error: (error) => console.log(error),
    });
  }

  public updateTotalPrice(): void {
    this.totalPrice = this.items.reduce(
      (total, item) => total + item.product.price * item.quantity,
      0
    );
  }

  public changeItemQuantity(item: CartItem, isIncrease: boolean): void {
    if (isIncrease) {
      item.quantity += 1;
      this.cahngeItemNumber(item);
    } else if (item.quantity > 1) {
      item.quantity -= 1;
      this.cahngeItemNumber(item);
    }
  }

  public removeCartItem(item: CartItem): void {
    this.items = this.items.filter(
      (itemTarget) => itemTarget.cartItemId !== item.cartItemId
    );
    this.cartService.removeFromCart(item.cartItemId);
    this.updateTotalPrice();
  }

  public cahngeItemNumber(item: CartItem): void {
    const cahngeCartRequest = {
      productId: item.product.productId,
      quantity: item.quantity,
    };
    this.cartService.cahngeCart(cahngeCartRequest);
    this.updateTotalPrice();
  }

  public selectDelivery(method: string): void {
    this.selectedDeliveryMethod = method;
  }

  public selectPayment(method: string): void {
    this.selectedPaymentMethod = method;
  }

  public onSubmit(form: any): void {
    console.log('Form Data:', form);
    alert('Форма отправлена!');
  }
}
