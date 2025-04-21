import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AdminService } from '../../../../service/admin/admin.service';
import { ProductProperty } from '../../../../data/interfaces/product/product-property.interface';
import { ProductService } from '../../../../service/product.service';
import { OptionsForProduct } from '../../../../data/interfaces/some/options-for-order.interface';
import { CreateProductRequest } from '../../../../data/interfaces/product/create-product-request.interface';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-create-product',
  imports: [CommonModule, FormsModule, RouterModule, ReactiveFormsModule],
  templateUrl: './create-product.component.html',
  styleUrl: './create-product.component.scss',
})
export class CreateProductComponent implements OnInit, OnDestroy {
  public productProperties: ProductProperty[] = [];
  public optionsForProduct!: OptionsForProduct;
  public photos: File[] = [];
  public photosToView: string[] = [];
  public productForm!: FormGroup;
  public propertiesForm!: FormGroup;
  private unsubscribe$ = new Subject<void>();

  constructor(
    private router: Router,
    private fb: FormBuilder,
    private adminService: AdminService,
    private productService: ProductService
  ) {
    this.productForm = fb.group({
      name: ['', Validators.required],
      category: ['', Validators.required],
      brand: ['', Validators.required],
      country: ['', Validators.required],
      price: ['', [Validators.required, Validators.min(0)]],
      quantity: ['', [Validators.required, Validators.min(0)]],
      description: ['', Validators.required],
      photos: [[]],
    });

    this.propertiesForm = fb.group({});
  }

  public ngOnInit(): void {
    this.adminService
      .getOptionsForProduct()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((opt) => {
        this.optionsForProduct = opt;
      });
  }

  public onSubmit(): void {
    this.productForm.markAllAsTouched();
    this.propertiesForm.markAllAsTouched();
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

    const formData = new FormData();
    formData.append('productDto', JSON.stringify(product));
    product.images.forEach((image) => {
      formData.append('images', image, image.name);
    });

    this.productService
      .createProduct(formData)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {
          this.exit();
        },
        error: () => {},
      });
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
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((val) => {
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
    }
  }

  public onPhotoDelete(index: number): void {
    this.photos.splice(index, 1);
    this.photosToView.splice(index, 1);
    this.productForm.patchValue({ photos: this.photos });
  }

  public getPhotoUrl(file: File): string {
    return URL.createObjectURL(file);
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();

    this.photos.forEach((photo) => {
      URL.revokeObjectURL(this.getPhotoUrl(photo));
    });
  }

  public exit(): void {
    this.router.navigate(['/admin/catalog']);
  }
}
