import { Component, OnDestroy, OnInit } from '@angular/core';
import { CartService } from '../../service/cart/cart.service';
import { CartItem } from '../../data/interfaces/cart/cart-item.interface';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { OrderOptions } from '../../data/interfaces/order-options/order-options.interface';
import { UserOrderInfo } from '../../data/interfaces/user/user-order-info.interface';
import { forkJoin, Subject, takeUntil } from 'rxjs';
import { AuthService } from '../../service/auth/auth.service';
import { RegisterUserFromOrderRequest } from '../../data/interfaces/user/register-user-from-order-request.interface';
import { AddToCartRequest } from '../../data/interfaces/cart/add-to-cart-request.interface';
import { CartItemComponent } from './cart-item/cart-item.component';
import { UserDeliveryAddress } from '../../data/interfaces/user/user-delivery-address.interface';
import { Router } from '@angular/router';
import { ToastService } from '../../service/toast/toast.service';
import { UserService } from '../../service/user/user.service';

@Component({
  selector: 'app-cart-page',
  imports: [CommonModule, FormsModule, CartItemComponent, ReactiveFormsModule],
  templateUrl: './cart-page.component.html',
  styleUrls: ['./cart-page.component.scss'],
})
export class CartPageComponent implements OnInit, OnDestroy {
  public items: CartItem[] = [];
  public totalPrice: number = 0;
  public showAddressFields: boolean = false;
  public orderOptions!: OrderOptions;
  public userOrderInfo!: UserOrderInfo;
  public orderForm: FormGroup;
  private unsubscribe$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private cartService: CartService,
    private authService: AuthService,
    private router: Router,
    private userService: UserService,
    private toastService: ToastService
  ) {
    this.orderForm = this.fb.group({
      firstName: ['', Validators.required],
      telephone: [
        '',
        [
          Validators.required,
          Validators.pattern(/^\+375\s\d{2}\s\d{3}-\d{2}-\d{2}$/),
        ],
      ],
      email: ['', [Validators.required, Validators.email]],
      city: [''],
      street: [''],
      houseNumber: [''],
      flat: [''],
      deliveryMethodId: ['', Validators.required],
      paymentMethod: ['', Validators.required],
    });
  }

  public ngOnInit(): void {
    forkJoin({
      cartItems: this.cartService.getCartItems(),
      orderOptions: this.cartService.getOrderOptions(),
      userOrderInfo: this.cartService.getUserOrderInfo(),
    })
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (val) => {
          this.items = val.cartItems;
          this.updateTotalPrice();
          this.orderOptions = val.orderOptions;
          this.userOrderInfo = val.userOrderInfo;
          this.orderForm.patchValue(this.userOrderInfo);
          this.orderForm.patchValue(this.userOrderInfo.userDeliveryAddress);
          this.orderForm
            .get('deliveryMethodId')!
            .setValue(
              this.orderOptions.deliveryMethods.find((m) => m.price == 0)!
                .deliveryMethodId
            );
          this.orderForm
            .get('paymentMethod')!
            .setValue(this.orderOptions.paymentMethods[0]);
        },
        error: (error) => {
          if (error.status == 401) {
            this.authService.logout();
            this.router.navigate(['/']);
          }
        },
      });
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public updateTotalPrice(): void {
    this.totalPrice = this.items.reduce(
      (total, item) => total + item.productPrice * item.quantity,
      0
    );
  }

  public changeItemQuantity(item: CartItem, isIncrease: boolean): void {
    if (isIncrease) item.quantity++;
    else item.quantity--;

    this.cartService
      .addToUserCart({
        productId: item.productId,
        quantity: item.quantity,
      })
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe();
    this.updateTotalPrice();
  }

  public removeCartItem(productId: string): void {
    this.items = this.items.filter(
      (itemTarget) => itemTarget.productId !== productId
    );
    this.cartService
      .removeFromCart(productId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe();
    this.updateTotalPrice();
  }

  public orderCart(): void {
    if (this.orderForm.invalid) {
      this.orderForm.markAllAsTouched();
      return;
    }
    if (this.items.length == 0) {
      this.toastService.showError('В корзине нет ни одного товара');
      return;
    }

    if (this.authService.isLoggedIn()) {
      const payload = {
        deliveryMethodId: this.orderForm.value.deliveryMethodId,
        paymentMethod: this.orderForm.value.paymentMethod,
        userDelivaryAddress: {
          city: this.orderForm.value.city,
          street: this.orderForm.value.street,
          houseNumber: this.orderForm.value.houseNumber,
          entrance: this.orderForm.value.entrance,
          flat: this.orderForm.value.flat,
        } as UserDeliveryAddress,
      };
      this.cartService
        .orderCartForRegistered(payload)
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe({
          next: (val) => {},
          error: (error) => {
            this.toastService.showError(error.error.error);
          },
        });
    } else {
      this.orderCartForUnregistered();
    }
    this.items = [];
    this.updateTotalPrice();
  }

  private orderCartForUnregistered(): void {
    const user: RegisterUserFromOrderRequest = {
      firstName: this.orderForm.value.firstName,
      surname: this.orderForm.value.surname,
      telephone: this.orderForm.value.telephone,
      email: this.orderForm.value.email,
      city: this.orderForm.value.city,
      street: this.orderForm.value.street,
      houseNumber: this.orderForm.value.houseNumber,
      entrance: this.orderForm.value.entrance,
      flat: this.orderForm.value.flat,
    };

    const cartItems: AddToCartRequest[] = this.items.map((item) => ({
      productId: item.productId,
      quantity: item.quantity,
    }));

    const payload = {
      user,
      cartItems,
      deliveryMethodId: this.orderForm.value.deliveryMethodId,
      paymentMethod: this.orderForm.value.paymentMethod,
    };

    this.cartService.orderCartForUnregistered(payload).subscribe({
      next: (value: { orderId: string }) => {
        localStorage.setItem(
          this.userService.userEMailKey,
          this.orderForm.value.email
        );
        this.router.navigate(['/user']);
      },
      error: (error) => {
        this.toastService.showError(error);
      },
    });
    this.cartService.clearLocalCart();
  }

  public isFieldInvalid(fieldName: string): boolean {
    const field = this.orderForm.get(fieldName);
    return field ? field.invalid && (field.dirty || field.touched) : false;
  }

  public onDeliveryMethodChange(): void {
    const deliveryMethodId = this.orderForm.get('deliveryMethodId')?.value;
    const selectedMethod = this.orderOptions.deliveryMethods.find(
      (method) => method.deliveryMethodId === deliveryMethodId
    );

    this.showAddressFields = selectedMethod!.name
      .toLowerCase()
      .includes('доставка');

    const addressFields = ['city', 'street', 'houseNumber', 'flat'];
    addressFields.forEach((field) => {
      const control = this.orderForm.get(field);
      if (control) {
        if (this.showAddressFields) {
          control.setValidators(Validators.required);
        } else {
          control.clearValidators();
        }
        control.updateValueAndValidity();
      }
    });
  }
}
