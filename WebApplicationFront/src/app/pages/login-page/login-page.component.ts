import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../service/auth/auth.service';

@Component({
  selector: 'app-login-page',
  imports: [ReactiveFormsModule],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.scss',
})
export class LoginPageComponent {
  constructor(private authService: AuthService) {}

  form = new FormGroup({
    eMail: new FormControl('', Validators.required),
    password: new FormControl('', Validators.required),
  });

  public onSubmit(): void {
    if (this.form.valid) {
      const formValue = {
        eMail: this.form.get('eMail')!.value!,
        password: this.form.get('password')!.value!,
      };
      this.authService.login(formValue);
    }
  }
}
