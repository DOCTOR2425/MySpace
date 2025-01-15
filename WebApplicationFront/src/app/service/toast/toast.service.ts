import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root',
})
export class ToastService {
  constructor(private toastr: ToastrService) {}
  
  public showError(message: string, title: string): void {
    this.toastr.error(message, title);
  }

  public showSuccess(message: string, title: string): void {
    this.toastr.success(message, title);
  }
}
