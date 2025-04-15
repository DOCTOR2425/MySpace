import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { ProductService } from '../../service/product.service';
import { CommonModule } from '@angular/common';
import { CartService } from '../../service/cart/cart.service';
import { filter, Subject, takeUntil } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { CreateCommentRequest } from '../../data/interfaces/Comment/create-comment-request.interface';
import { CommentResponse } from '../../data/interfaces/Comment/comment-response.interface';
import { ComparisonService } from '../../service/comparison/comparison.service';
import { ToastService } from '../../service/toast/toast.service';
import { FullProductInfoResponse } from '../../data/interfaces/product/product-to-update-response.interface';
import { ProductCard } from '../../data/interfaces/product/product-card.interface';
import { ProductCardComponent } from '../../common-ui/product-card/product-card.component';
import { ScrollingModule } from '@angular/cdk/scrolling';

@Component({
  selector: 'app-product',
  imports: [CommonModule, FormsModule, ProductCardComponent, ScrollingModule],
  templateUrl: './product-page.component.html',
  styleUrls: ['./product-page.component.scss'],
})
export class ProductComponent implements OnInit, OnDestroy {
  @Input() id!: string;
  public product!: FullProductInfoResponse;
  public selectedImage: string | null = null;
  public newComment: string = '';
  public comments: CommentResponse[] = [];
  public simmularProducts: ProductCard[] = [];
  private unsubscribe$ = new Subject<void>();

  public viewportHeight = 500;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private productService: ProductService,
    private cartService: CartService,
    private comparisonService: ComparisonService,
    private toastService: ToastService
  ) {}

  public ngOnInit(): void {
    this.loadPage();
    this.router.events
      .pipe(filter((e): e is NavigationEnd => e instanceof NavigationEnd))
      .subscribe(() => {
        this.id = this.route.snapshot.paramMap.get('id')!;
        this.loadPage();
      });
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  private loadPage() {
    this.selectedImage = null;
    this.newComment = '';

    this.productService
      .getProductById(this.id)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((data) => {
        this.product = data;
      });
    this.loadComments();
    this.productService
      .getSimmularToProduct(this.id)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((data) => {
        this.simmularProducts = data;
      });
  }
  private loadComments() {
    this.productService
      .getCommentsByProduct(this.id)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (comments) => {
          if (!comments) this.comments = [];
          else {
            this.comments = comments;
          }
        },
      });
  }

  public selectImage(image: string): void {
    this.selectedImage = image;
  }

  public addToCart(): void {
    const addToCartRequest = {
      productId: this.id,
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
      text: this.newComment.trim(),
      productId: this.id,
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
      .addToComparison(this.id)
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
