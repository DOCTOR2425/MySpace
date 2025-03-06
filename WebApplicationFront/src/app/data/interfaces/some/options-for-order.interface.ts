import { Brand } from './brand.interface';
import { Country } from './country.interface';
import { ProductCategory } from './product-category.interface';

export interface OptionsForProduct {
  brands: Brand[];
  countries: Country[];
  productCategories: ProductCategory[];
}
