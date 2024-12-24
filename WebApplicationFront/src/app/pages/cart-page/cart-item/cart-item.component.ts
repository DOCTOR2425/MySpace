import { Component, Input } from '@angular/core';
import { CartItem } from '../../../data/interfaces/cartItem.interface';
import { CartService } from '../../../service/cart/cart.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-cart-item',
  imports: [CommonModule],
  templateUrl: './cart-item.component.html',
  styleUrl: './cart-item.component.scss',
})
export class CartItemComponent {
  @Input() item!: CartItem;

  constructor(private cartService: CartService) {}

  public changeQuantity(isIncrease: boolean, itemId: string) {
    let counterElement = document.getElementById(`counter${itemId}`);
    let decBtnElement = document.getElementById(
      `decBtn${itemId}`
    ) as HTMLButtonElement;
    if (counterElement && counterElement.textContent !== null) {
      let counter = parseInt(counterElement.textContent, 10);
      counter = isIncrease ? counter + 1 : counter - 1;

      if (counter > 0) {
        counterElement.textContent = counter.toString();
        this.cahngeItemNumber(counter);
        if (decBtnElement) {
          decBtnElement.disabled = false;
          decBtnElement.style.backgroundColor = ''; // Убираем серый цвет
        }
      } else {
        counterElement.textContent = '0';
        if (decBtnElement) {
          decBtnElement.disabled = true;
          decBtnElement.style.backgroundColor = 'grey'; // Делаем кнопку серой
        }
      }
    }
  }

  public removeCartItem(itemId: string) {
    let cartItemElement = document.querySelector(
      `.cart-item[data-id="${itemId}"]`
    );
    if (cartItemElement) {
      this.cartService.removeFromCart(itemId);
      cartItemElement.remove();
    }
  }

  public cahngeItemNumber(quantity: number): void {
    const addToCartRequest = {
      productId: this.item.product.productId,
      quantity: quantity,
    };
    this.cartService.addToCart(addToCartRequest);
  }
}
