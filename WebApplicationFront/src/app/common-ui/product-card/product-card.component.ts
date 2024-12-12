import { Component, Input } from '@angular/core';
import { ProductCard } from '../../data/interfaces/productCard.interface';

@Component({
  selector: 'app-product-card',
  imports: [],
  templateUrl: './product-card.component.html',
  styleUrl: './product-card.component.scss'
})
export class ProductCardComponent {
  @Input() product!: ProductCard
}
