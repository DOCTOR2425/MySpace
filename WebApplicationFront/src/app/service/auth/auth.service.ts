import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { LoginModel } from '../../data/interfaces/loginModel.interface';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor(private http: HttpClient) { }

  baseApiUrl = 'https://localhost:7295/1';

  public getProductCards(): Observable<LoginModel> {
    return this.http.get<LoginModel>(`${this.baseApiUrl}`);
  }
}
