import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-create-product',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './create-product.component.html',
  styleUrl: './create-product.component.scss',
})
export class CreateProductComponent {
  @Input() product: any = {};

  categories = [
    'Ручные инструменты',
    'Электроинструменты',
    'Садовые инструменты',
  ];
  brands = ['Bosch', 'Makita', 'DeWalt', 'Stanley'];
  countries = ['Беларусь', 'Германия', 'Китай', 'США'];

  onSubmit() {
    if (this.product.id) {
      console.log('Редактирование товара:', this.product);
    } else {
      console.log('Добавление товара:', this.product);
    }
  }
}
