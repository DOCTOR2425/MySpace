import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { CookiesManagerService } from '../cookies-manager/cookies-manager.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private baseApiUrl = environment.apiUrl + '/login';

  constructor(
    private http: HttpClient,
    private cookiesManager: CookiesManagerService
  ) {}

  public login(payload: { eMail: string; password: string }) {
    this.http
      .post(this.baseApiUrl, payload, { withCredentials: true })
      .subscribe();
  }

  isLoggedIn(): boolean {
    if (this.cookiesManager.getAuthCookie() !== null) {
      return true;
    }
    return false;
  }
}
