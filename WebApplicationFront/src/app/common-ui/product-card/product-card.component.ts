import { Component, Input } from '@angular/core';
import { ProductCard } from '../../data/interfaces/productCard.interface';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-product-card',
  imports: [CommonModule],
  templateUrl: './product-card.component.html',
  styleUrl: './product-card.component.scss'
})
export class ProductCardComponent {
  @Input() product!: ProductCard
}
