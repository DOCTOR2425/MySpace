import { Component, inject } from '@angular/core';
import { ProductCardComponent } from './common-ui/product-card/product-card.component';
import { ProductCard } from './data/interfaces/productCard.interface';
import { ProductService } from './service/product.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [ProductCardComponent, CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  products: ProductCard[] = [];
  constructor(private productService: ProductService) {
    this.productService.getProductCards().subscribe((val) => {
      this.products = val;
      console.log(this.products);
    });
  }
}
