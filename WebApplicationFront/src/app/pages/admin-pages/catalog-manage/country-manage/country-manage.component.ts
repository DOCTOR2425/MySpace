import { Component, OnDestroy, OnInit } from '@angular/core';
import { Country } from '../../../../data/interfaces/some/country.interface';
import { AdminService } from '../../../../service/admin/admin.service';
import { ToastService } from '../../../../service/toast/toast.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-country-manage',
  imports: [CommonModule, FormsModule],
  templateUrl: './country-manage.component.html',
  styleUrl: './country-manage.component.scss',
})
export class CountyManageComponent implements OnInit, OnDestroy {
  private unsubscribe$ = new Subject<void>();
  viewMode: 'list' | 'create' | 'edit' | 'delete' = 'list';
  countries: Country[] = [];
  filteredcountries: Country[] = [];
  searchCountry: string = '';
  newCountryName: string = '';
  editingCountry: Country = { countryId: '', name: '' };

  constructor(
    private adminService: AdminService,
    private toastService: ToastService
  ) {}

  public ngOnInit(): void {
    this.loadcountries();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  loadcountries(): void {
    this.adminService
      .getAllCountries()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (countries) => {
          this.countries = countries;
          this.filteredcountries = [...countries];
        },
        error: (error) => {
          this.toastService.showError('Ошибка загрузки стран');
        },
      });
  }

  filterCountries(): void {
    if (!this.searchCountry) {
      this.filteredcountries = [...this.countries];
      return;
    }

    const query = this.searchCountry.toLowerCase();
    this.filteredcountries = this.countries.filter((country) =>
      country.name.toLowerCase().includes(query)
    );
  }

  showCreateForm(): void {
    this.newCountryName = '';
    this.viewMode = 'create';
  }

  showEditForm(country: Country): void {
    this.editingCountry = { ...country };
    this.viewMode = 'edit';
  }

  resetView(): void {
    this.viewMode = 'list';
  }

  createCountry(): void {
    if (!this.newCountryName.trim()) return;

    this.adminService
      .createCountry(this.newCountryName.trim())
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {
          this.toastService.showSuccess('Страна успешно создана', '');
          this.loadcountries();
          this.resetView();
        },
        error: (error) => {
          this.toastService.showError(
            error.error?.error || 'Ошибка создания страны'
          );
        },
      });
  }

  updateCountry(): void {
    this.adminService
      .updateCountry(
        this.editingCountry.countryId,
        this.editingCountry.name.trim()
      )
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {
          this.toastService.showSuccess('Страна успешно обновлен', '');
          this.loadcountries();
          this.resetView();
        },
        error: (error) => {
          this.toastService.showError(
            error.error?.error || 'Ошибка обновления страны'
          );
        },
      });
  }
}
