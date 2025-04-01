import { Component, OnDestroy, OnInit } from '@angular/core';
import { AdminPaidOrder } from '../../../data/interfaces/paid-order/admin-paid-order.interface';
import { AdminService } from '../../../service/admin/admin.service';
import { Subject, takeUntil } from 'rxjs';
import { CommonModule } from '@angular/common';
import { AdminOrderComponent } from '../../../common-ui/admin-order/admin-order.component';

@Component({
  selector: 'app-orders-page',
  standalone: true,
  imports: [CommonModule, AdminOrderComponent],
  templateUrl: './orders-page.component.html',
  styleUrls: ['./orders-page.component.scss'],
})
export class OrdersPageComponent implements OnInit, OnDestroy {
  public allOrders: AdminPaidOrder[] = [];
  private unsubscribe$ = new Subject<void>();

  public pageNumber: number = 1;

  filteredOrders: AdminPaidOrder[] = [];
  currentFilter: 'all' | 'active' | 'completed' | 'cancelled' = 'active';

  constructor(private adminService: AdminService) {}

  public ngOnInit(): void {
    this.adminService
      .getAllOrders(this.pageNumber)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((orders) => {
        this.allOrders = orders;
        this.applyFilter();
      });
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public applyFilter(filter: string = this.currentFilter): void {
    this.currentFilter = filter as any;

    switch (filter) {
      case 'active':
        this.filteredOrders = this.allOrders.filter((o) =>
          this.isOrderActive(o)
        );
        break;
      case 'completed':
        this.filteredOrders = this.allOrders.filter((o) =>
          this.isOrderCompleted(o)
        );
        break;
      case 'cancelled':
        this.filteredOrders = this.allOrders.filter((o) =>
          this.isOrderCancelled(o)
        );
        break;
      default:
        this.filteredOrders = [...this.allOrders];
    }
  }

  private isOrderActive(order: AdminPaidOrder): boolean {
    return order.receiptDate.toString() == '0001-01-01T00:00:00';
  }

  private isOrderCompleted(order: AdminPaidOrder): boolean {
    return (
      order.receiptDate.toString() != '0001-01-01T00:00:00' &&
      order.receiptDate.toString() != '9999-12-31T23:59:59.9999999'
    );
  }

  private isOrderCancelled(order: AdminPaidOrder): boolean {
    return order.receiptDate.toString() == '9999-12-31T23:59:59.9999999';
  }

  public onOrderRemoved(orderId: string): void {
    this.allOrders = this.allOrders.filter((o) => o.paidOrderId !== orderId);
    this.applyFilter();
  }

  public getOrderCount(filter: string): number {
    switch (filter) {
      case 'active':
        return this.allOrders.filter((o) => this.isOrderActive(o)).length;
      case 'completed':
        return this.allOrders.filter((o) => this.isOrderCompleted(o)).length;
      case 'cancelled':
        return this.allOrders.filter((o) => this.isOrderCancelled(o)).length;
      default:
        return this.allOrders.length;
    }
  }

  public getStatusClass(order: AdminPaidOrder): string {
    if (this.isOrderCancelled(order)) return 'cancelled';
    if (this.isOrderCompleted(order)) return 'completed';
    return 'active';
  }

  public getStatusText(order: AdminPaidOrder): string {
    if (this.isOrderCancelled(order)) return 'Отменен';
    if (this.isOrderCompleted(order)) return 'Завершен';
    return 'В обработке';
  }

  public uploadMoreOrders() {
    this.pageNumber++;
    this.adminService
      .getAllOrders(this.pageNumber)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (val) => {
          val.forEach((order) => this.allOrders.push(order));
        },
      });
  }
}
