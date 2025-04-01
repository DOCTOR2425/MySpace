import { Component, OnInit } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Subject, takeUntil } from 'rxjs';
import { AdminService } from '../../../service/admin/admin.service';
import { AdminPaidOrder } from '../../../data/interfaces/paid-order/admin-paid-order.interface';
import { ActivatedRoute } from '@angular/router';

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
    private route: ActivatedRoute
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
        next: () => {},
      });
  }

  public cancelOrder(): void {
    this.adminService
      .cancelOrder(this.order.paidOrderId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {},
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
}
