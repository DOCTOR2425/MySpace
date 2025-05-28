import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { UserCommentResponse } from '../../../../data/interfaces/comment/user-comment-response.interface';
import { AdminUser } from '../../../../data/interfaces/user/admin-user.interface';
import { UserService } from '../../../../service/user/user.service';
import { Subject, takeUntil } from 'rxjs';
import { ToastService } from '../../../../service/toast/toast.service';
import { CommonModule } from '@angular/common';
import { AdminPaidOrder } from '../../../../data/interfaces/paid-order/admin-paid-order.interface';
import { AdminService } from '../../../../service/admin/admin.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-update-user',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './update-user.component.html',
  styleUrl: './update-user.component.scss',
})
export class UpdateUserComponent implements OnInit, OnDestroy {
  public user!: AdminUser;
  public usersComments: UserCommentResponse[] = [];
  public usersPaidOrders: AdminPaidOrder[] = [];
  public filteredOrders: AdminPaidOrder[] = [];
  public filteredComments: UserCommentResponse[] = [];
  public isPopupOpen = false;
  public blockReasonText = '';
  public blockReasons = [
    'За нарушение правил площадки',
    'За спам комментариев',
    'За спам заказов',
  ];
  public activeTab: 'orders' | 'comments' = 'orders';
  public orderFilter = 'all';
  public commentSearch = '';
  private expandedOrders: Set<string> = new Set();
  private unsubscribe$ = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private toastService: ToastService,
    private adminService: AdminService
  ) {}

  public ngOnInit(): void {
    const userId = this.route.snapshot.paramMap.get('id')!;

    this.userService
      .getUserForAdmin(userId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (user) => {
          this.user = user;
        },
        error: (error) => {
          this.toastService.showError(error.error.error);
        },
      });

    this.userService
      .getUserComments(userId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (comments) => {
          this.usersComments = comments;
          this.filteredComments = [...comments];
        },
        error: (error) => {
          this.toastService.showError(error.error.error);
        },
      });

    this.adminService
      .getUserPaidOrdersForAdmin(userId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (orders) => {
          this.usersPaidOrders = orders;
          this.filteredOrders = [...orders];
        },
        error: (error) => {
          this.toastService.showError(error.error.error);
        },
      });
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public formatPhoneNumber(phone: string): string {
    if (!phone) return '';
    return phone.replace(
      /(\+375)(\d{2})(\d{3})(\d{2})(\d{2})/,
      '$1 ($2) $3-$4-$5'
    );
  }

  public isOrderExpanded(orderId: string): boolean {
    return this.expandedOrders.has(orderId);
  }

  public toggleOrder(orderId: string): void {
    if (this.expandedOrders.has(orderId)) {
      this.expandedOrders.delete(orderId);
    } else {
      this.expandedOrders.add(orderId);
    }
  }

  public applyOrderFilter(): void {
    const now = new Date();
    this.filteredOrders = this.usersPaidOrders.filter((order) => {
      if (this.orderFilter === 'all') return true;

      const orderDate = new Date(order.orderDate);
      const diff = now.getTime() - orderDate.getTime();

      if (this.orderFilter === 'month') {
        return diff <= 30 * 24 * 60 * 60 * 1000;
      } else if (this.orderFilter === 'year') {
        return diff <= 365 * 24 * 60 * 60 * 1000;
      }
      return true;
    });
  }

  public filterComments(): void {
    if (!this.commentSearch) {
      this.filteredComments = [...this.usersComments];
      return;
    }

    const searchTerm = this.commentSearch.toLowerCase();
    this.filteredComments = this.usersComments.filter(
      (comment) =>
        comment.text.toLowerCase().includes(searchTerm) ||
        comment.productName.toLowerCase().includes(searchTerm)
    );
  }

  public selectReason(reason: string): void {
    this.blockReasonText = reason;
  }

  public blockUser(): void {
    if (!this.blockReasonText) return;

    this.userService
      .blockUser(this.user.userId, this.blockReasonText)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {
          this.user.blockDate = new Date();
          this.user.blockDetails = this.blockReasonText;
          this.closePopup();
        },
        error: (error) => {
          this.toastService.showError(error.error.error);
          this.closePopup();
        },
      });
  }

  public unblockUser(): void {
    this.userService
      .unblockUser(this.user.userId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {
          this.user.blockDate = undefined;
          this.user.blockDetails = undefined;
        },
        error: (error) => {
          this.toastService.showError(error.error.error);
        },
      });
  }

  public closePopup(): void {
    this.isPopupOpen = false;
    this.blockReasonText = '';

    document.body.style.overflow = '';
    document.body.style.position = '';
    document.body.style.width = '';
  }

  public openPopup(): void {
    this.isPopupOpen = true;
    this.blockReasonText = '';

    document.body.style.overflow = 'hidden';
    document.body.style.position = 'fixed';
    document.body.style.width = '100%';
  }

  public getOrderTotal(order: AdminPaidOrder): number {
    return order.paidOrderItems.reduce(
      (total, item) => total + item.price * item.quantity,
      0
    );
  }

  public getOrderTotalWithPromoCode(order: AdminPaidOrder): number {
    let amount = this.getOrderTotal(order);

    if (order.promoCode)
      if (amount - order.promoCode.amount > 0)
        return amount - order.promoCode.amount;
      else if (amount - order.promoCode.amount <= 0) return 0;

    return amount;
  }

  public isOrderInProcess(order: AdminPaidOrder): boolean {
    if (order.receiptDate.toString() == '0001-01-01T00:00:00') return true;
    return false;
  }

  public isOrderReceipted(order: AdminPaidOrder): boolean {
    if (
      this.isOrderCanceled(order) == false &&
      this.isOrderInProcess(order) == false
    )
      return true;
    return false;
  }

  public isOrderCanceled(order: AdminPaidOrder): boolean {
    if (order.receiptDate.toString() == '9999-12-31T23:59:59.9999999')
      return true;
    return false;
  }
}
