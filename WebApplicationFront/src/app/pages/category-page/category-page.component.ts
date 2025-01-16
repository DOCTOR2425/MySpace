import { Component, OnDestroy, OnInit } from '@angular/core';
import { ProductService } from '../../service/product.service';
import { ActivatedRoute } from '@angular/router';
import { debounceTime, pipe, Subject, takeUntil } from 'rxjs';
import { ProductData } from '../../data/interfaces/product/product-data.interface';
import { CommonModule } from '@angular/common';
import { ProductCardComponent } from '../../common-ui/product-card/product-card.component';
import { FilterRequest } from '../../data/interfaces/filters/filter-request.interface';

@Component({
  selector: 'app-category-page',
  imports: [ProductCardComponent, CommonModule],
  templateUrl: './category-page.component.html',
  styleUrl: './category-page.component.scss',
})
export class CategoryPageComponent implements OnInit, OnDestroy {
  public categoryName!: string;
  private unsubscribe$ = new Subject<void>();
  products: ProductData[] = [];
  private inputSubject = new Subject<string>();

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService
  ) {}

  public ngOnInit(): void {
    this.route.paramMap
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((params) => {
        this.categoryName = params.get('categoryName')!;
        // TODO forkJoin в cart-page
        this.productService
          .getAllProductsCardsByCategory(this.categoryName)
          .pipe(takeUntil(this.unsubscribe$))
          .subscribe((data) => {
            this.products = data;
          });
      });

    this.inputSubject
      .pipe(
        debounceTime(1000), // Задержка в 1 секунду
        takeUntil(this.unsubscribe$)
      )
      .subscribe((value) => {
        this.onDebouncedInput(value);
      });
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  onInputChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.inputSubject.next(input.value);
  }

  onDebouncedInput(value: string): void {
    console.log('input');

    const filterRequest: FilterRequest = {
      rangeFilters: [{ valueFrom: 100, valueTo: 300, property: 'price' }],
      collectionFilters: [{ propertyId: 'brand', propertyValue: 'bosch' }],
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
