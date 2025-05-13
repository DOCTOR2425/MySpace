import { Component, Input, OnDestroy } from '@angular/core';
import { UserProductCard } from '../../data/interfaces/product/user-product-card.interface';
import { CartService } from '../../service/cart/cart.service';
import { RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-product-card',
  imports: [RouterModule, CommonModule],
  templateUrl: './product-card.component.html',
  styleUrl: './product-card.component.scss',
})
export class UserProductCardComponent implements OnDestroy {
  @Input() product!: UserProductCard;
  private unsubscribe$ = new Subject<void>();

  constructor(private cartService: CartService) {}

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public addToCart(): void {
    const addToCartRequest = {
      productId: this.product.productId,
      quantity: 1,
    };
    this.cartService
      .addToUserCart(addToCartRequest)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (val) => {
          this.product.cartCount = 1;
          const button = document.getElementsByClassName(
            'add-to-cart-btn ' + this.product.productId
          )[0] as HTMLButtonElement;
          button.className = 'already-in-cart-btn';
        },
      });
  }
}
