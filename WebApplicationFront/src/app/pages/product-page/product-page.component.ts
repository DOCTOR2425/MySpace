import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import {
  ActivatedRoute,
  NavigationEnd,
  Router,
  RouterLink,
} from '@angular/router';
import { ProductService } from '../../service/product.service';
import { CommonModule } from '@angular/common';
import { CartService } from '../../service/cart/cart.service';
import { filter, Subject, takeUntil } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { CreateCommentRequest } from '../../data/interfaces/comment/create-comment-request.interface';
import { CommentResponse } from '../../data/interfaces/comment/comment-response.interface';
import { ComparisonService } from '../../service/comparison/comparison.service';
import { ToastService } from '../../service/toast/toast.service';
import { FullProductInfoResponse } from '../../data/interfaces/product/product-to-update-response.interface';
import { UserProductCard } from '../../data/interfaces/product/user-product-card.interface';
import { UserProductCardComponent } from '../../common-ui/product-card/product-card.component';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { AuthService } from '../../service/auth/auth.service';
import { UserService } from '../../service/user/user.service';
import { UserProductStats } from '../../data/interfaces/product/user-product-stats.interface';

@Component({
  selector: 'app-product',
  imports: [
    CommonModule,
    FormsModule,
    UserProductCardComponent,
    ScrollingModule,
    RouterLink,
  ],
  templateUrl: './product-page.component.html',
  styleUrls: ['./product-page.component.scss'],
})
export class ProductComponent implements OnInit, OnDestroy {
  @Input() id!: string;
  public product!: FullProductInfoResponse;
  public selectedImage: string | null = null;
  public newComment: string = '';
  public comments: CommentResponse[] = [];
  public simmularProducts: UserProductCard[] = [];
  public userProductStats!: UserProductStats;

  public viewportHeight: number = 300;
  public expanded: boolean = false;

  private unsubscribe$ = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    public router: Router,
    private productService: ProductService,
    private cartService: CartService,
    private comparisonService: ComparisonService,
    private toastService: ToastService,
    public authService: AuthService,
    private userService: UserService
  ) {}

  public ngOnInit(): void {
    this.loadPage();
    this.router.events
      .pipe(takeUntil(this.unsubscribe$))
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

  public toggleViewportSize() {
    this.expanded = !this.expanded;
    this.viewportHeight = this.expanded
      ? window.visualViewport!.height * 0.78
      : 300;
    setTimeout(() => window.visualViewport!.height, 300);
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
        this.cartService
          .getCartItems()
          .pipe(takeUntil(this.unsubscribe$))
          .subscribe({
            next: (cartItems) => {
              this.simmularProducts = this.simmularProducts.map((product) => {
                let cartItem = cartItems.find(
                  (i) => i.productId == product.productId
                );
                product.cartCount = cartItem ? cartItem.quantity : 0;
                return product;
              });
            },
          });
      });

    this.getUserProductStats();
  }

  private getUserProductStats() {
    if (this.authService.isLoggedIn()) {
      this.userService
        .getUserProductStats(this.id)
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe({
          next: (stats) => {
            this.userProductStats = stats;
          },
        });
    } else {
      this.userProductStats = {
        cartCount: 0,
        isInComparison: false,
      };

      this.cartService
        .getCartItems()
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe({
          next: (cartItems) => {
            let item = cartItems.filter((item) => item.productId == this.id)[0];
            this.userProductStats.cartCount = item ? item.quantity : 0;
          },
        });

      this.comparisonService
        .getUserComparison()
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe({
          next: (products) => {
            let product = products.filter(
              (product) => product.productResponseData.productId == this.id
            )[0];
            this.userProductStats.isInComparison = product != null;
          },
        });
    }
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
    this.userProductStats.cartCount = 1;
    this.changeUserCart(this.userProductStats.cartCount);
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
          this.userProductStats.isInComparison = true;
          this.toastService.showInfo(
            'Продукт добавлне в сравнения',
            'Добавленно'
          );
        },
      });
  }

  public deleteFromComparison(): void {
    this.comparisonService
      .deleteFromComparison(this.id)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {
          this.userProductStats.isInComparison = false;
          this.toastService.showInfo('Продукт удалён из сравнений', 'Удалено');
        },
      });
  }

  public increaseQuantity(): void {
    this.userProductStats.cartCount++;
    this.changeUserCart(this.userProductStats.cartCount);
  }

  public decreaseQuantity(): void {
    if (this.userProductStats.cartCount > 1) {
      this.userProductStats.cartCount--;
    } else {
      this.userProductStats.cartCount = 0;
    }
    this.changeUserCart(this.userProductStats.cartCount);
  }

  private changeUserCart(cartCount: number): void {
    this.cartService
      .addToUserCart({
        productId: this.id,
        quantity: cartCount,
      })
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({ next: (val) => {} });
  }
}
