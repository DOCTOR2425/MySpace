import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ReportService {
  private baseApiUrl = environment.apiUrl + '/api/Report/';
  constructor(private http: HttpClient) {}

  public generateReportSalesByCategoryOverTime(
    from: string,
    to: string
  ): Observable<Blob> {
    return this.http.get(
      `${this.baseApiUrl}generate-report-sales-by-category-over-time` +
        `?from=${from}&to=${to}`,
      {
        withCredentials: true,
        responseType: 'blob',
      }
    );
  }

  public generateExcelStockReport(from: string, to: string): Observable<Blob> {
    return this.http.get(
      `${this.baseApiUrl}generate-stock-report-over-time` +
        `?from=${from}&to=${to}`,
      {
        withCredentials: true,
        responseType: 'blob',
      }
    );
  }

  public generateOrdersReport(from: string, to: string): Observable<Blob> {
    return this.http.get(
      `${this.baseApiUrl}generate-orders-report?from=${from}&to=${to}`,
      {
        withCredentials: true,
        responseType: 'blob',
      }
    );
  }

  public generateProfitFromUsersReport(
    from: string,
    to: string
  ): Observable<Blob> {
    return this.http.get(
      `${this.baseApiUrl}generate-profit-from-users-report` +
        `?from=${from}&to=${to}`,
      {
        withCredentials: true,
        responseType: 'blob',
      }
    );
  }
}
