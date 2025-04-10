import { Component, OnDestroy, OnInit } from '@angular/core';
import { ProductCategoryService } from '../../../service/product-category/product-category.service';
import { Subject, takeUntil } from 'rxjs';
import { ProductCategoryForAdmin } from '../../../data/interfaces/product-category/product-category-for-admin.interface';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-category-manage',
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './category-manage.component.html',
  styleUrl: './category-manage.component.scss',
})
export class CategoryManageComponent implements OnInit, OnDestroy {
  public categories: ProductCategoryForAdmin[] = [];
  public filteredCategories: ProductCategoryForAdmin[] = [];
  private unsubscribe$ = new Subject<void>();

  public searchMode = false;
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
    if (this.searchMode) {
      this.filterByName();
      this.filteredCategories = this.filteredCategories.filter((category) => {
        let matchesStatus = true;
        if (this.visibilityStatus === 'active') {
          matchesStatus = !category.isHidden;
        } else if (this.visibilityStatus === 'archived') {
          matchesStatus = category.isHidden;
        }
        return matchesStatus;
      });
    } else {
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
  }

  public filterByVisibility(
    categories: ProductCategoryForAdmin[]
  ): ProductCategoryForAdmin[] {
    return categories;
  }

  public filterByName(
    categories: ProductCategoryForAdmin[] = this.categories
  ): void {
    if (this.searchQuery == '') this.searchMode = false;
    else this.searchMode = true;

    this.filteredCategories = categories.filter((category) =>
      category.name
        .toLocaleLowerCase()
        .includes(this.searchQuery.toLocaleLowerCase())
    );
  }
}
