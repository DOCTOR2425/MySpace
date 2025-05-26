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
import { FullProductInfoResponse } from '../../../../data/interfaces/product/product-to-update-response.interface';
import { OptionsForProduct } from '../../../../data/interfaces/some/options-for-order.interface';
import { forkJoin, Subject, takeUntil } from 'rxjs';
import { CreateProductRequest } from '../../../../data/interfaces/product/create-product-request.interface';
import { ToastService } from '../../../../service/toast/toast.service';

@Component({
  selector: 'app-update-product',
  imports: [CommonModule, FormsModule, RouterModule, ReactiveFormsModule],
  templateUrl: './update-product.component.html',
  styleUrl: './update-product.component.scss',
})
export class UpdateProductComponent implements OnInit, OnDestroy {
  public productToUpdateId!: string;
  public FullProductInfoResponse!: FullProductInfoResponse;
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
    private productService: ProductService,
    private toastService: ToastService
  ) {}

  selectedImage: string = '';

  openImageModal(imageUrl: string) {
    this.selectedImage = imageUrl;
    // const modal = new bootstrap.Modal(document.getElementById('imageModal'));
    // modal.show();
  }

  public ngOnInit(): void {
    this.productToUpdateId = this.route.snapshot.paramMap.get('id')!;

    forkJoin({
      FullProductInfoResponse: this.productService.getProductById(
        this.productToUpdateId
      ),
      productOptions: this.adminService.getOptionsForProduct(),
    })
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((val) => {
        this.FullProductInfoResponse = val.FullProductInfoResponse;
        this.optionsForProduct = val.productOptions;
        this.buildForms(val.FullProductInfoResponse);
      });
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();

    this.photos.forEach((photo) => {
      URL.revokeObjectURL(this.getPhotoUrl(photo));
    });
  }

  private buildForms(FullProductInfoResponse: FullProductInfoResponse): void {
    this.productForm = this.fb.group({
      name: [FullProductInfoResponse.name, Validators.required],
      brand: [
        this.optionsForProduct.brands.find(
          (b) => b.name == FullProductInfoResponse.brand
        )?.brandId,
        Validators.required,
      ],
      country: [
        this.optionsForProduct.countries.find(
          (c) => c.name == FullProductInfoResponse.country
        )?.countryId,
        Validators.required,
      ],
      category: [
        this.optionsForProduct.productCategories.find(
          (c) => c.name == FullProductInfoResponse.productCategory
        )?.productCategoryId,
        Validators.required,
      ],
      price: [
        FullProductInfoResponse.price,
        [Validators.required, Validators.min(0)],
      ],
      quantity: [
        FullProductInfoResponse.quantity,
        [Validators.required, Validators.min(0)],
      ],
      description: [FullProductInfoResponse.description, Validators.required],
    });
    if (FullProductInfoResponse.isArchive) this.productForm.disable();

    this.adminService
      .getProductPropertiesByCategory(this.productForm.value.category)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((val) => {
        this.productProperties = val;
        this.initPropertiesForm();
      });

    const imageRequests = FullProductInfoResponse.images.map((image) => {
      const name = image.replace('https://localhost:7295/images/', '');
      return this.adminService.uploadImage(name);
    });
    forkJoin(imageRequests)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((blobs) => {
        this.photos = blobs.map((blob, index) => {
          return new File(
            [blob],
            FullProductInfoResponse.images[index].replace(
              'https://localhost:7295/images/',
              ''
            ),
            { type: blob.type }
          );
        });
        this.photosToView = this.photos.map((file) => this.getPhotoUrl(file));
      });
  }

  public onSubmit(): void {
    this.productForm.markAllAsTouched();
    this.propertiesForm.markAllAsTouched();
    if (this.photos.length == 0)
      this.toastService.showInfo(
        'Нужно добавить для товара хотя бы одну картинку',
        'Добавьте картинку'
      );

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
    if (selectedOption?.name == this.FullProductInfoResponse.productCategory) {
      this.productProperties.forEach((property) => {
        controls[property.productPropertyId] = this.fb.control(
          this.FullProductInfoResponse.productPropertyValues.find(
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
    if (this.FullProductInfoResponse.isArchive) this.propertiesForm.disable();
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

  public exit(): void {
    this.router.navigate(['/admin/catalog']);
  }

  public changeArchiveStatus(status: boolean): void {
    this.productService
      .changeArchiveStatus(this.productToUpdateId, status)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {
          this.FullProductInfoResponse.isArchive = status;

          if (this.FullProductInfoResponse.isArchive) {
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
