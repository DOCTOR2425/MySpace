import { Component, OnDestroy, OnInit } from '@angular/core';
import { CartService } from '../../service/cart/cart.service';
import { CartItem } from '../../data/interfaces/cartItem.interface';
import { CommonModule } from '@angular/common';
import { CartItemComponent } from './cart-item/cart-item.component';
import { FormsModule, NgForm } from '@angular/forms';
import { OrderOptions } from '../../data/interfaces/order-options/order-options.interface';
import { UserOrderInfo } from '../../data/interfaces/user/user-order-info.interface';
import { forkJoin, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-cart-page',
  imports: [CommonModule, CartItemComponent, FormsModule],
  templateUrl: './cart-page.component.html',
  styleUrls: ['./cart-page.component.scss'],
})
export class CartPageComponent implements OnInit, OnDestroy {
  public items: CartItem[] = [];
  public totalPrice: number = 0;
  public orderOptions!: OrderOptions;
  public userOrderInfo!: UserOrderInfo;
  private unsubscribe$ = new Subject<void>();

  constructor(private cartService: CartService) {}

  public ngOnInit(): void {
    forkJoin({
      cartItems: this.cartService.getCartItems(),
      orderOptions: this.cartService.getOrderOptions(),
      userOrderInfo: this.cartService.getUserOrderInfo(),
    })
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((val) => {
        this.items = val.cartItems;
        this.updateTotalPrice();
        this.orderOptions = val.orderOptions;
        this.userOrderInfo = val.userOrderInfo;
      });
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
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
    this.cartService
      .removeFromCart(item.cartItemId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe();
    this.updateTotalPrice();
  }

  public cahngeItemNumber(item: CartItem): void {
    const cahngeCartRequest = {
      productId: item.product.productId,
      quantity: item.quantity,
    };
    this.cartService
      .cahngeCart(cahngeCartRequest)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe();
    this.updateTotalPrice();
  }

  public orderCart(form: NgForm): void {
    let payload = {
      deliveryMethodId: form.value.deliveryMethodId,
      paymentMethodId: form.value.paymentMethodId,
    };
    this.cartService
      .orderCartForRegistered(payload)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe();

    this.items = [];
    this.updateTotalPrice();
  }
}
