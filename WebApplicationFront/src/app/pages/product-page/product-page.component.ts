import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../../service/product.service';
import { CommonModule } from '@angular/common';
import { Product } from '../../data/interfaces/product/product.interface';
import { CartService } from '../../service/cart/cart.service';
import { Subject, takeUntil } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { CreateCommentRequest } from '../../data/interfaces/Comment/create-comment-request.interface';
import { CommentResponse } from '../../data/interfaces/Comment/comment-response.interface';
import { ComparisonService } from '../../service/comparison/comparison.service';
import { ToastService } from '../../service/toast/toast.service';

@Component({
  selector: 'app-product',
  imports: [CommonModule, FormsModule],
  templateUrl: './product-page.component.html',
  styleUrls: ['./product-page.component.scss'],
})
export class ProductComponent implements OnInit, OnDestroy {
  public productId!: string;
  public product!: Product;
  public propertyNames: string[] = [];
  public selectedImage: string | null = null;
  public newComment: string = '';
  public comments: CommentResponse[] = [];
  private unsubscribe$ = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private cartService: CartService,
    private comparisonService: ComparisonService,
    private toastService: ToastService
  ) {}

  public ngOnInit(): void {
    this.productId = this.route.snapshot.paramMap.get('id')!;
    this.loadProduct();
    this.loadComments();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  private loadProduct(): void {
    this.productService
      .getProductById(this.productId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((data) => {
        this.product = data;
        this.propertyNames = Object.keys(data.properties);
      });
  }

  private loadComments(): void {
    this.productService
      .getCommentsByProduct(this.productId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (comments) => {
          this.comments = comments;
        },
      });
  }

  public selectImage(image: string): void {
    this.selectedImage = image;
  }

  public addToCart(): void {
    const addToCartRequest = {
      productId: this.productId,
      quantity: 1,
    };
    this.cartService
      .addToUserCart(addToCartRequest)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe(() => {
        const button = document.getElementById(
          'addToCart'
        ) as HTMLButtonElement;
        button.disabled = true;
        button.textContent = 'Добавлено в корзину';
      });
  }

  public addComment(): void {
    if (!this.newComment.trim()) return;

    const request: CreateCommentRequest = {
      text: this.newComment,
      productId: this.productId,
    };

    this.productService
      .addComment(request)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {
          this.loadComments();
          this.newComment = '';
        },
      });
  }

  public addToComparison(): void {
    this.comparisonService
      .addToComparison(this.productId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (val) => {
          this.toastService.showInfo(
            'Продукт добавлне в сравнения',
            'Добавленно'
          );
        },
      });
  }
}
