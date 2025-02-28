import { Component } from '@angular/core';
import { AdminService } from '../../../service/admin/admin.service';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-admin-main-page',
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-main-page.component.html',
  styleUrl: './admin-main-page.component.scss'
})
export class AdminMainPageComponent {
  constructor(private adminService: AdminService){}
  pendingOrders = [
    { 
      id: '#12345', 
      date: '2023-10-01', 
      customer: 'Иван Иванов', 
      contact: { phone: '+375 29 123 45 67', email: 'ivan@mail.ru' }, 
      total: 1500 
    },
    { 
      id: '#12346', 
      date: '2023-10-02', 
      customer: 'Петр Петров', 
      contact: { phone: '+375 29 234 56 78', email: 'petr@mail.ru' }, 
      total: 2300 
    },
    { 
      id: '#12347', 
      date: '2023-10-03', 
      customer: 'Сергей Сергеев', 
      contact: { phone: '+375 29 345 67 89', email: 'sergey@mail.ru' }, 
      total: 1800 
    }
  ];

  // Метод для закрытия заказа
  closeOrder(orderId: string) {
    console.log(`Закрыть заказ: ${orderId}`);
    // Здесь можно добавить логику для закрытия заказа
  }

  // Метод для отмены заказа
  cancelOrder(orderId: string) {
    console.log(`Отменить заказ: ${orderId}`);
    // Здесь можно добавить логику для отмены заказа
  }
}
