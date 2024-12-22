import { Component } from '@angular/core';
import { ProductCardComponent } from '../../common-ui/product-card/product-card.component';
import { CommonModule } from '@angular/common';
import { Product } from '../../data/interfaces/product.interface';
import { ProductService } from '../../service/product.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-catalog',
  imports: [ProductCardComponent, CommonModule],
  templateUrl: './catalog-page.component.html',
  styleUrl: './catalog-page.component.scss',
})
export class CatalogComponent {
  products: Product[] = [];
  constructor(private router: Router, private productService: ProductService) {
    this.productService.getProductCards().subscribe((val) => {
      this.products = val;
    });
  }

  public viewProduct(id: string): void {
    this.router.navigate([`/product/${id}`]);
  }
}
