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
import { HttpClient } from '@angular/common/http';

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
    private http: HttpClient,
    private router: Router,
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private adminService: AdminService,
    private productService: ProductService
  ) {}

  private buildForms(productToUpdate: ProductToUpdate): void {
    this.productForm = this.fb.group({
      name: [productToUpdate.name, Validators.required],
      brand: [
        this.optionsForProduct.brands.find(
          (b) => b.name == productToUpdate.brand
        )?.brandId,
        Validators.required,
      ],
      country: [
        this.optionsForProduct.countries.find(
          (c) => c.name == productToUpdate.country
        )?.countryId,
        Validators.required,
      ],
      category: [
        this.optionsForProduct.productCategories.find(
          (c) => c.name == productToUpdate.productCategory
        )?.productCategoryId,
        Validators.required,
      ],
      price: [productToUpdate.price, [Validators.required, Validators.min(0)]],
      quantity: [
        productToUpdate.quantity,
        [Validators.required, Validators.min(0)],
      ],
      description: [productToUpdate.description, Validators.required],
    });
    if (productToUpdate.isArchive) this.productForm.disable();

    this.adminService
      .getProductPropertiesByCategory(this.productForm.value.category)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((val) => {
        this.productProperties = val;
        this.initPropertiesForm();
      });

    const imageRequests = productToUpdate.images.map((image) =>
      this.http.get(`https://localhost:7295/images/${image}`, {
        responseType: 'blob',
      })
    );
    forkJoin(imageRequests).subscribe((blobs) => {
      this.photos = blobs.map(
        (blob, index) =>
          new File([blob], productToUpdate.images[index], { type: blob.type })
      );
      this.photosToView = this.photos.map((file) => this.getPhotoUrl(file));
    });
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
    product.images.forEach((image) => {
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
      const control = this.propertiesForm.get(property.productPropertyId);

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
    const selectedValue = this.productForm.get('category')?.value;
    const selectedOption = this.optionsForProduct.productCategories.find(
      (category) => category.productCategoryId === selectedValue
    );
    const controls: { [key: string]: FormControl } = {};
    if (selectedOption?.name == this.productToUpdate.productCategory) {
      this.productProperties.forEach((property) => {
        controls[property.productPropertyId] = this.fb.control(
          this.productToUpdate.productPropertyValues.find(
            (prop) => prop.propertyId == property.productPropertyId
          )?.value,
          Validators.required
        );
      });
    } else {
      this.productProperties.forEach((property) => {
        controls[property.productPropertyId] = this.fb.control(
          '',
          Validators.required
        );
      });
    }

    this.propertiesForm = this.fb.group(controls);
    if (this.productToUpdate.isArchive) this.propertiesForm.disable();
  }

  public onPhotosChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const files = Array.from(input.files).slice(0, 4 - this.photos.length);
      this.photos.push(...files);
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

  public changeArchiveStatus(status: boolean): void {
    this.productService
      .changeArchiveStatus(this.productToUpdateId, status)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {
          this.productToUpdate.isArchive = status;

          if (this.productToUpdate.isArchive) {
            this.productForm.disable();
            this.propertiesForm?.disable();
          } else {
            this.productForm.enable();
            this.propertiesForm?.enable();
          }
        },
      });
  }
}
