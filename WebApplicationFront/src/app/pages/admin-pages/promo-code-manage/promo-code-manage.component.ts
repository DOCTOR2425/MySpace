import { Component, OnDestroy, OnInit } from '@angular/core';
import { PromoCodeService } from '../../../service/promo-code/promo-code.service';
import { PromoCode } from '../../../data/interfaces/paid-order/promo-code.interface';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ToastService } from '../../../service/toast/toast.service';
import { CommonModule } from '@angular/common';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-promo-code-manage',
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './promo-code-manage.component.html',
  styleUrl: './promo-code-manage.component.scss',
})
export class PromoCodeManageComponent implements OnInit, OnDestroy {
  private promoCodes: PromoCode[] = [];
  public filteredPromoCodes: PromoCode[] = [];
  public isLoading = false;
  public promoCodeForm: FormGroup;
  public searchTerm: string = '';
  private unsubscribe$ = new Subject<void>();

  constructor(
    private promoCodeService: PromoCodeService,
    private fb: FormBuilder,
    private toastr: ToastService
  ) {
    this.promoCodeForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(20)]],
      amount: [
        '',
        [Validators.required, Validators.min(1), Validators.max(10000)],
      ],
    });
  }

  public ngOnInit(): void {
    this.loadPromoCodes();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  private loadPromoCodes(): void {
    this.isLoading = true;
    this.promoCodeService.getAllPromoCodes().subscribe({
      next: (data: any) => {
        this.promoCodes = data;
        this.filteredPromoCodes = [...this.promoCodes];
        this.isLoading = false;
        console.log(this.filteredPromoCodes);

        this.applyFilter();
      },
      error: (err) => {
        this.toastr.showError('Ошибка при загрузке промокодов');
        this.isLoading = false;
      },
    });
  }

  public applyFilter(): void {
    if (!this.searchTerm) {
      this.filteredPromoCodes = [...this.promoCodes];
      return;
    }

    const term = this.searchTerm.toLowerCase();
    this.filteredPromoCodes = this.promoCodes.filter((promo) =>
      promo.name.toLowerCase().includes(term)
    );
  }

  public createPromoCode(): void {
    if (this.promoCodeForm.invalid) {
      return;
    }

    const { name, amount } = this.promoCodeForm.value;
    this.isLoading = true;

    this.promoCodeService.createPromoCode(name, amount).subscribe({
      next: () => {
        this.promoCodeForm.reset();
        this.loadPromoCodes();
      },
      error: (err) => {
        this.toastr.showError('Ошибка при создании промокода');
        this.isLoading = false;
      },
    });
  }

  public togglePromoCodeStatus(promoCode: PromoCode): void {
    const newStatus = !promoCode.isActive;
    console.log(promoCode);

    this.promoCodeService
      .changePromoCodeStatus(promoCode.promoCodeId, newStatus)
      .subscribe({
        next: () => {
          promoCode.isActive = newStatus;
          this.toastr.showSuccess(
            `Промокод ${newStatus ? 'активирован' : 'деактивирован'}`,
            ''
          );
          this.applyFilter();
        },
        error: (err) => {
          this.toastr.showError('Ошибка при изменении статуса промокода');
        },
      });
  }

  public copyToClipboard(text: string): void {
    navigator.clipboard.writeText(text).then(() => {
      this.toastr.showInfo('Промокод скопирован в буфер обмена', '');
    });
  }
}
