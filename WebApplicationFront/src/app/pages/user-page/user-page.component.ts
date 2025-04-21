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
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-user-page',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './user-page.component.html',
  styleUrls: ['./user-page.component.scss'],
})
export class UserPageComponent implements OnInit, OnDestroy {
  public user!: UserProfile;
  public userForm!: FormGroup;
  public editMode: boolean = false;
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
        this.editMode = false;
      });
  }

  public logout(): void {
    this.authService.logout();
    this.router.navigate(['']);
  }

  public toggleEditMode(): void {
    this.editMode = !this.editMode;
    if (this.editMode) {
      this.userForm.patchValue(this.user);
    }
  }

  public cancelEdit(): void {
    this.editMode = false;
    this.userForm.reset();
    this.userForm.patchValue(this.user);
  }

  public formatAddress(): string {
    if (!this.user) return '';

    const parts = [
      this.user.city,
      this.user.street,
      this.user.houseNumber,
      this.user.flat ? `кв. ${this.user.flat}` : '',
    ].filter((part) => !!part);

    return parts.join(', ');
  }

  public formatPhoneNumber(phone: string): string {
    if (!phone) return '';
    return phone.replace(
      /(\+375)(\d{2})(\d{3})(\d{2})(\d{2})/,
      '$1 ($2) $3-$4-$5'
    );
  }

  public goToComments(): void {
    this.router.navigate(['/user/comments'], {
      state: { userName: this.user.firstName },
    });
  }
}
