import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { AuthService } from '../../service/auth/auth.service';
import { AdminService } from '../../service/admin/admin.service';
import { CommonModule } from '@angular/common';
import { UserService } from '../../service/user/user.service';
import { ProductService } from '../../service/product.service';
import { filter, Subject, takeUntil } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { ProductCategoryService } from '../../service/product-category/product-category.service';
import { ProductCategory } from '../../data/interfaces/some/product-category.interface';

@Component({
  selector: 'app-header',
  imports: [RouterModule, CommonModule, FormsModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent implements OnInit, OnDestroy, AfterViewInit {
  public searchQuery: string = '';
  public categories: ProductCategory[] = [];
  private unsubscribe$ = new Subject<void>();

  constructor(
    public router: Router,
    public authService: AuthService,
    public userService: UserService,
    public adminService: AdminService,
    private productService: ProductService,
    private productCategoryService: ProductCategoryService
  ) {}

  public ngOnInit(): void {
    this.router.events
      .pipe(
        filter((event) => event instanceof NavigationEnd),
        takeUntil(this.unsubscribe$)
      )
      .subscribe(() => {
        this.searchQuery = '';
      });

    this.productCategoryService
      .getTopCategoriesBySales()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (val) => {
          this.categories = val;
        },
      });
  }

  public getEmail(): string | undefined {
    return localStorage
      .getItem(this.userService.userEMailKey)
      ?.slice(0, 3)
      .toUpperCase();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public ngAfterViewInit(): void {
    const header = document.getElementsByClassName('header')[0] as HTMLElement;
    const updateHeight = () => {
      const height = header.offsetHeight;
      document.documentElement.style.setProperty(
        '--header-height',
        `${height}px`
      );
    };

    updateHeight();
    new ResizeObserver(updateHeight).observe(header);
  }

  public searchByName(): void {
    if (this.searchQuery) {
      this.productService
        .searchByName(this.searchQuery, 1)
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe(() => {
          this.router.navigate([`/search/${this.searchQuery}`]);
        });
    }
  }
}
