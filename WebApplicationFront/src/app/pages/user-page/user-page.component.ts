import { Component, OnDestroy, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { UserService } from '../../service/user/user.service';
import { AuthService } from '../../service/auth/auth.service';
import { UserProfile } from '../../data/interfaces/user/user-profile';
import { Subject, takeUntil } from 'rxjs';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { UserPaidOrder } from '../../data/interfaces/paid-order/user-paid-order.interface';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { CommentForUserResponse } from '../../data/interfaces/Comment/comment-for-user-response.interface';

@Component({
  selector: 'app-user-page',
  imports: [CommonModule, ReactiveFormsModule, ScrollingModule],
  templateUrl: './user-page.component.html',
  styleUrls: ['./user-page.component.scss'],
})
export class UserPageComponent implements OnInit, OnDestroy {
  public user!: UserProfile;
  public userForm!: FormGroup;
  public paidOrders: UserPaidOrder[] = [];
  public comments: CommentForUserResponse[] = [];
  private unsubscribe$ = new Subject<void>();

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private fb: FormBuilder,
    private router: Router
  ) {}

  public ngOnInit(): void {
    this.initForm();
    this.getUser();
    this.getPaidOrder();
    this.getUserComments();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  private initForm(): void {
    this.userForm = this.fb.group({
      firstName: ['', Validators.required],
      surname: ['', Validators.required],
      telephone: [
        '',
        [
          Validators.required,
          Validators.pattern(/^\+375\s\d{2}\s\d{3}-\d{2}-\d{2}$/),
        ],
      ],
      city: [''],
      street: [''],
      houseNumber: [''],
      entrance: [''],
      flat: [''],
      email: ['', [Validators.required, Validators.email]],
    });
  }

  private getUser(): void {
    this.userService
      .getUser()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (user) => {
          this.user = user;
          this.userForm.patchValue(this.user);
        },
        error: (error) => {
          if (error.status == 401) {
            this.logout();
          }
        },
      });
  }

  private getPaidOrder(): void {
    this.userService
      .getPaidOrders()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((paidOrders) => (this.paidOrders = paidOrders));
  }

  private getUserComments(): void {
    this.userService
      .getUserComments()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (val) => {
          this.comments = val;
        },
      });
  }

  public getOrderTotal(order: UserPaidOrder): number {
    return order.paidOrderItems.reduce(
      (total, item) => total + item.price * item.quantity,
      0
    );
  }

  public updateUser(): void {
    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      return;
    }

    this.userService
      .updateUser(this.userForm.value)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((user) => {
        this.user = user;
        this.userForm.patchValue(this.user);
        this.userService.userEMail = user.email;
        localStorage.setItem(this.userService.userEMailKey, user.email);
      });
  }

  public logout(): void {
    this.authService.logout();
    this.router.navigate(['']);
  }

  public goToProduct(productId: string): void {
    const selection = window.getSelection();
    if (selection && selection.toString().length > 0) {
      return;
    }
    this.router.navigate(['/product', productId]);
  }
}
