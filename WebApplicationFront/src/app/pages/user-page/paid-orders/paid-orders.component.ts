import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { UserService } from '../../../service/user/user.service';
import { Subject, takeUntil } from 'rxjs';
import { UserPaidOrder } from '../../../data/interfaces/paid-order/user-paid-order.interface';
import { Router } from '@angular/router';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-paid-orders',
  imports: [ScrollingModule, CommonModule],
  templateUrl: './paid-orders.component.html',
  styleUrl: './paid-orders.component.scss',
})
export class PaidOrdersComponent implements OnInit, OnDestroy {
  public paidOrders: UserPaidOrder[] = [];
  public filteredOrders: UserPaidOrder[] = [];
  public viewportHeight: string = '100vh';
  private unsubscribe$ = new Subject<void>();

  constructor(private userService: UserService, private router: Router) {}

  public ngOnInit(): void {
    this.userService
      .getPaidOrders()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((paidOrders) => {
        this.paidOrders = paidOrders;
        this.filteredOrders = paidOrders;
      });
    this.updateViewportHeight();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: Event): void {
    this.updateViewportHeight();
  }

  private updateViewportHeight(): void {
    const filterHeaderHeight =
      document.querySelector('.sticky-header')?.clientHeight || 0;
    this.viewportHeight = `calc(100vh - var(--header-height) - ${filterHeaderHeight}px - 34px)`;
  }

  public getOrderTotal(order: UserPaidOrder): number {
    return order.paidOrderItems.reduce(
      (total, item) => total + item.price * item.quantity,
      0
    );
  }

  public getOrderTotalWithPromoCode(order: UserPaidOrder): number {
    let amount = this.getOrderTotal(order);

    if (order.promoCode)
      if (amount - order.promoCode.amount > 0)
        return amount - order.promoCode.amount;
      else if (amount - order.promoCode.amount <= 0) return 0;

    return amount;
  }

  public goToProduct(productId: string): void {
    const selection = window.getSelection();
    if (selection && selection.toString().length > 0) {
      return;
    }
    this.router.navigate(['/product', productId]);
  }

  public filterOrders(event: Event): void {
    const status = (event.target as HTMLSelectElement).value;
    if (status === 'all') {
      this.filteredOrders = this.paidOrders;
    } else if (status === 'delivered') {
      this.filteredOrders = this.paidOrders.filter((order) =>
        this.isOrderCompleted(order)
      );
    } else if (status === 'in_delivery') {
      this.filteredOrders = this.paidOrders.filter((order) =>
        this.isOrderActive(order)
      );
    } else if (status === 'cancelled') {
      this.filteredOrders = this.paidOrders.filter((order) =>
        this.isOrderCancelled(order)
      );
    }
  }

  public getStatusClass(order: UserPaidOrder): string {
    if (this.isOrderCancelled(order)) return 'Отменен';
    if (this.isOrderCompleted(order)) return 'Доставлен';
    return 'status-active';
  }

  public getStatusText(order: UserPaidOrder): string {
    if (this.isOrderCancelled(order)) return 'Отменен';
    if (this.isOrderCompleted(order)) return 'Доставлен';
    return 'Доставляется';
  }

  public isOrderActive(order: UserPaidOrder): boolean {
    return order.receiptDate.toString() == '0001-01-01T00:00:00';
  }

  public isOrderCompleted(order: UserPaidOrder): boolean {
    return (
      order.receiptDate.toString() != '0001-01-01T00:00:00' &&
      order.receiptDate.toString() != '9999-12-31T23:59:59.9999999'
    );
  }

  public isOrderCancelled(order: UserPaidOrder): boolean {
    return order.receiptDate.toString() == '9999-12-31T23:59:59.9999999';
  }
}
