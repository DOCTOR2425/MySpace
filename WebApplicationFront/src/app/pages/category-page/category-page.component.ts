import {
  Component,
  OnDestroy,
  OnInit,
  ViewChildren,
  QueryList,
} from '@angular/core';
import { ProductService } from '../../service/product.service';
import { ActivatedRoute } from '@angular/router';
import { forkJoin, Subject, takeUntil } from 'rxjs';
import { ProductData } from '../../data/interfaces/product/product-data.interface';
import { CommonModule } from '@angular/common';
import { ProductCardComponent } from '../../common-ui/product-card/product-card.component';
import { FilterRequest } from '../../data/interfaces/filters/filter-request.interface';
import { FormsModule } from '@angular/forms';
import { RangeFilter } from '../../data/interfaces/filters/range-filter.interface';
import { CollectionFilter } from '../../data/interfaces/filters/collection-filter.interface';
import { CategoryFilters } from '../../data/interfaces/filters/category-filters.intervace';
import { RangePropertyForFilter } from '../../data/interfaces/filters/range-property-for-filter.intervace';

@Component({
  selector: 'app-category-page',
  imports: [ProductCardComponent, CommonModule, FormsModule],
  templateUrl: './category-page.component.html',
  styleUrl: './category-page.component.scss',
})
export class CategoryPageComponent implements OnInit, OnDestroy {
  public categoryName!: string;
  public products: ProductData[] = [];
  public categoryFilters!: CategoryFilters;

  private unsubscribe$ = new Subject<void>();

  @ViewChildren('rangeInputMin') rangeInputsMin!: QueryList<any>;
  @ViewChildren('rangeInputMax') rangeInputsMax!: QueryList<any>;
  @ViewChildren('collectionInput') collectionInputs!: QueryList<any>;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService
  ) {}

  public ngOnInit(): void {
    this.route.paramMap
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((params) => {
        this.categoryName = params.get('categoryName')!;
        forkJoin({
          productsCardsByCategory:
            this.productService.getAllProductsCardsByCategory(
              this.categoryName
            ),
          categoryFilters: this.productService.getCategoryFilters(
            this.categoryName
          ),
        })
          .pipe(takeUntil(this.unsubscribe$))
          .subscribe((val) => {
            this.products = val.productsCardsByCategory;
            this.categoryFilters = val.categoryFilters;

            // Установка начальных значений для range фильтров
            this.categoryFilters.rangePropertyForFilters.forEach((filter) => {
              filter.currentMinValue = filter.minValue;
              filter.currentMaxValue = filter.maxValue;
            });
          });
      });
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  private getRangeFilters(): RangeFilter[] {
    let rangesForFilter: RangePropertyForFilter[] =
      this.categoryFilters.rangePropertyForFilters.filter(
        (filter) =>
          filter.minValue != filter.currentMinValue ||
          filter.maxValue != filter.currentMaxValue
      );

    return rangesForFilter.map((filter) => ({
      minValue: filter.currentMinValue ?? 0,
      maxValue: filter.currentMaxValue ?? 0,
      property: filter.propertyName,
    }));
  }

  private getCollectionFilters(): CollectionFilter[] {
    const collectionFilters = this.collectionInputs
      .filter((input) => input.nativeElement.checked)
      .map((input) => ({
        property: input.nativeElement.getAttribute('data-property'),
        propertyValue: input.nativeElement.value,
      }));

    if (collectionFilters.length == 0) return [];

    return collectionFilters;
  }

  onFilterChange(): void {
    const rangeFilters = this.getRangeFilters();
    const collectionFilters = this.getCollectionFilters();

    const filterRequest: FilterRequest = {
      rangeFilters,
      collectionFilters,
    };

    this.productService
      .getAllProductsCardsByCategoryWithFilters(
        this.categoryName,
        filterRequest
      )
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((data) => {
        this.products = data;
      });
  }
}
