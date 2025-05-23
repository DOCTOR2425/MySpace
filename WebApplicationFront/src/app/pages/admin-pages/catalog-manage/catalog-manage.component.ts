import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../../service/product.service';
import { Subject, takeUntil } from 'rxjs';
import { BrandManageComponent } from './brand-manage/brand-manage.component';
import { CountyManageComponent } from './country-manage/country-manage.component';
import { AdminProductCard } from '../../../data/interfaces/product/admin-product-card.interface';

@Component({
  standalone: true,
  selector: 'app-catalog-manage',
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    BrandManageComponent,
    CountyManageComponent,
  ],
  templateUrl: './catalog-manage.component.html',
  styleUrl: './catalog-manage.component.scss',
})
export class CatalogManageComponent implements OnInit, OnDestroy {
  public products: AdminProductCard[] = [];
  public filteredProducts: AdminProductCard[] = [];
  public page = 1;
  public loading = false;
  public hasMoreData = true;
  private unsubscribe$ = new Subject<void>();

  public searchQuery = '';
  public archiveStatus: 'all' | 'active' | 'archived' = 'all';

  constructor(private productService: ProductService, private router: Router) {}

  public ngOnInit(): void {
    this.loadProducts(this.page);
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public applyFilters(): void {
    this.filteredProducts = this.products.filter((product) => {
      let matchesStatus = true;
      if (this.archiveStatus === 'active') {
        matchesStatus = !product.isArchive;
      } else if (this.archiveStatus === 'archived') {
        matchesStatus = product.isArchive;
      }
      return matchesStatus;
    });
  }

  public uploadProducts(): void {
    this.page++;
    if (this.searchQuery != '') {
      this.searchByNameUpload(this.page);
    } else {
      this.loadProducts(this.page);
    }
  }

  public searchProducts(): void {
    this.hasMoreData = true;
    this.page = 1;
    this.products = [];
    this.searchByName(this.page);
  }

  private loadProducts(page: number): void {
    if (this.loading || this.hasMoreData == false) {
      return;
    }
    this.loading = true;

    this.productService
      .getProductsForAdmin(page)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (val) => {
          if (val.length === 0) {
            this.hasMoreData = false;
          } else {
            this.products = this.products.concat(val);
            this.applyFilters();
          }
          this.loading = false;
        },
      });
  }

  private searchByName(page: number): void {
    this.productService
      .searchByNameWithArchive(this.searchQuery, page)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (val) => {
          this.products = val;
          this.applyFilters();
          this.loading = false;
        },
      });
  }

  private searchByNameUpload(page: number): void {
    this.productService
      .searchByNameWithArchive(this.searchQuery, page)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (val) => {
          if (val.length === 0) {
            this.hasMoreData = false;
          } else {
            this.products = this.products.concat(val);
            this.applyFilters();
          }
          this.loading = false;
        },
      });
  }

  public changeArchiveStatusToProduct(
    productId: string,
    newStatus: boolean,
    event: Event
  ): void {
    event.stopPropagation();
    const product = this.products.find((p) => p.productId === productId);
    if (!product) {
      return;
    }

    this.productService
      .changeArchiveStatus(productId, newStatus)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {
          product.isArchive = newStatus;
          this.applyFilters();
        },
      });
  }

  public handleRowClick(productId: string) {
    const selection = window.getSelection();
    if (!selection || selection.toString().length === 0) {
      this.router.navigate(['/admin/update-product', productId]);
    }
  }
}
