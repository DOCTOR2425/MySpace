import { Component, OnInit } from '@angular/core';
import { Product } from '../../data/interfaces/product.interface';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../../service/product.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-product',
  imports: [CommonModule],
  templateUrl: './product-page.component.html',
  styleUrl: './product-page.component.scss',
})
export class ProductComponent implements OnInit {
  productId!: string;
  product!: Product;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService
  ) {}

  public ngOnInit(): void {
    this.productId = this.route.snapshot.paramMap.get('id')!;
    this.productService
      .getProductById(this.productId)
      .subscribe((data) => (this.product = data));
  }
}
