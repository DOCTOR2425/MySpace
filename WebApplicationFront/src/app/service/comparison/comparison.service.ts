import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { Product } from '../../data/interfaces/product/product.interface';
import { AuthService } from '../auth/auth.service';
import { ProductService } from '../product.service';

@Injectable({
  providedIn: 'root',
})
export class ComparisonService {
  private baseApiUrl = environment.apiUrl + '/api/Comparison/';
  public comparisonKey = 'comparison';

  constructor(
    private http: HttpClient,
    private authService: AuthService,
    private productService: ProductService
  ) {}

  public getUserComparison(): Observable<Product[]> {
    if (this.authService.isLoggedIn() == true) {
      return this.http.get<Product[]>(`${this.baseApiUrl}get-user-comparison`, {
        withCredentials: true,
      });
    } else {
      return of(this.getUserComparisonLocal());
    }
  }

  private getUserComparisonLocal(): Product[] {
    let items = localStorage.getItem(this.comparisonKey);
    return items
      ? (JSON.parse(items) as Product[]).sort((a, b) =>
          a.productResponseData.productCategory.localeCompare(
            b.productResponseData.productCategory
          )
        )
      : [];
  }

  public addToComparison(productId: string): Observable<object> {
    if (this.authService.isLoggedIn() == true) {
      return this.http.post(
        `${this.baseApiUrl}add-to-comparison/${productId}`,
        {
          withCredentials: true,
        }
      );
    } else {
      return of(this.addToComparisonLocal(productId));
    }
  }

  private addToComparisonLocal(productId: string): any {
    let items = this.getUserComparisonLocal();

    if (items.some((i) => i.productResponseData.productId == productId))
      return productId;

    this.productService.getProductById(productId).subscribe({
      next: (product) => {
        items.push(product);
        product.productResponseData.productCategory;
        localStorage.setItem(this.comparisonKey, JSON.stringify(items));
        return productId;
      },
    });
  }

  public deleteFromComparison(productId: string): Observable<object> {
    if (this.authService.isLoggedIn() == true) {
      return this.http.delete(
        `${this.baseApiUrl}delete-from-comparison/${productId}`,
        {
          withCredentials: true,
        }
      );
    } else {
      return of(this.deleteFromComparisonLocal(productId));
    }
  }

  private deleteFromComparisonLocal(productId: string): any {
    let items = this.getUserComparisonLocal();
    items = items.filter((i) => i.productResponseData.productId != productId);
    localStorage.setItem(this.comparisonKey, JSON.stringify(items));

    return productId;
  }

  public clearComparisonList(): Observable<object> {
    if (this.authService.isLoggedIn() == true) {
      return this.http.delete(`${this.baseApiUrl}clear-comparison-list`, {
        withCredentials: true,
      });
    } else {
      return of(this.clearComparisonListLocal());
    }
  }

  private clearComparisonListLocal(): any {
    localStorage.setItem(this.comparisonKey, '');
  }
}
