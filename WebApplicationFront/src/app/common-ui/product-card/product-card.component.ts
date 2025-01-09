import { Component, Input } from '@angular/core';
import { ProductCard } from '../../data/interfaces/product/productCard.interface';
import { CartService } from '../../service/cart/cart.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-product-card',
  imports: [RouterModule],
  templateUrl: './product-card.component.html',
  styleUrl: './product-card.component.scss',
})
export class ProductCardComponent {
  @Input() product!: ProductCard;

  constructor(private cartService: CartService) {}

  public addToCart(): void {
    const addToCartRequest = {
      productId: this.product.productId,
      quantity: 1,
    };
    this.cartService.cahngeCart(addToCartRequest);

    const button = document.getElementById(this.product.productId) as HTMLButtonElement;
    button.style.backgroundColor = 'gray';
  }
}
