import { Routes } from '@angular/router';
import { CatalogComponent } from './pages/catalog/catalog.component';
import { ProductComponent } from './pages/product/product.component';
import { LoginPageComponent } from './pages/login-page/login-page.component';
import { LayoutComponent } from './common-ui/layout/layout.component';

export const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      { path: '', component: CatalogComponent },
      { path: 'product/:id', component: ProductComponent },
    ],
  },
  { path: 'login', component: LoginPageComponent },
];
