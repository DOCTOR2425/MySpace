import { Component, Input } from '@angular/core';
import { ProductCard } from '../../data/interfaces/productCard.interface';
import { CartService } from '../../service/cart/cart.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-product-card',
  imports: [],
  templateUrl: './product-card.component.html',
  styleUrl: './product-card.component.scss',
})
export class ProductCardComponent {
  @Input() product!: ProductCard;

  constructor(private router: Router, private cartService: CartService) {}

  public viewProduct(id: string): void {
    this.router.navigate([`/product/${id}`]);
  }

  public addToCart(): void {
    const addToCartRequest = {
      productId: this.product.productId,
      quantity: 1,
    };
    this.cartService.addToCart(addToCartRequest);

    const button = document.querySelector('.btn') as HTMLButtonElement;
    button.style.backgroundColor = 'gray';
  }
}
