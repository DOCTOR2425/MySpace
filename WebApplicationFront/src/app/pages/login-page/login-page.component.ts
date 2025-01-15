import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../service/auth/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-login-page',
  imports: [ReactiveFormsModule],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.scss',
})
export class LoginPageComponent implements OnInit, OnDestroy {
  public returnUrl: string = '';
  private unsubscribe$ = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService
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
          this.router.navigate([`/${this.returnUrl}`]);
        },
        error: (error) => {
          console.log(error.status);
          
        }
      });
    }
  }
}
