import { Component, OnDestroy } from '@angular/core';
import {
  FormControl,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { AuthService } from '../../service/auth/auth.service';
import { Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { ToastService } from '../../service/toast/toast.service';
import { AdminService } from '../../service/admin/admin.service';
import { CommonModule } from '@angular/common';
import { UserService } from '../../service/user/user.service';
import { ComparisonService } from '../../service/comparison/comparison.service';
import { Location } from '@angular/common';
import { CartService } from '../../service/cart/cart.service';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss'],
})
export class LoginPageComponent implements OnDestroy {
  public isLoginMode: boolean = true;
  public showVerificationCode: boolean = false;
  public isLoading: boolean = false;
  private currentEmail: string = '';
  private unsubscribe$ = new Subject<void>();

  constructor(
    private router: Router,
    private authService: AuthService,
    private cartService: CartService,
    private userService: UserService,
    private adminService: AdminService,
    private toastService: ToastService,
    private comparisonService: ComparisonService,
    private location: Location
  ) {}

  public loginForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
  });

  public verificationForm = new FormGroup({
    verificationCode: new FormControl('', Validators.required),
  });

  public registerForm = new FormGroup({
    firstName: new FormControl('', Validators.required),
    surname: new FormControl('', Validators.required),
    telephone: new FormControl('', [
      Validators.required,
      Validators.pattern(/^\+375\s\d{2}\s\d{3}-\d{2}-\d{2}$/),
    ]),
    email: new FormControl('', [Validators.required, Validators.email]),
  });

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public setMode(mode: 'login' | 'register'): void {
    this.isLoginMode = mode === 'login';
    this.showVerificationCode = false;
    this.loginForm.reset();
    this.registerForm.reset();
    this.verificationForm.reset();
  }

  private login(response: { role: string }, email: string) {
    this.isLoading = false;
    if (response.role === 'admin') {
      this.adminService.isAdmin = true;
      localStorage.setItem(this.userService.userEMailKey, '');
      this.router.navigate(['admin']);
    } else {
      localStorage.setItem(this.comparisonService.comparisonKey, '');
      localStorage.setItem(this.userService.userEMailKey, email);
      if (!this.isLoginMode) {
        this.cartService.registerCart();
        this.router.navigate(['/']);
        return;
      }
      this.location.back();
    }
  }

  public onSubmit(): void {
    if (this.showVerificationCode) {
      this.verifyCode();
    } else if (this.isLoginMode) {
      this.loginFirstStage();
    } else {
      this.registerFirstStage();
    }
  }

  private verifyCode(): void {
    if (this.verificationForm.invalid) {
      this.markFormGroupTouched(this.verificationForm);
      return;
    }

    const code = this.verificationForm.value.verificationCode || '';

    this.isLoading = true;
    this.authService
      .verifyCode(this.currentEmail, code)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (response) => {
          this.login(response, this.currentEmail);
        },
        error: (error) => {
          this.isLoading = false;
          const errorMessage =
            error.error?.error || 'Неверный код подтверждения';
          this.toastService.showError(errorMessage, 'Ошибка подтверждения');
        },
      });
  }

  private loginFirstStage(): void {
    if (this.loginForm.invalid) {
      this.markFormGroupTouched(this.loginForm);
      return;
    }

    this.isLoading = true;
    this.currentEmail = this.loginForm.value.email!;
    this.authService
      .login(this.currentEmail)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {
          this.isLoading = false;
          this.showVerificationCode = true;
          this.toastService.showSuccess(
            'Код подтверждения отправлен на вашу почту',
            'Успешно'
          );
        },
        error: (error) => {
          this.isLoading = false;
          const errorMessage =
            error.error?.error || 'Не удалось отправить код подтверждения';
          this.toastService.showError(errorMessage, 'Ошибка входа');
        },
      });
  }

  private registerFirstStage(): void {
    if (this.registerForm.invalid) {
      this.markFormGroupTouched(this.registerForm);
      return;
    }

    this.isLoading = true;
    const registerValue = this.registerForm.value as {
      firstName: string;
      surname: string;
      telephone: string;
      email: string;
    };

    this.currentEmail = registerValue.email!;

    this.authService
      .register({
        firstName: registerValue.firstName!,
        surname: registerValue.surname!,
        telephone: registerValue.telephone!,
        email: registerValue.email!,
      })
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {
          this.isLoading = false;
          this.showVerificationCode = true;
          this.toastService.showSuccess(
            'Код подтверждения отправлен на вашу почту',
            'Успешно'
          );
        },
        error: (error) => {
          this.isLoading = false;
          const errorMessage =
            error.error?.error || 'Не удалось отправить код подтверждения';
          this.toastService.showError(errorMessage, 'Ошибка регистрации');
        },
      });
  }

  private markFormGroupTouched(formGroup: FormGroup) {
    Object.values(formGroup.controls).forEach((control) => {
      control.markAsTouched();

      if (control instanceof FormGroup) {
        this.markFormGroupTouched(control);
      }
    });
  }
}
