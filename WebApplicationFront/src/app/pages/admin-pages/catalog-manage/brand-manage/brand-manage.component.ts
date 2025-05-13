import { Component, OnDestroy, OnInit } from '@angular/core';
import { Brand } from '../../../../data/interfaces/some/brand.interface';
import { AdminService } from '../../../../service/admin/admin.service';
import { ToastService } from '../../../../service/toast/toast.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-brand-manage',
  imports: [CommonModule, FormsModule],
  templateUrl: './brand-manage.component.html',
  styleUrl: './brand-manage.component.scss',
})
export class BrandManageComponent implements OnInit, OnDestroy {
  private unsubscribe$ = new Subject<void>();
  viewMode: 'list' | 'create' | 'edit' | 'delete' = 'list';
  brands: Brand[] = [];
  filteredBrands: Brand[] = [];
  searchBrand: string = '';
  newBrandName: string = '';
  editingBrand: Brand = { brandId: '', name: '' };

  constructor(
    private adminService: AdminService,
    private toastService: ToastService
  ) {}

  public ngOnInit(): void {
    this.loadBrands();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  loadBrands(): void {
    this.adminService
      .getAllBrands()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (brands) => {
          this.brands = brands;
          this.filteredBrands = [...brands];
        },
        error: (error) => {
          this.toastService.showError('Ошибка загрузки брендов');
        },
      });
  }

  filterBrands(): void {
    if (!this.searchBrand) {
      this.filteredBrands = [...this.brands];
      return;
    }

    const query = this.searchBrand.toLowerCase();
    this.filteredBrands = this.brands.filter((brand) =>
      brand.name.toLowerCase().includes(query)
    );
  }

  showCreateForm(): void {
    this.newBrandName = '';
    this.viewMode = 'create';
  }

  showEditForm(brand: Brand): void {
    this.editingBrand = { ...brand };
    this.viewMode = 'edit';
  }

  resetView(): void {
    this.viewMode = 'list';
  }

  createBrand(): void {
    if (!this.newBrandName.trim()) return;

    this.adminService
      .createBrand(this.newBrandName.trim())
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {
          this.toastService.showSuccess('Бренд успешно создан', '');
          this.loadBrands();
          this.resetView();
        },
        error: (error) => {
          this.toastService.showError(
            error.error?.error || 'Ошибка создания бренда'
          );
        },
      });
  }

  updateBrand(): void {
    this.adminService
      .updateBrand(this.editingBrand.brandId, this.editingBrand.name.trim())
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {
          this.toastService.showSuccess('Бренд успешно обновлен', '');
          this.loadBrands();
          this.resetView();
        },
        error: (error) => {
          this.toastService.showError(
            error.error?.error || 'Ошибка обновления бренда'
          );
        },
      });
  }
}
