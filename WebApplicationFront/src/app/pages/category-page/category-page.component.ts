import {
  Component,
  OnDestroy,
  OnInit,
  ViewChildren,
  QueryList,
  ElementRef,
} from '@angular/core';
import { ProductService } from '../../service/product.service';
import { ActivatedRoute } from '@angular/router';
import { forkJoin, Subject, takeUntil } from 'rxjs';
import { CommonModule } from '@angular/common';
import { UserProductCardComponent } from '../../common-ui/product-card/product-card.component';
import { FilterRequest } from '../../data/interfaces/filters/filter-request.interface';
import { FormsModule } from '@angular/forms';
import { RangeFilter } from '../../data/interfaces/filters/range-filter.interface';
import { CollectionFilter } from '../../data/interfaces/filters/collection-filter.interface';
import { CategoryFilters } from '../../data/interfaces/filters/category-filters.intervace';
import { RangePropertyForFilter } from '../../data/interfaces/filters/range-property-for-filter.intervace';
import { UserProductCard } from '../../data/interfaces/product/user-product-card.interface';
import { CartService } from '../../service/cart/cart.service';
import { AuthService } from '../../service/auth/auth.service';

@Component({
  selector: 'app-category-page',
  standalone: true,
  imports: [UserProductCardComponent, CommonModule, FormsModule],
  templateUrl: './category-page.component.html',
  styleUrls: ['./category-page.component.scss'],
})
export class CategoryPageComponent implements OnInit, OnDestroy {
  public categoryId: string = '';
  public products: UserProductCard[] = [];
  public categoryFilters!: CategoryFilters;
  public showAllStates: boolean[] = [];

  // Пагинация
  public currentPage: number = 1;
  public itemsPerPage: number = 20; // Можно изменить на нужное количество
  public totalItems: number = 0;
  public totalPages: number = 0;
  public pages: number[] = [];

  private unsubscribe$ = new Subject<void>();
  private debounceTimer: any;

  @ViewChildren('collectionInput') collectionInputs!: QueryList<ElementRef>;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private authService: AuthService,
    private cartService: CartService
  ) {}

  public ngOnInit(): void {
    this.route.paramMap
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((params) => {
        this.categoryId = params.get('id') || '';
        this.currentPage = 1;
        this.loadData();
      });
  }

  private loadData(): void {
    forkJoin({
      productsCardsByCategory:
        this.productService.getAllProductsCardsByCategory(
          this.categoryId,
          this.currentPage
        ),
      categoryFilters: this.productService.getCategoryFilters(this.categoryId),
    })
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: ({ productsCardsByCategory, categoryFilters }) => {
          this.products = productsCardsByCategory.items;
          if (!this.authService.isLoggedIn())
            this.getProductsStatsForUnregister();

          this.totalItems = productsCardsByCategory.totalCount;
          this.totalPages = Math.ceil(this.totalItems / this.itemsPerPage);
          this.updatePageNumbers();
          this.initializeFilters(categoryFilters);
        },
        error: (err) => console.error('Error loading data:', err),
      });
  }

  private getProductsStatsForUnregister() {
    this.cartService
      .getCartItems()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (cartItems) => {
          this.products = this.products.map((product) => {
            let cartItem = cartItems.find(
              (i) => i.productId == product.productId
            );
            product.cartCount = cartItem ? cartItem.quantity : 0;
            return product;
          });
        },
      });
  }

  private updatePageNumbers(): void {
    const maxPagesToShow = 5; // Максимальное количество отображаемых страниц
    let startPage: number, endPage: number;

    if (this.totalPages <= maxPagesToShow) {
      startPage = 1;
      endPage = this.totalPages;
    } else {
      const halfMaxPages = Math.floor(maxPagesToShow / 2);
      if (this.currentPage <= halfMaxPages + 1) {
        startPage = 1;
        endPage = maxPagesToShow;
      } else if (this.currentPage >= this.totalPages - halfMaxPages) {
        startPage = this.totalPages - maxPagesToShow + 1;
        endPage = this.totalPages;
      } else {
        startPage = this.currentPage - halfMaxPages;
        endPage = this.currentPage + halfMaxPages;
      }
    }

    this.pages = Array.from(
      { length: endPage - startPage + 1 },
      (_, i) => startPage + i
    );
  }

  private initializeFilters(filters: CategoryFilters): void {
    this.categoryFilters = filters;
    this.showAllStates = new Array(
      this.categoryFilters.collectionPropertyForFilters.length
    ).fill(false);
    this.categoryFilters.rangePropertyForFilters.forEach((filter) => {
      filter.currentMinValue = filter.minValue;
      filter.currentMaxValue = filter.maxValue;
    });
    this.sortFilters();
  }

  private sortFilters(): void {
    this.categoryFilters.rangePropertyForFilters.sort((a, b) =>
      a.propertyName === 'Цена' ? -1 : b.propertyName === 'Цена' ? 1 : 0
    );
    this.categoryFilters.collectionPropertyForFilters.sort((a, b) =>
      a.propertyName === 'Бренд' ? -1 : b.propertyName === 'Бренд' ? 1 : 0
    );
  }

  public onRangeChange(
    filter: RangePropertyForFilter,
    isMin: boolean,
    event: Event
  ): void {
    const value = parseFloat((event.target as HTMLInputElement).value);
    if (isMin) {
      filter.currentMinValue = Math.min(value, filter.currentMaxValue!);
    } else {
      filter.currentMaxValue = Math.max(value, filter.currentMinValue!);
    }
    this.currentPage = 1; // Сброс на первую страницу при изменении фильтров
    this.debouncedFilterChange();
  }

  public onFilterChange(): void {
    this.currentPage = 1; // Сброс на первую страницу при изменении фильтров
    this.debouncedFilterChange();
  }

  private debouncedFilterChange(): void {
    clearTimeout(this.debounceTimer);
    this.debounceTimer = setTimeout(() => this.applyFilters(), 300);
  }

  private applyFilters(): void {
    const rangeFilters = this.getActiveRangeFilters();
    const collectionFilters = this.getActiveCollectionFilters();
    const filterRequest: FilterRequest = { rangeFilters, collectionFilters };

    this.productService
      .getAllProductsCardsByCategoryWithFilters(
        this.categoryId,
        filterRequest,
        this.currentPage
      )
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (data) => {
          this.products = data.items;
          this.totalItems = data.totalCount;
          this.totalPages = Math.ceil(this.totalItems / this.itemsPerPage);
          this.updatePageNumbers();
        },
        error: (err) => console.error('Error applying filters:', err),
      });
  }

  private getActiveRangeFilters(): RangeFilter[] {
    return this.categoryFilters.rangePropertyForFilters
      .filter(
        (filter) =>
          filter.minValue !== filter.currentMinValue ||
          filter.maxValue !== filter.currentMaxValue
      )
      .map((filter) => ({
        minValue: filter.currentMinValue ?? filter.minValue,
        maxValue: filter.currentMaxValue ?? filter.maxValue,
        property: filter.propertyName,
      }));
  }

  private getActiveCollectionFilters(): CollectionFilter[] {
    const checkedInputs = this.collectionInputs.filter(
      (input) => input.nativeElement.checked
    );
    return checkedInputs.length === 0
      ? []
      : checkedInputs.map((input) => ({
          property: input.nativeElement.getAttribute('data-property'),
          propertyValue: input.nativeElement.value,
        }));
  }

  public getRangeLeft(filter: RangePropertyForFilter): string {
    const min = filter.minValue;
    const max = filter.maxValue;
    const currentMin = filter.currentMinValue ?? min;
    return `${((currentMin - min) / (max - min)) * 100}%`;
  }

  public getRangeRight(filter: RangePropertyForFilter): string {
    const min = filter.minValue;
    const max = filter.maxValue;
    const currentMax = filter.currentMaxValue ?? max;
    return `${100 - ((currentMax - min) / (max - min)) * 100}%`;
  }

  public toggleShowAll(index: number): void {
    this.showAllStates[index] = !this.showAllStates[index];
  }

  public getVisibleValues(filter: any, count: number): any[] {
    return filter.uniqueValues.slice(0, count);
  }

  public getHiddenValues(filter: any, count: number): any[] {
    return filter.uniqueValues.slice(count);
  }

  public trackByFn(index: number, item: any): any {
    return item.propertyName || index;
  }

  // Методы для пагинации
  public goToPage(page: number): void {
    if (page < 1 || page > this.totalPages || page === this.currentPage) {
      return;
    }
    this.currentPage = page;
    this.applyFilters();
  }

  public prevPage(): void {
    if (this.currentPage > 1) {
      this.goToPage(this.currentPage - 1);
    }
  }

  public nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.goToPage(this.currentPage + 1);
    }
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
}
