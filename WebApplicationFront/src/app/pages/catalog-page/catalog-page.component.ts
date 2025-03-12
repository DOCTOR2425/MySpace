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
  private unsubscribe$ = new Subject<void>();
  products: ProductCard[] = [];

  constructor(private productService: ProductService) {}

  public ngOnInit(): void {
    this.productService
      .getProductCards()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((val) => {
        this.products = val;
      });
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
}
