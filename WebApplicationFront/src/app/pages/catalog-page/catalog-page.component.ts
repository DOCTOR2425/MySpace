import { Component } from '@angular/core';
import { ProductCardComponent } from '../../common-ui/product-card/product-card.component';
import { CommonModule } from '@angular/common';
import { ProductService } from '../../service/product.service';
import { ProductData } from '../../data/interfaces/product/productData.interface';

@Component({
  selector: 'app-catalog',
  imports: [ProductCardComponent, CommonModule],
  templateUrl: './catalog-page.component.html',
  styleUrl: './catalog-page.component.scss',
})
export class CatalogComponent {
  products: ProductData[] = [];
  constructor(private productService: ProductService) {
    this.productService.getProductCards().subscribe((val) => {
      this.products = val;
    });
  }
}
