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

  constructor(
    private http: HttpClient,
    private cookiesManager: CookiesManagerService
  ) {}

  public login(payload: {
    eMail: string;
    password: string;
  }): Observable<Object> {
    return this.http.post(`${this.baseApiUrl}/login`, payload, {
      withCredentials: true,
    });
  }

  public isLoggedIn(): boolean {
    if (this.cookiesManager.getAuthCookie() !== null) {
      return true;
    }
    return false;
  }

  public test(): Observable<Object> {
    return this.http.get(`${this.baseApiUrl}/api/Admin/test`, {
      withCredentials: true,
    });
  }
}
