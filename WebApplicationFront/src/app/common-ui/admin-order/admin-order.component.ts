import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
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
  styleUrl: './admin-order.component.scss',
})
export class AdminOrderComponent implements OnDestroy {
  private unsubscribe$ = new Subject<void>();
  @Input() order!: AdminPaidOrder;
  @Output() removeOrder = new EventEmitter<string>();

  constructor(private adminService: AdminService) {}

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

  public closeOrder(orderId: string): void {
    this.adminService
      .closeOrder(orderId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {
          this.removeOrder.emit(orderId);
        },
      });
  }

  public cancelOrder(orderId: string): void {
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
