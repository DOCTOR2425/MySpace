import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { CookiesManagerService } from '../cookies-manager/cookies-manager.service';
import { catchError, Observable, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private baseApiUrl = environment.apiUrl;
  public userEMail: string = '';

  constructor(
    private http: HttpClient,
    private cookiesManager: CookiesManagerService
  ) {}

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
}
