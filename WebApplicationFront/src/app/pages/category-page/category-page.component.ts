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
  imports: [ProductCardComponent, CommonModule, FormsModule],
  templateUrl: './category-page.component.html',
  styleUrl: './category-page.component.scss',
})
export class CategoryPageComponent implements OnInit, OnDestroy {
  public categoryName!: string;
  public products: ProductCard[] = [];
  public categoryFilters!: CategoryFilters;
  public showAllValues: boolean = false; // Declare showAllValues variable

  private unsubscribe$ = new Subject<void>();

  @ViewChildren('rangeInput') rangeInputs!: QueryList<any>;
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
            this.categoryFilters.rangePropertyForFilters.forEach((filter) => {
              filter.currentMinValue = filter.minValue;
              filter.currentMaxValue = filter.maxValue;
            });

            // Ensure 'Цена' and 'Бренд' are the first filters
            this.categoryFilters.rangePropertyForFilters.sort((a, b) => {
              if (a.propertyName === 'Цена') return -1;
              if (b.propertyName === 'Цена') return 1;
              return 0;
            });
            this.categoryFilters.collectionPropertyForFilters.sort((a, b) => {
              if (a.propertyName === 'Бренд') return -1;
              if (b.propertyName === 'Бренд') return 1;
              return 0;
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
