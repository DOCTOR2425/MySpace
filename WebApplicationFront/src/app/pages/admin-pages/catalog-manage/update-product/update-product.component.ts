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
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AdminService } from '../../../../service/admin/admin.service';
import { ProductProperty } from '../../../../data/interfaces/product/product-property.interface';
import { ProductService } from '../../../../service/product.service';
import { ProductToUpdate } from '../../../../data/interfaces/product/product-to-update-response.interface';
import { OptionsForProduct } from '../../../../data/interfaces/some/options-for-order.interface';
import { forkJoin, Subject, takeUntil } from 'rxjs';
import { CreateProductRequest } from '../../../../data/interfaces/product/create-product-request.interface';

@Component({
  selector: 'app-update-product',
  imports: [CommonModule, FormsModule, RouterModule, ReactiveFormsModule],
  templateUrl: './update-product.component.html',
  styleUrl: './update-product.component.scss',
})
export class UpdateProductComponent implements OnInit, OnDestroy {
  public productToUpdateId!: string;
  public productToUpdate!: ProductToUpdate;
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
    private route: ActivatedRoute,
    private adminService: AdminService,
    private productService: ProductService
  ) {}

  private buildForms(productToUpdate: ProductToUpdate): void {
    this.productForm = this.fb.group({
      name: [productToUpdate.name, Validators.required],
      category: [productToUpdate.productCategory, Validators.required],
      brand: [productToUpdate.brand, Validators.required],
      country: [productToUpdate.country, Validators.required],
      price: [productToUpdate.price, [Validators.required, Validators.min(0)]],
      quantity: [
        productToUpdate.quantity,
        [Validators.required, Validators.min(0)],
      ],
      description: [productToUpdate.description, Validators.required],
      photos: [[]],
    });

    this.propertiesForm = this.fb.group({});
  }

  public ngOnInit(): void {
    this.productToUpdateId = this.route.snapshot.paramMap.get('id')!;

    forkJoin({
      productToUpdate: this.productService.getProductToUpdate(
        this.productToUpdateId
      ),
      productOptions: this.adminService.getOptionsForProduct(),
    })
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((val) => {
        this.productToUpdate = val.productToUpdate;
        this.optionsForProduct = val.productOptions;
        this.buildForms(val.productToUpdate);
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
    product.images.forEach((image, index) => {
      formData.append('images', image, image.name);
    });

    this.productService
      .updateProduct(this.productToUpdateId, formData)
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
