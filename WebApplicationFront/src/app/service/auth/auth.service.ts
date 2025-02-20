import { HttpClient } from '@angular/common/http';
import { effect, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { CookiesManagerService } from '../cookies-manager/cookies-manager.service';
import { Observable } from 'rxjs';
import { RegisterUser } from '../../data/interfaces/user/register-user.interface';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private baseApiUrl = environment.apiUrl;
  public userEMail?: string;
  public userEMailKey: string = 'userEmail';

  constructor(
    private http: HttpClient,
    private cookiesManager: CookiesManagerService
  ) {
    this.userEMail = localStorage.getItem(this.userEMailKey)?.toString();
  }

  public login(payload: {
    eMail: string;
    password: string;
  }): Observable<{ role: string }> {
    return this.http.post<{ role: string }>(
      `${this.baseApiUrl}/login`,
      payload,
      {
        withCredentials: true,
      }
    );
  }

  public isLoggedIn(): boolean {
    if (this.cookiesManager.getAuthCookie() !== null) {
      return true;
    }
    return false;
  }

  public register(payload: RegisterUser): Observable<any> {
    return this.http.post<any>(`${this.baseApiUrl}/register`, payload, {
      withCredentials: true,
    });
  }
}
