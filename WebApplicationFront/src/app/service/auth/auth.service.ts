import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoginModel } from '../../data/interfaces/loginModel.interface';

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
    this.http.post<LoginModel>('https://localhost:7295/login', payload, { withCredentials: true }).subscribe()
  }
}
