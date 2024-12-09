import { Component, inject, Pipe } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ProductCardComponent } from "./common-ui/product-card/product-card.component";
import { ProductService } from './data/service/product.service';
import { JsonPipe } from '@angular/common';
import { Product } from './data/interfaces/product.interface';

@Component({
  selector: 'app-root',
  imports: [ProductCardComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  productService = inject(ProductService);
  products:Product[] = []
  constructor()
  {
    this.productService.getTestProduct()
    .subscribe(val => {
      this.products = val
      console.log(this.products)
    })
  }
} 
