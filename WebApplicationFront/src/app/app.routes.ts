import { Routes } from '@angular/router';
import { CatalogComponent } from './pages/catalog-page/catalog-page.component';
import { ProductComponent } from './pages/product-page/product-page.component';
import { LoginPageComponent } from './pages/login-page/login-page.component';
import { LayoutComponent } from './common-ui/layout/layout.component';
import { CartPageComponent } from './pages/cart-page/cart-page.component';
import { CategoryPageComponent } from './pages/category-page/category-page.component';
import { AdminMainPageComponent } from './pages/admin-pages/admin-main-page/admin-main-page.component';
import { UserPageComponent } from './pages/user-page/user-page.component';
import { AdminLayoutComponent } from './common-ui/admin-layout/admin-layout.component';
import { CatalogManageComponent } from './pages/admin-pages/catalog-manage/catalog-manage.component';
import { CreateProductComponent } from './pages/admin-pages/create-product/create-product.component';
import { ReportsPageComponent } from './pages/admin-pages/reports-page/reports-page.component';

export const routes: Routes = [
  {
    path: '',
    component: LayoutComponent,
    children: [
      { path: '', component: CatalogComponent },
      { path: 'product/:id', component: ProductComponent },
      { path: 'cart', component: CartPageComponent },
      { path: 'category/:categoryName', component: CategoryPageComponent },
      { path: 'user', component: UserPageComponent },
    ],
  },
  { path: 'login', component: LoginPageComponent },
  {
    path: 'admin',
    component: AdminLayoutComponent,
    children: [
      { path: '', component: AdminMainPageComponent },
      { path: 'catalog', component: CatalogManageComponent },
      { path: 'create-product', component: CreateProductComponent },
      { path: 'reports', component: ReportsPageComponent },
    ],
  },
];
