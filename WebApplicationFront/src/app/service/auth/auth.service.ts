import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private http: HttpClient) {}

  baseApiUrl = 'https://localhost:7295/login';

  public login(payload: {
    eMail: string,
    password: string;
  }) {
    this.http.post('https://localhost:7295/login', payload, { withCredentials: true }).subscribe()
  }
}
