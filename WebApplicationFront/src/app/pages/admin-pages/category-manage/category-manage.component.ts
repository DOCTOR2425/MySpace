import { Component, OnDestroy, OnInit } from '@angular/core';
import { ProductCategoryService } from '../../../service/product-category/product-category.service';
import { Subject, takeUntil } from 'rxjs';
import { ProductCategoryForAdmin } from '../../../data/interfaces/product-category/product-category-for-admin.interface';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-category-manage',
  imports: [CommonModule, RouterLink],
  templateUrl: './category-manage.component.html',
  styleUrl: './category-manage.component.scss',
})
export class CategoryManageComponent implements OnInit, OnDestroy {
  public categories: ProductCategoryForAdmin[] = [];
  public filteredCategories: ProductCategoryForAdmin[] = [];
  private unsubscribe$ = new Subject<void>();

  public searchQuery = '';
  public visibilityStatus: 'all' | 'active' | 'archived' = 'all';

  constructor(private productCategoryService: ProductCategoryService) {}

  ngOnInit(): void {
    this.productCategoryService
      .getProductCategoriesForAdmin()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (val) => {
          this.categories = val;
          this.applyFilters();
        },
      });
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public changeVisibilityStatus(
    categoryId: string,
    visibilityStatus: boolean
  ): void {
    this.productCategoryService
      .changeVisibilityStatus(categoryId, visibilityStatus)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {
          this.categories.find(
            (c) => c.productCategoryId == categoryId
          )!.isHidden = visibilityStatus;
        },
      });
  }

  public applyFilters(): void {
    this.filteredCategories = this.categories.filter((category) => {
      let matchesStatus = true;
      if (this.visibilityStatus === 'active') {
        matchesStatus = !category.isHidden;
      } else if (this.visibilityStatus === 'archived') {
        matchesStatus = category.isHidden;
      }
      return matchesStatus;
    });
  }

  private filterByVisibility(
    categories: ProductCategoryForAdmin[]
  ): ProductCategoryForAdmin[] {
    return categories;
  }

  private filterByName(
    categories: ProductCategoryForAdmin[]
  ): ProductCategoryForAdmin[] {
    return categories.filter((category) =>
      category.name.includes(this.searchQuery)
    );
  }
}
