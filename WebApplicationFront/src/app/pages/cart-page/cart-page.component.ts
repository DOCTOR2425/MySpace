import { Component, OnDestroy, OnInit } from '@angular/core';
import { CartService } from '../../service/cart/cart.service';
import { CartItem } from '../../data/interfaces/cart/cart-item.interface';
import { CommonModule } from '@angular/common';
import { CartItemComponent } from './cart-item/cart-item.component';
import { FormsModule, NgForm } from '@angular/forms';
import { OrderOptions } from '../../data/interfaces/order-options/order-options.interface';
import { UserOrderInfo } from '../../data/interfaces/user/user-order-info.interface';
import { forkJoin, Subject, takeUntil } from 'rxjs';
import { AuthService } from '../../service/auth/auth.service';
import { RegisterUserFromOrderRequest } from '../../data/interfaces/user/register-user-from-order-request.interface';
import { AddToCartRequest } from '../../data/interfaces/cart/add-to-cart-request.interface';

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

  constructor(
    private cartService: CartService,
    private authService: AuthService
  ) {}

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

  public cahngeItemNumber(cartItem: CartItem): void {
    this.cartService
      .cahngeCart(cartItem)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe();
    this.updateTotalPrice();
  }

  public orderCart(form: NgForm): void {
    if (this.authService.isLoggedIn() == true) {
      let payload = {
        deliveryMethodId: form.value.deliveryMethodId,
        paymentMethodId: form.value.paymentMethodId,
      };
      this.cartService
        .orderCartForRegistered(payload)
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe();
    } else {
      this.orderCartForUnregistered(form);
    }
    this.items = [];
    this.updateTotalPrice();
  }

  private orderCartForUnregistered(form: NgForm): void {
    let user: RegisterUserFromOrderRequest = {
      firstName: form.value.firstName,
      surname: '',
      telephone: form.value.telephone,
      eMail: form.value.email,

      city: form.value.city,
      street: form.value.street,
      houseNumber: form.value.houseNumber,
      entrance: '',
      flat: form.value.flat,
    };

    let cartItems: AddToCartRequest[] = [];
    this.cartService.getCartItems().subscribe((val) => {
      val.forEach((item) => {
        let newItem: AddToCartRequest = {
          productId: item.product.productId,
          quantity: item.quantity,
        };
        cartItems.push(newItem);
      });

      let payload = {
        user,
        cartItems,
        deliveryMethodId: form.value.deliveryMethodId,
        paymentMethodId: form.value.paymentMethodId,
      };

      this.cartService.orderCartForUnregistered(payload).subscribe();
      this.cartService.clearLocalCart();
    });
  }
}
