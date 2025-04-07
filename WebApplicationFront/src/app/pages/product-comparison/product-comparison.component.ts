import { Component, OnInit } from '@angular/core';
import { Product } from '../../data/interfaces/product/product.interface';
import { ComparisonService } from '../../service/comparison/comparison.service';
import { Subject, takeUntil } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-product-comparison',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './product-comparison.component.html',
  styleUrl: './product-comparison.component.scss',
})
export class ProductComparisonComponent implements OnInit {
  public products: Product[] = [];
  public properties: string[] = [];

  private unsubscribe$ = new Subject<void>();

  constructor(private comparisonService: ComparisonService) {}

  public ngOnInit(): void {
    this.comparisonService
      .getUserComparison()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((products) => {
        this.products = products;
        this.updatePropertiesLists();
      });
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  private updatePropertiesLists() {
    this.properties = [];

    const propertiesSet = new Set<string>();
    this.products.forEach((product) => {
      Object.keys(product.properties).forEach((prop) => {
        propertiesSet.add(prop);
      });
    });

    this.properties = Array.from(propertiesSet);
  }

  public deleteFromComparison(productId: string) {
    this.comparisonService
      .deleteFromComparison(productId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {
          this.products = this.products.filter(
            (product) => product.productResponseData.productId !== productId
          );
          this.updatePropertiesLists();
        },
      });
  }

  public getPropertyValue(product: Product, property: string): string {
    return product.properties[property] || '-';
  }

  public clearComparisonList() {
    this.comparisonService
      .clearComparisonList()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {
          this.products = [];
          this.updatePropertiesLists();
        },
      });
  }
}
