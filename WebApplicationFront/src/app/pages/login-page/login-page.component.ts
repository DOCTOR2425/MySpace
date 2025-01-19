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

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.scss'],
})
export class LoginPageComponent implements OnInit, OnDestroy {
  public returnUrl: string = '';
  private unsubscribe$ = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
    private adminService: AdminService,
    private toastService: ToastService
  ) {}

  form = new FormGroup({
    eMail: new FormControl('', Validators.required),
    password: new FormControl('', Validators.required),
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

  public onSubmit(): void {
    if (this.form.valid) {
      const formValue = {
        eMail: this.form.get('eMail')!.value!,
        password: this.form.get('password')!.value!,
      };
      this.authService.login(formValue).subscribe({
        next: (response) => {
          if (response.role === 'admin') {
            this.adminService.isAdmin = true;
            console.log('LogIn as Admin');
            this.router.navigate([`admin`]);
          } else {
            this.authService.userEMail = formValue.eMail;
            this.router.navigate([`${this.returnUrl}`]);
          }
        },
        error: (error) => {
          console.log(error.status);
          this.toastService.showError(error.message, 'Ошибка');
        },
      });
    }
  }
}
