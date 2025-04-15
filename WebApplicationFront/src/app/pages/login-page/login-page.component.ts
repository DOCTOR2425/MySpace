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

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss'],
})
export class LoginPageComponent implements OnDestroy {
  public isLoginMode: boolean = true;
  private unsubscribe$ = new Subject<void>();

  constructor(
    private router: Router,
    private authService: AuthService,
    private userService: UserService,
    private adminService: AdminService,
    private toastService: ToastService,
    private comparisonService: ComparisonService,
    private location: Location
  ) {}

  public loginForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', Validators.required),
  });

  public registerForm = new FormGroup({
    firstName: new FormControl('', Validators.required),
    surname: new FormControl('', Validators.required),
    telephone: new FormControl('', [
      Validators.required,
      Validators.pattern(/^\+375\s\d{2}\s\d{3}-\d{2}-\d{2}$/),
    ]),
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', Validators.required),
  });

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public setMode(mode: 'login' | 'register'): void {
    this.isLoginMode = mode === 'login';
  }

  private login(
    response: { role: string },
    loginValue: { email: string; password: string }
  ) {
    if (response.role === 'admin') {
      this.adminService.isAdmin = true;

      this.userService.userEMail = undefined;
      localStorage.setItem(this.userService.userEMailKey, '');
      this.router.navigate(['admin']);
    } else {
      localStorage.setItem(this.comparisonService.comparisonKey, '');
      localStorage.setItem(this.userService.userEMailKey, loginValue.email);
      this.userService.userEMail = loginValue.email;
      this.location.back();
    }
  }

  public onSubmit(): void {
    if (this.isLoginMode) {
      if (this.loginForm.invalid) {
        this.markFormGroupTouched(this.loginForm);
        return;
      }
      const loginValue = this.loginForm.value as {
        email: string;
        password: string;
      };

      this.authService
        .login({
          email: loginValue.email!,
          password: loginValue.password!,
        })
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe({
          next: (response) => {
            this.login(response, loginValue);
          },
          error: (error) => {
            this.toastService.showError(
              error.error.error,
              'Ошибка регистрации'
            );
          },
        });
    } else {
      if (this.registerForm.invalid) {
        this.markFormGroupTouched(this.registerForm);
        return;
      }
      const registerValue = this.registerForm.value as {
        firstName: string;
        surname: string;
        telephone: string;
        email: string;
        password: string;
      };

      this.authService
        .register({
          firstName: registerValue.firstName!,
          surname: registerValue.surname!,
          telephone: registerValue.telephone!,
          email: registerValue.email!,
          password: registerValue.password!,
        })
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe({
          next: (response) => {
            this.login(response, {
              email: registerValue.email!,
              password: registerValue.password!,
            });
          },
          error: (error) => {
            this.toastService.showError(error.message, 'Ошибка');
          },
        });
    }
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
