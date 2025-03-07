import { CommonModule } from '@angular/common';
import {
  ChangeDetectorRef,
  Component,
  Input,
  OnDestroy,
  OnInit,
} from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AdminService } from '../../../service/admin/admin.service';
import { ProductProperty } from '../../../data/interfaces/product/product-property.interface';
import { ProductService } from '../../../service/product.service';
import { ProductToUpdateResponse } from '../../../data/interfaces/product/product-toupdate-response.interface';
import { OptionsForProduct } from '../../../data/interfaces/some/options-for-order.interface';
import { CreateProductRequest } from '../../../data/interfaces/product/create-product-request.interface';

@Component({
  selector: 'app-create-product',
  imports: [CommonModule, FormsModule, RouterModule, ReactiveFormsModule],
  templateUrl: './create-product.component.html',
  styleUrl: './create-product.component.scss',
})
export class CreateProductComponent implements OnInit, OnDestroy {
  @Input() productToUpdateId: string = '';
  public productToUpdate: ProductToUpdateResponse | null = null;
  public productProperties: ProductProperty[] = [];
  public optionsForProduct!: OptionsForProduct;
  public photos: File[] = [];
  public photosToView: string[] = [];
  public productForm!: FormGroup;
  public propertiesForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private adminService: AdminService,
    private productService: ProductService,
    private cdr: ChangeDetectorRef
  ) {
    this.productForm = fb.group({
      name: [this.productToUpdate?.name || '', Validators.required],
      category: [
        this.productToUpdate?.productCategory || '',
        Validators.required,
      ],
      brand: [this.productToUpdate?.brand || '', Validators.required],
      country: [this.productToUpdate?.country || '', Validators.required],
      price: [
        this.productToUpdate?.price || '',
        [Validators.required, Validators.min(0)],
      ],
      quantity: [
        this.productToUpdate?.quantity || '',
        [Validators.required, Validators.min(0)],
      ],
      description: [
        this.productToUpdate?.description || '',
        Validators.required,
      ],
      photos: [[]],
    });

    this.propertiesForm = fb.group({});
  }

  public ngOnInit(): void {
    this.adminService.getOptionsForProduct().subscribe((opt) => {
      this.optionsForProduct = opt;
    });
  }

  public onSubmit(): void {
    this.productForm.markAllAsTouched();
    this.propertiesForm.markAllAsTouched();
    if (this.productToUpdateId) {
      console.log('Редактирование товара:', this.productToUpdate); //Нужно собрать товар и на бэке его принять
    } else {
      let product: CreateProductRequest = {
        name: this.productForm.value.name,
        description: this.productForm.value.description,
        price: this.productForm.value.price,
        quantity: this.productForm.value.quantity,
        images: this.photos,
        propertyValues: this.getPropertyValues(),
        productCategoryId: this.productForm.value.category,
        brandId: this.productForm.value.brand,
        countryId: this.productForm.value.country,
      };

      console.log('Добавление товара:', product);
      this.productService.createProduct(product);
    }
  }

  public getPropertyValues(): { [key: string]: string } {
    const propertyValues: { [key: string]: string } = {};

    this.productProperties.forEach((property) => {
      const control = this.propertiesForm.get(property.name);
      if (control) {
        propertyValues[property.productPropertyId] = control.value;
      }
    });

    return propertyValues;
  }

  public getCategoryProperty(event: Event) {
    const selectElement = event.target as HTMLSelectElement;
    const categoryId = selectElement?.value;

    this.adminService
      .getProductPropertiesByCategory(categoryId)
      .subscribe((val) => {
        console.log(val);

        this.productProperties = val;
        this.initPropertiesForm();
      });
  }

  private initPropertiesForm() {
    const controls: { [key: string]: FormControl } = {};

    this.productProperties.forEach((property) => {
      controls[property.name] = this.fb.control('', Validators.required);
    });

    this.propertiesForm = this.fb.group(controls);
  }

  public onPhotosChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const files = Array.from(input.files).slice(0, 4 - this.photos.length);
      this.photos.push(...files);
      this.productForm.patchValue({ photos: this.photos });
      this.photosToView = [];
      for (let file of this.photos) {
        this.photosToView.push(this.getPhotoUrl(file));
      }
      console.log(this.photosToView);
    }
  }

  public onPhotoDelete(index: number): void {
    this.photos.splice(index, 1);
    this.photosToView.splice(index, 1);
    this.productForm.patchValue({ photos: this.photos });
    // this.cdr.detectChanges();
  }

  public getPhotoUrl(file: File): string {
    return URL.createObjectURL(file);
  }

  public ngOnDestroy(): void {
    // Освобождаем память, занятую blob: URL
    this.photos.forEach((photo) => {
      URL.revokeObjectURL(this.getPhotoUrl(photo));
    });
  }
}
