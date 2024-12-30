import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private http: HttpClient) {}

  baseApiUrl = environment.apiUrl + '/login';

  public login(payload: {
    eMail: string,
    password: string;
  }) {
    this.http.post(this.baseApiUrl, payload, { withCredentials: true }).subscribe();
  }
}
