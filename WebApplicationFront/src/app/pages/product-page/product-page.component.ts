import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../../service/product.service';
import { CommonModule } from '@angular/common';
import { Product } from '../../data/interfaces/product/product.interface';
import { CartService } from '../../service/cart/cart.service';

@Component({
  selector: 'app-product',
  imports: [CommonModule],
  templateUrl: './product-page.component.html',
  styleUrl: './product-page.component.scss',
})
export class ProductComponent implements OnInit {
  productId!: string;
  product!: Product;
  propertyNames: string[] = [];

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private cartService: CartService
  ) {}

  public ngOnInit(): void {
    this.productId = this.route.snapshot.paramMap.get('id')!;
    this.productService.getProductById(this.productId).subscribe((data) => {
      this.product = data;
      this.propertyNames = Object.keys(data.properties);
    });
  }

  public addToCart(): void {
    const addToCartRequest = {
      productId: this.productId,
      quantity: 1,
    };
    this.cartService.cahngeCart(addToCartRequest);

    const button = document.getElementById("addToCart") as HTMLButtonElement;
    button.style.backgroundColor = 'gray';
  }
}
