import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../../service/product.service';
import { ProductCard } from '../../data/interfaces/product/product-card.interface';
import { switchMap } from 'rxjs';
import { ProductCardComponent } from '../../common-ui/product-card/product-card.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-search-page',
  imports: [ProductCardComponent, CommonModule],
  templateUrl: './search-page.component.html',
  styleUrl: './search-page.component.scss',
})
export class SearchPageComponent implements OnInit {
  public search: string = '';
  public products: ProductCard[] = [];

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService
  ) {}

  public ngOnInit(): void {
    this.route.paramMap
      .pipe(
        switchMap((params) => {
          this.search = params.get('search')!;
          return this.productService.searchByName(this.search, 1);
        })
      )
      .subscribe((val) => {
        this.products = val;
        console.log(this.products);
      });
  }
}
