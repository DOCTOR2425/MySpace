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
import { UpdateUserRequest } from '../../data/interfaces/user/update-user.interface';
import { Router } from '@angular/router';
import { UserPaidOrder } from '../../data/interfaces/paid-order/user-paid-order.interface';

@Component({
  selector: 'app-user-page',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './user-page.component.html',
  styleUrls: ['./user-page.component.scss'],
})
export class UserPageComponent implements OnInit, OnDestroy {
  public user!: UserProfile;
  public userForm!: FormGroup;
  public paidOrders: UserPaidOrder[] = [];
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
    // this.getPaidOrder();
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
          this.getPaidOrder();
        },
        error: (error) => {
          if (error.status == 401) {
            this.authService.logout();
            this.router.navigate(['/']);
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
    let updatedUser: UpdateUserRequest = {
      firstName: this.userForm.value.firstName,
      surname: this.userForm.value.surname,
      telephone: this.userForm.value.telephone,
      email: this.userForm.value.email,
      city: this.userForm.value.city,
      street: this.userForm.value.street,
      houseNumber: this.userForm.value.houseNumber,
      entrance: this.userForm.value.entrance,
      flat: this.userForm.value.flat,
    };

    this.userService
      .updateUser(updatedUser)
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
}
