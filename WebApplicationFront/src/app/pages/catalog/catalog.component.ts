import { Component } from '@angular/core';
import { ProductCardComponent } from '../../common-ui/product-card/product-card.component';
import { CommonModule } from '@angular/common';
import { ProductCard } from '../../data/interfaces/productCard.interface';
import { ProductService } from '../../service/product.service';

@Component({
  selector: 'app-catalog',
  imports: [ProductCardComponent, CommonModule],
  templateUrl: './catalog.component.html',
  styleUrl: './catalog.component.scss',
})
export class CatalogComponent {
  products: ProductCard[] = [];
  constructor(private productService: ProductService) {
    this.productService.getProductCards().subscribe((val) => {
      this.products = val;
      console.log(this.products);
    });
  }
}
