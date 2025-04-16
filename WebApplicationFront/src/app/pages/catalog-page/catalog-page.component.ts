import { Component, OnDestroy, OnInit } from '@angular/core';
import { ProductCardComponent } from '../../common-ui/product-card/product-card.component';
import { CommonModule } from '@angular/common';
import { ProductService } from '../../service/product.service';
import { Subject, takeUntil } from 'rxjs';
import { ProductCard } from '../../data/interfaces/product/product-card.interface';

@Component({
  selector: 'app-catalog',
  imports: [ProductCardComponent, CommonModule],
  templateUrl: './catalog-page.component.html',
  styleUrl: './catalog-page.component.scss',
})
export class CatalogComponent implements OnInit, OnDestroy {
  public products: ProductCard[] = [];
  private unsubscribe$ = new Subject<void>();

  constructor(private productService: ProductService) {}

  public ngOnInit(): void {
    this.productService
      .getSpecialProductsForUser()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((val) => {
        console.log(val);

        this.products = val;
      });
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
}
