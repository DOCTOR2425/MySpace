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
import { ProductCardComponent } from '../../common-ui/product-card/product-card.component';
import { FilterRequest } from '../../data/interfaces/filters/filter-request.interface';
import { FormsModule } from '@angular/forms';
import { RangeFilter } from '../../data/interfaces/filters/range-filter.interface';
import { CollectionFilter } from '../../data/interfaces/filters/collection-filter.interface';
import { CategoryFilters } from '../../data/interfaces/filters/category-filters.intervace';
import { RangePropertyForFilter } from '../../data/interfaces/filters/range-property-for-filter.intervace';
import { ProductCard } from '../../data/interfaces/product/product-card.interface';

@Component({
  selector: 'app-category-page',
  standalone: true,
  imports: [ProductCardComponent, CommonModule, FormsModule],
  templateUrl: './category-page.component.html',
  styleUrls: ['./category-page.component.scss'],
})
export class CategoryPageComponent implements OnInit, OnDestroy {
  public categoryName: string = '';
  public products: ProductCard[] = [];
  public categoryFilters!: CategoryFilters;
  public showAllStates: boolean[] = [];

  private unsubscribe$ = new Subject<void>();
  private debounceTimer: any;

  @ViewChildren('collectionInput') collectionInputs!: QueryList<ElementRef>;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService
  ) {}

  public ngOnInit(): void {
    this.route.paramMap
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((params) => {
        this.categoryName = params.get('categoryName') || '';
        this.loadData();
      });
  }

  private loadData(): void {
    forkJoin({
      productsCardsByCategory:
        this.productService.getAllProductsCardsByCategory(this.categoryName),
      categoryFilters: this.productService.getCategoryFilters(
        this.categoryName
      ),
    })
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: ({ productsCardsByCategory, categoryFilters }) => {
          this.products = productsCardsByCategory;
          this.initializeFilters(categoryFilters);
        },
        error: (err) => console.error('Error loading data:', err),
      });
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
    this.debouncedFilterChange();
  }

  public onFilterChange(): void {
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
        this.categoryName,
        filterRequest
      )
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (data) => (this.products = data),
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

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }
}
