import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../service/auth/auth.service';
import { AdminService } from '../../service/admin/admin.service';
import { CommonModule } from '@angular/common';
import { UserService } from '../../service/user/user.service';
import { ProductService } from '../../service/product.service';
import { Subject, takeUntil } from 'rxjs';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-header',
  imports: [RouterModule, CommonModule, FormsModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent implements OnInit, OnDestroy {
  public userEmail?: string = undefined;
  public searchQuery: string = '';
  private unsubscribe$ = new Subject<void>();

  constructor(
    public router: Router,
    public authService: AuthService,
    public userService: UserService,
    public adminService: AdminService,
    private productService: ProductService
  ) {}

  public ngOnInit(): void {
    this.userEmail = this.userService.userEMail?.slice(0, 3).toUpperCase();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public searchByName(): void {
    this.productService
      .searchByName(this.searchQuery, 1)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((val) => {
        console.log(val);
      });
  }
}
