import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';
import { AdminPaidOrder } from '../../data/interfaces/paid-order/admin-paid-order.interface';
import { CommonModule } from '@angular/common';
import { AdminService } from '../../service/admin/admin.service';
import { Subject, takeUntil } from 'rxjs';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-admin-order',
  imports: [CommonModule, RouterLink],
  templateUrl: './admin-order.component.html',
  styleUrls: ['./admin-order.component.scss'],
})
export class AdminOrderComponent implements OnDestroy, OnInit {
  private unsubscribe$ = new Subject<void>();
  @Input() order!: AdminPaidOrder;
  @Output() removeOrder = new EventEmitter<string>();

  constructor(private adminService: AdminService) {}

  public ngOnInit(): void {
    console.log(this.order.receiptDate);
    console.log(this.order.receiptDate.toString() == '0001-01-01T00:00:00');
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public getTotalPrice(order: AdminPaidOrder): number {
    return order.paidOrderItems.reduce(
      (sum, item) => sum + item.price * item.quantity,
      0
    );
  }

  public getStatusClass(): string {
    if (this.isOrderCancelled()) return 'status-cancelled';
    if (this.isOrderCompleted()) return 'status-completed';
    return 'status-active';
  }

  public getStatusText(): string {
    if (this.isOrderCancelled()) return 'Отменен';
    if (this.isOrderCompleted()) return 'Завершен';
    return 'В обработке';
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

  public closeOrder(orderId: string, event: Event): void {
    event.stopPropagation();
    this.adminService
      .closeOrder(orderId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {
          this.removeOrder.emit(orderId);
        },
      });
  }

  public cancelOrder(orderId: string, event: Event): void {
    event.stopPropagation();
    this.adminService
      .cancelOrder(orderId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {
          this.removeOrder.emit(orderId);
        },
      });
  }
}
