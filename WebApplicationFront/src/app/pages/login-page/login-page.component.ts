import { Component, Input } from '@angular/core';
import { LoginModel } from '../../data/interfaces/loginModel.interface';
import { AuthService } from '../../service/auth/auth.service';

@Component({
  selector: 'app-login-page',
  imports: [],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.scss'
})
export class LoginPageComponent {
  loginModel: LoginModel | undefined;
  constructor(private authService: AuthService) {
    this.authService.getProductCards().subscribe((val) => {
      this.loginModel = val;
      console.log(this.loginModel);
    });
  }
}
