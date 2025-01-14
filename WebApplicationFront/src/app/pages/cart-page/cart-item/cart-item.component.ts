import { Component, Input } from '@angular/core';
import { CartItem } from '../../../data/interfaces/cart/cart-item.interface';
import { CartPageComponent } from '../cart-page.component';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-cart-item',
  imports: [RouterModule],
  templateUrl: './cart-item.component.html',
  styleUrls: ['./cart-item.component.scss'],
})
export class CartItemComponent {
  @Input() item!: CartItem;
  @Input() cartPage!: CartPageComponent;

  public changeQuantity(isIncrease: boolean) {
    this.cartPage.changeItemQuantity(this.item, isIncrease);
  }

  public removeItem() {
    this.cartPage.removeCartItem(this.item);
  }
}
