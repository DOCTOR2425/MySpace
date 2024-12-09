import { Component, inject } from '@angular/core';
import { ProductCardComponent } from './common-ui/product-card/product-card.component';
import { Product } from './data/interfaces/product.interface';
import { ProductService } from './service/product.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [ProductCardComponent, CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  productService = inject(ProductService);
  products: Product[] = [];
  constructor() {
    this.productService.getTestProduct().subscribe((val) => {
      this.products = val;
      console.log(this.products);
    });
  }
}
