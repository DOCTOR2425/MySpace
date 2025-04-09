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
  private baseApiUrl = environment.apiUrl + '/api/User/';

  constructor(
    private http: HttpClient,
    private cookiesManager: CookiesManagerService,
    private userService: UserService
  ) {}

  public login(payload: {
    email: string;
    password: string;
  }): Observable<{ role: string }> {
    return this.http.post<{ role: string }>(
      `${this.baseApiUrl}login`,
      payload,
      {
        withCredentials: true,
      }
    );
  }

  public logout(): void {
    this.userService.userEMail = undefined;
    this.cookiesManager.deleteAuthCookie();
    localStorage.setItem(this.userService.userEMailKey, '');
  }

  public isLoggedIn(): boolean {
    if (this.cookiesManager.getAuthCookie() !== null) {
      return true;
    }
    return false;
  }

  public register(payload: RegisterUser): Observable<any> {
    return this.http.post<any>(`${this.baseApiUrl}register`, payload, {
      withCredentials: true,
    });
  }
}
