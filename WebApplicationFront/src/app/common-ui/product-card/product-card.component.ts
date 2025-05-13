import { Component, Input, OnDestroy } from '@angular/core';
import { ProductCard } from '../../data/interfaces/product/product-card.interface';
import { CartService } from '../../service/cart/cart.service';
import { RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-product-card',
  imports: [RouterModule],
  templateUrl: './product-card.component.html',
  styleUrl: './product-card.component.scss',
})
export class ProductCardComponent implements OnDestroy {
  @Input() product!: ProductCard;
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
      .subscribe();

    const button = document.getElementsByClassName(
      'add-to-cart-btn ' + this.product.productId
    )[0] as HTMLButtonElement;
    button.style.backgroundColor = 'gray';
  }
}
