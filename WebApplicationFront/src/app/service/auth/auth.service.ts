import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { CookiesManagerService } from '../cookies-manager/cookies-manager.service';
import { Observable } from 'rxjs';
import { RegisterUser } from '../../data/interfaces/user/register-user.interface';
import { UserService } from '../user/user.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private baseApiUrl = environment.apiUrl + '/api/Account/';

  constructor(
    private http: HttpClient,
    private cookiesManager: CookiesManagerService,
    private userService: UserService
  ) {}

  public login(email: string): Observable<{ role: string }> {
    return this.http.post<{ role: string }>(
      `${this.baseApiUrl}login-first-stage/${email}`,
      null,
      { withCredentials: true }
    );
  }

  public register(payload: RegisterUser): Observable<any> {
    return this.http.post<any>(
      `${this.baseApiUrl}register-first-stage`,
      payload,
      { withCredentials: true }
    );
  }

  public logout(): void {
    this.cookiesManager.deleteAuthCookie();
    localStorage.setItem(this.userService.userEMailKey, '');
  }

  public isLoggedIn(): boolean {
    if (this.cookiesManager.getAuthCookie() !== null) {
      return true;
    }
    return false;
  }

  public verifyCode(
    email: string,
    code: string
  ): Observable<{ role: string; isBlocked: boolean }> {
    return this.http.post<{ role: string; isBlocked: boolean }>(
      `${this.baseApiUrl}verify-login-code/${email}/${code}`,
      null,
      { withCredentials: true }
    );
  }
}
