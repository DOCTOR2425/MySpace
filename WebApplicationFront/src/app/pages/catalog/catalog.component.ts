import { Component } from '@angular/core';
import { ProductCardComponent } from '../../common-ui/product-card/product-card.component';
import { CommonModule } from '@angular/common';
import { ProductCard } from '../../data/interfaces/productCard.interface';
import { ProductService } from '../../service/product.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-catalog',
  imports: [ProductCardComponent, CommonModule],
  templateUrl: './catalog.component.html',
  styleUrl: './catalog.component.scss',
})
export class CatalogComponent {
  products: ProductCard[] = [];
  constructor(private productService: ProductService, private router: Router) {
    this.productService.getProductCards().subscribe((val) => {
      this.products = val;
    });
  }

  public viewProduct(productId: string) {
    this.router.navigate([`/product/${productId}`]);
  }
}
