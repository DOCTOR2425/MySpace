import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../../service/product.service';
import { CommonModule } from '@angular/common';
import { Product } from '../../data/interfaces/product/product.interface';
import { CartService } from '../../service/cart/cart.service';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-product',
  imports: [CommonModule],
  templateUrl: './product-page.component.html',
  styleUrl: './product-page.component.scss',
})
export class ProductComponent implements OnInit, OnDestroy {
  public productId!: string;
  public product!: Product;
  public propertyNames: string[] = [];
  private unsubscribe$ = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private cartService: CartService
  ) {}

  public ngOnInit(): void {
    this.productId = this.route.snapshot.paramMap.get('id')!;
    this.productService
      .getProductById(this.productId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((data) => {
        this.product = data;
        this.propertyNames = Object.keys(data.properties);
      });
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public addToCart(): void {
    const addToCartRequest = {
      productId: this.productId,
      quantity: 1,
    };
    this.cartService
      .cahngeCart(addToCartRequest)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe();

    const button = document.getElementById('addToCart') as HTMLButtonElement;
    button.style.backgroundColor = 'gray';
  }
}
