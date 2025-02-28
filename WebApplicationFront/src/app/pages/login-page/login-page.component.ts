import { Component, OnDestroy, OnInit } from '@angular/core';
import {
  FormControl,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { AuthService } from '../../service/auth/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { ToastService } from '../../service/toast/toast.service';
import { AdminService } from '../../service/admin/admin.service';
import { CommonModule } from '@angular/common';
import { UserService } from '../../service/user/user.service';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss'],
})
export class LoginPageComponent implements OnInit, OnDestroy {
  public returnUrl: string = '';
  public isLoginMode: boolean = true;
  private unsubscribe$ = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
    private userService: UserService,
    private adminService: AdminService,
    private toastService: ToastService
  ) {}

  loginForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    // email: new FormControl('', [Validators.required]),
    password: new FormControl('', Validators.required),
  });

  registerForm = new FormGroup({
    firstName: new FormControl('', Validators.required),
    surname: new FormControl('', Validators.required),
    telephone: new FormControl('', Validators.required),
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', Validators.required),
    city: new FormControl('', Validators.required),
    street: new FormControl('', Validators.required),
    houseNumber: new FormControl('', Validators.required),
    entrance: new FormControl('', Validators.required),
    flat: new FormControl('', Validators.required),
  });

  ngOnInit(): void {
    this.route.queryParams
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((params) => {
        this.returnUrl = params['returnUrl'] || '/';
      });
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public setMode(mode: 'login' | 'register'): void {
    this.isLoginMode = mode === 'login';
  }

  public onSubmit(): void {
    if (this.isLoginMode) {
      if (this.loginForm.valid) {
        const loginValue = this.loginForm.value as {
          email: string;
          password: string;
        };

        this.authService
          .login({
            email: loginValue.email!,
            password: loginValue.password!,
          })
          .subscribe({
            next: (response) => {
              if (response.role === 'admin') {
                this.adminService.isAdmin = true;
                console.log('LogIn as Admin');
                this.router.navigate(['admin']);
              } else {
                localStorage.setItem(
                  this.userService.userEMailKey,
                  loginValue.email
                );
                this.userService.userEMail = loginValue.email;
                this.router.navigate([`${this.returnUrl}`]);
              }
            },
            error: (error) => {
              console.log(error.status);
              this.toastService.showError(error.message, 'Ошибка');
            },
          });
      }
    } else {
      if (this.registerForm.valid) {
        const registerValue = this.registerForm.value as {
          firstName: string;
          surname: string;
          telephone: string;
          email: string;
          password: string;
          city: string;
          street: string;
          houseNumber: string;
          entrance: string;
          flat: string;
        };

        this.authService
          .register({
            firstName: registerValue.firstName!,
            surname: registerValue.surname!,
            telephone: registerValue.telephone!,
            email: registerValue.email!,
            password: registerValue.password!,
            city: registerValue.city!,
            street: registerValue.street!,
            houseNumber: registerValue.houseNumber!,
            entrance: registerValue.entrance!,
            flat: registerValue.flat!,
          })
          .subscribe({
            next: () => {
              this.toastService.showSuccess(
                'Registration successful!',
                'Success'
              );
              this.router.navigate([`${this.returnUrl}`]);
            },
            error: (error) => {
              console.log(error.status);
              this.toastService.showError(error.message, 'Ошибка');
            },
          });
      }
    }
  }
}
