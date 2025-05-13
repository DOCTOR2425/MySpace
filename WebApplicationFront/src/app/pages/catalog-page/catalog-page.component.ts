import { Component, OnDestroy, OnInit } from '@angular/core';
import { UserProductCardComponent } from '../../common-ui/product-card/product-card.component';
import { CommonModule } from '@angular/common';
import { ProductService } from '../../service/product.service';
import { Subject, takeUntil } from 'rxjs';
import { UserProductCard } from '../../data/interfaces/product/user-product-card.interface';
import { AuthService } from '../../service/auth/auth.service';
import { CartService } from '../../service/cart/cart.service';

@Component({
  selector: 'app-catalog',
  imports: [UserProductCardComponent, CommonModule],
  templateUrl: './catalog-page.component.html',
  styleUrl: './catalog-page.component.scss',
})
export class CatalogComponent implements OnInit, OnDestroy {
  public products: UserProductCard[] = [];
  private unsubscribe$ = new Subject<void>();

  constructor(
    private productService: ProductService,
    private cartService: CartService,
    private authService: AuthService
  ) {}

  public ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.productService
        .getSpecialProductsForUser()
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe({
          next: (val) => {
            this.products = val;
          },
        });
    } else {
      this.getProductsForUnregister();
    }
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  private getProductsForUnregister() {
    this.productService
      .getMostPopularProducts(1)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (val) => {
          this.products = val;
          this.cartService
            .getCartItems()
            .pipe(takeUntil(this.unsubscribe$))
            .subscribe({
              next: (cartItems) => {
                this.products = this.products.map((product) => {
                  let cartItem = cartItems.find(
                    (i) => i.productId == product.productId
                  );
                  product.cartCount = cartItem ? cartItem.quantity : 0;
                  return product;
                });
              },
            });
        },
      });
  }
}
