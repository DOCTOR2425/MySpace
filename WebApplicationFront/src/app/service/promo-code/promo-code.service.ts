import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { Observable } from 'rxjs';
import { PromoCode } from '../../data/interfaces/paid-order/promo-code.interface';

@Injectable({
  providedIn: 'root',
})
export class PromoCodeService {
  baseApiUrl = environment.apiUrl + '/api/PromoCode/';

  constructor(private http: HttpClient) {}

  public createPromoCode(name: string, amount: number): Observable<any> {
    return this.http.post(
      `${this.baseApiUrl}create-promo-code/${name}/${amount}`,
      null,
      { withCredentials: true }
    );
  }

  public getAllPromoCodes(): Observable<PromoCode> {
    return this.http.get<PromoCode>(`${this.baseApiUrl}get-all-promo-codes`, {
      withCredentials: true,
    });
  }

  public changePromoCodeStatus(
    promoCodeId: string,
    status: boolean
  ): Observable<any> {
    return this.http.put<PromoCode>(
      `${this.baseApiUrl}change-promo-code-status/${promoCodeId}/${status}`,
      null,
      { withCredentials: true }
    );
  }
}
