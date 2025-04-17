import { Component, OnDestroy, OnInit } from '@angular/core';
import { ProductCardComponent } from '../../common-ui/product-card/product-card.component';
import { CommonModule } from '@angular/common';
import { ProductService } from '../../service/product.service';
import { Subject, takeUntil } from 'rxjs';
import { ProductCard } from '../../data/interfaces/product/product-card.interface';
import { AuthService } from '../../service/auth/auth.service';

@Component({
  selector: 'app-catalog',
  imports: [ProductCardComponent, CommonModule],
  templateUrl: './catalog-page.component.html',
  styleUrl: './catalog-page.component.scss',
})
export class CatalogComponent implements OnInit, OnDestroy {
  public products: ProductCard[] = [];
  private unsubscribe$ = new Subject<void>();

  constructor(
    private productService: ProductService,
    private authService: AuthService
  ) {}

  public ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.productService
        .getSpecialProductsForUser()
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe((val) => {
          this.products = val;
        });
    } else {
      this.productService
        .getMostPopularProducts(1)
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe((val) => {
          this.products = val;
        });
    }
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
}
