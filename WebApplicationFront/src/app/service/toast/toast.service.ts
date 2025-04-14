import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root',
})
export class ToastService {
  constructor(private toastr: ToastrService) {}

  public showError(message: string, title: string = 'Ошибка'): void {
    this.toastr.error(message, title, {
      timeOut: 3000,
    });
  }

  public showSuccess(message: string, title: string): void {
    this.toastr.success(message, title, {
      timeOut: 1000,
    });
  }

  public showInfo(message: string, title: string): void {
    this.toastr.show(message, title, {
      timeOut: 1500,
    });
  }
}
