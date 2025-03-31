import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CommonModule, DatePipe } from '@angular/common';

@Component({
  selector: 'app-order-page',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './order-page.component.html',
  styleUrl: './order-page.component.scss',
})
export class OrderPageComponent implements OnInit {
  public orderForm!: FormGroup;
  currentDate: string;
  orderItems = [
    { id: 1, name: 'Товар 1', price: 1500, quantity: 2 },
    { id: 2, name: 'Товар 2', price: 2300, quantity: 1 },
  ];

  deliveryMethods = [
    { id: 1, name: 'Курьером' },
    { id: 2, name: 'Самовывоз' },
    { id: 3, name: 'Почта России' },
  ];

  paymentMethods = [
    { id: 1, name: 'Наличными при получении' },
    { id: 2, name: 'Картой онлайн' },
    { id: 3, name: 'Картой при получении' },
  ];

  constructor(private fb: FormBuilder, private datePipe: DatePipe) {
    this.currentDate =
      this.datePipe.transform(new Date(), 'dd.MM.yyyy HH:mm') || '';
  }

  ngOnInit(): void {
    this.initForm();
  }

  initForm(): void {
    this.orderForm = this.fb.group({
      phone: [
        '',
        [Validators.required, Validators.pattern(/^\+?[0-9]{10,15}$/)],
      ],
      fullName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      deliveryMethod: ['', Validators.required],
      paymentMethod: ['', Validators.required],
      address: [''],
      comment: [''],
    });

    this.orderForm.get('deliveryMethod')?.valueChanges.subscribe((method) => {
      const addressControl = this.orderForm.get('address');
      if (method === 2) {
        // Самовывоз
        addressControl?.clearValidators();
      } else {
        addressControl?.setValidators([Validators.required]);
      }
      addressControl?.updateValueAndValidity();
    });
  }

  getTotalPrice(): number {
    return this.orderItems.reduce(
      (sum, item) => sum + item.price * item.quantity,
      0
    );
  }

  cancelOrder(): void {
    if (confirm('Вы уверены, что хотите отменить заказ?')) {
      // Логика отмены заказа
      console.log('Заказ отменен');
    }
  }

  completeOrder(): void {
    if (this.orderForm.valid) {
      // Логика завершения заказа
      console.log('Заказ оформлен', this.orderForm.value);
    } else {
      this.orderForm.markAllAsTouched();
    }
  }
}
