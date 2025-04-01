import { Component, OnDestroy, OnInit } from '@angular/core';
import { AdminService } from '../../../service/admin/admin.service';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AdminPaidOrder } from '../../../data/interfaces/paid-order/admin-paid-order.interface';
import { Subject, takeUntil } from 'rxjs';
import { AdminOrderComponent } from '../../../common-ui/admin-order/admin-order.component';

@Component({
  selector: 'app-admin-main-page',
  imports: [CommonModule, RouterModule, AdminOrderComponent],
  templateUrl: './admin-main-page.component.html',
  styleUrl: './admin-main-page.component.scss',
})
export class AdminMainPageComponent implements OnInit, OnDestroy {
  public paidOrders: AdminPaidOrder[] = [];
  private unsubscribe$ = new Subject<void>();

  constructor(private adminService: AdminService) {}

  public ngOnInit(): void {
    this.adminService
      .getProcessingOrders()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((val) => {
        this.paidOrders = val;
      });
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public getTotalPrice(order: AdminPaidOrder): number {
    let amount = 0;
    for (let item of order.paidOrderItems) {
      amount += item.price;
    }
    return amount;
  }

  public onOrderRemoved(orderId: string): void {
    this.paidOrders = this.paidOrders.filter(
      (order) => order.paidOrderId !== orderId
    );
  }
}
