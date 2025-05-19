import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { UserService } from '../../../service/user/user.service';
import { UserProductCard } from '../../../data/interfaces/product/user-product-card.interface';
import { ProductService } from '../../../service/product.service';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ToastService } from '../../../service/toast/toast.service';

@Component({
  selector: 'app-products-pending-reviews',
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './products-pending-reviews.component.html',
  styleUrl: './products-pending-reviews.component.scss',
})
export class ProductsPendingReviewsComponent implements OnInit, OnDestroy {
  public products!: UserProductCard[];
  public isReviewPopupOpen = false;
  public selectedProduct: UserProductCard | null = null;
  public reviewText = '';
  private unsubscribe$ = new Subject<void>();

  constructor(
    private userService: UserService,
    private productService: ProductService,
    private toastService: ToastService
  ) {}

  public ngOnInit(): void {
    this.userService
      .getOrderedProductsPendingReviews()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (val) => {
          this.products = val;
        },
      });
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public openReviewPopup(product: UserProductCard): void {
    this.selectedProduct = product;
    this.isReviewPopupOpen = true;

    document.body.style.overflow = 'hidden';
    document.body.style.position = 'fixed';
    document.body.style.width = '100%';
  }

  public closeReviewPopup(): void {
    this.isReviewPopupOpen = false;
    this.reviewText = '';
    this.selectedProduct = null;

    document.body.style.overflow = '';
    document.body.style.position = '';
    document.body.style.width = '';
  }

  public submitReview() {
    const payload = {
      text: this.reviewText,
      productId: this.selectedProduct!.productId,
    };
    this.productService
      .addComment(payload)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {
          this.products = this.products.filter(
            (p) => p.productId != this.selectedProduct!.productId
          );
          this.closeReviewPopup();
        },
        error: (error) => {
          if (error.error.statusCode == 403) {
            this.toastService.showError('Вам запрещено оставлять комментарии');
            this.closeReviewPopup();
          }
        },
      });
  }
}
