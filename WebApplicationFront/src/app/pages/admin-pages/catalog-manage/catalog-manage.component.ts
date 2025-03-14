import { CommonModule } from '@angular/common';
import { Component, OnDestroy } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ProductData } from '../../../data/interfaces/product/product-data.interface';
import { ProductService } from '../../../service/product.service';
import { Subject, takeUntil } from 'rxjs';

@Component({
  standalone: true,
  selector: 'app-catalog-manage',
  imports: [CommonModule, RouterModule],
  templateUrl: './catalog-manage.component.html',
  styleUrl: './catalog-manage.component.scss',
})
export class CatalogManageComponent implements OnDestroy {
  public products: ProductData[] = [];
  public page = 1;
  public loading = false;
  public hasMoreData = true;
  private unsubscribe$ = new Subject<void>();

  constructor(private productService: ProductService) {}

  public ngOnInit(): void {
    this.loadProducts();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public loadProducts(): void {
    if (this.loading || this.hasMoreData == false) {
      console.log('this.loading || this.hasMoreData');
      console.log(this.loading, this.hasMoreData);

      return;
    }
    this.loading = true;

    this.productService
      .getProductsForAdmin(this.page)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (data) => {
          if (data.length === 0) {
            this.hasMoreData = false;
            console.log('this.hasMoreData = false');
          } else {
            this.products = this.products.concat(data);
            this.page++;
          }
          this.loading = false;
        },
        error: () => {
          console.error('Error loading products');
          this.loading = false;
        },
      });
  }

  public deleteProduct(productId: string): void {
    this.productService
      .deleteProduct(productId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {},
        error: () => {},
      });
  }
}
