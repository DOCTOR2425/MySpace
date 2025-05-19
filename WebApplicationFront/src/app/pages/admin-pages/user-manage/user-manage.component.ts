import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { AdminUser } from '../../../data/interfaces/user/admin-user.interface';
import { UserService } from '../../../service/user/user.service';
import { Subject, takeUntil } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-user-manage',
  imports: [CommonModule, FormsModule],
  templateUrl: './user-manage.component.html',
  styleUrl: './user-manage.component.scss',
})
export class UserManageComponent implements OnInit, OnDestroy {
  public users!: AdminUser[];
  public page = 1;
  public loading = false;
  public hasMoreData = true;
  private unsubscribe$ = new Subject<void>();

  searchQuery: string | undefined;
  dateFrom: Date | undefined;
  dateTo: Date | undefined;
  isBlocked: boolean | undefined;
  hasOrders: boolean | undefined;

  constructor(private userService: UserService, private router: Router) {}

  public ngOnInit(): void {
    this.loadUsers();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public loadUsers(): void {
    this.userService
      .getUsersForAdmin(
        this.searchQuery,
        this.dateFrom,
        this.dateTo,
        this.isBlocked,
        this.hasOrders
      )
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((users) => {
        this.users = users;
      });
  }

  public isUserBlocked(user: AdminUser): boolean {
    return user.blockDate != undefined;
  }

  public resetFilters(): void {
    this.searchQuery = undefined;
    this.dateFrom = undefined;
    this.dateTo = undefined;
    this.isBlocked = undefined;
    this.hasOrders = undefined;
    this.loadUsers();
  }

  public toggleBlockUser(user: AdminUser, event: Event): void {
    event.stopPropagation();

    if (this.isUserBlocked(user)) {
      this.userService
        .unblockUser(user.userId)
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe({
          next: () => {
            user.blockDate = undefined;
            user.blockDetails = undefined;
          },
        });
    } else {
      this.userService
        .blockUser(user.userId, 'Бан')
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe({
          next: () => {
            user.blockDate = new Date();
            user.blockDetails = 'Blocked by admin';
          },
        });
    }
  }

  public handleRowClick(userId: string) {
    const selection = window.getSelection();
    if (!selection || selection.toString().length === 0) {
      this.router.navigate(['/admin/update-user', userId]);
    }
  }
}
