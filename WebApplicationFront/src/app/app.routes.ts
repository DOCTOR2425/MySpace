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
import { CreateProductComponent } from './pages/admin-pages/catalog-manage/create-product/create-product.component';
import { ReportsPageComponent } from './pages/admin-pages/reports-page/reports-page.component';
import { UpdateProductComponent } from './pages/admin-pages/catalog-manage/update-product/update-product.component';
import { SearchPageComponent } from './pages/search-page/search-page.component';
import { OrderPageComponent } from './pages/admin-pages/orders-page/order-page/order-page.component';
import { OrdersPageComponent } from './pages/admin-pages/orders-page/orders-page.component';
import { ProductComparisonComponent } from './pages/product-comparison/product-comparison.component';
import { CategoryManageComponent } from './pages/admin-pages/category-manage/category-manage.component';
import { CreateCategoryComponent } from './pages/admin-pages/category-manage/create-category/create-category.component';
import { UpdateCategoryComponent } from './pages/admin-pages/category-manage/update-category/update-category.component';

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
      { path: 'search/:search', component: SearchPageComponent },
      { path: 'comparison', component: ProductComparisonComponent },
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
      { path: 'update-product/:id', component: UpdateProductComponent },
      { path: 'reports', component: ReportsPageComponent },
      { path: 'orders', component: OrdersPageComponent },
      { path: 'order/:id', component: OrderPageComponent },
      { path: 'categories', component: CategoryManageComponent },
      { path: 'create-category', component: CreateCategoryComponent },
      { path: 'update-category/:id', component: UpdateCategoryComponent },
    ],
  },
];
