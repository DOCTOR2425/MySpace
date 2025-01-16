import { Routes } from '@angular/router';
import { CatalogComponent } from './pages/catalog-page/catalog-page.component';
import { ProductComponent } from './pages/product-page/product-page.component';
import { LoginPageComponent } from './pages/login-page/login-page.component';
import { LayoutComponent } from './common-ui/layout/layout.component';
import { CartPageComponent } from './pages/cart-page/cart-page.component';
import { CategoryPageComponent } from './pages/category-page/category-page.component';

export const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      { path: '', component: CatalogComponent },
      { path: 'product/:id', component: ProductComponent },
      { path: 'cart', component: CartPageComponent },
      { path: 'category/:categoryName', component: CategoryPageComponent },
    ],
  },
  { path: 'login', component: LoginPageComponent },
];
