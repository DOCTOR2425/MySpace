import { Injectable } from '@angular/core';
import { ToastrService, GlobalConfig } from 'ngx-toastr';

@Injectable({
  providedIn: 'root',
})
export class ToastService {
  constructor(private toastr: ToastrService) {
    this.configureToastr();
  }

  private configureToastr(): void {
    const options: Partial<GlobalConfig> = {
      positionClass: 'toast-top-right',
    };
    this.toastr.toastrConfig = { ...this.toastr.toastrConfig, ...options };
  }

  public showError(message: string, title: string = 'Ошибка'): void {
    this.toastr.error(message, title, {
      timeOut: 3000,
      positionClass: 'toast-top-right',
    });
  }

  public showSuccess(message: string, title: string): void {
    this.toastr.success(message, title, {
      timeOut: 2000,
      positionClass: 'toast-top-right',
    });
  }

  public showInfo(message: string, title: string): void {
    this.toastr.info(message, title, {
      timeOut: 1500,
      positionClass: 'toast-top-right',
    });
  }
}
