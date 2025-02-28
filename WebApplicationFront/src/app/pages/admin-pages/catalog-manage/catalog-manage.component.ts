import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-catalog-manage',
  imports: [CommonModule, RouterModule],
  templateUrl: './catalog-manage.component.html',
  styleUrl: './catalog-manage.component.scss'
})
export class CatalogManageComponent {
  products = [
    { id: 1, name: 'Молоток', category: 'Ручные инструменты', price: 500, stock: 10 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 2, name: 'Отвертка', category: 'Ручные инструменты', price: 300, stock: 25 },
    { id: 3, name: 'Дрель', category: 'Электроинструменты', price: 2500, stock: 5 }
  ];

  // Метод для удаления товара
  deleteProduct(productId: number) {
    this.products = this.products.filter(product => product.id !== productId);
    console.log(`Товар с ID ${productId} удален`);
  }

  // Метод для редактирования товара
  editProduct(productId: number) {
    console.log(`Редактировать товар с ID ${productId}`);
    // Здесь можно добавить логику для редактирования товара
  }

  // Метод для добавления товара
  addProduct() {
    console.log('Добавить новый товар');
    // Здесь можно добавить логику для добавления товара
  }
}
