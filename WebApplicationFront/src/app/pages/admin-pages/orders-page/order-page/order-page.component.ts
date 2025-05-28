import { Component, OnInit } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Subject, takeUntil } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminPaidOrder } from '../../../../data/interfaces/paid-order/admin-paid-order.interface';
import { AdminService } from '../../../../service/admin/admin.service';

@Component({
  selector: 'app-order-page',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './order-page.component.html',
  styleUrl: './order-page.component.scss',
})
export class OrderPageComponent implements OnInit {
  public order!: AdminPaidOrder;
  private unsubscribe$ = new Subject<void>();

  constructor(
    private adminService: AdminService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  public ngOnInit(): void {
    let orderId = this.route.snapshot.paramMap.get('id')!;

    this.adminService
      .getOrderById(orderId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (val) => {
          this.order = val;
        },
      });
  }

  ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public closeOrder(): void {
    this.adminService
      .closeOrder(this.order.paidOrderId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {
          this.order.receiptDate = new Date();
        },
      });
  }

  public cancelOrder(): void {
    this.adminService
      .cancelOrder(this.order.paidOrderId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {
          this.order.receiptDate = new Date('9999-12-31T23:59:59.9999999');
        },
      });
  }

  public getTotalPrice(): number {
    return (
      this.order?.paidOrderItems.reduce(
        (sum, item) => sum + item.price * item.quantity,
        0
      ) || 0
    );
  }

  public getTotalPriceWithPromoCode(): number {
    let amount = this.getTotalPrice();

    if (this.order.promoCode)
      if (amount - this.order.promoCode.amount > 0)
        return amount - this.order.promoCode.amount;
      else if (amount - this.order.promoCode.amount <= 0) return 0;

    return amount;
  }

  public isOrderActive(): boolean {
    return this.order.receiptDate.toString() == '0001-01-01T00:00:00';
  }

  public isOrderCompleted(): boolean {
    return (
      this.order.receiptDate.toString() != '0001-01-01T00:00:00' &&
      this.order.receiptDate.toString() != '9999-12-31T23:59:59.9999999'
    );
  }

  public isOrderCancelled(): boolean {
    return this.order.receiptDate.toString() == '9999-12-31T23:59:59.9999999';
  }

  public handleRowClick(productId: string) {
    const selection = window.getSelection();
    if (!selection || selection.toString().length === 0) {
      this.router.navigate(['/admin/update-product', productId]);
    }
  }
}
