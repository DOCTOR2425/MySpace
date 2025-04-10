import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ProductCategoryCreateRequest } from '../../../../data/interfaces/product-category/product-category-create-request.interface';
import { ProductCategoryService } from '../../../../service/product-category/product-category.service';
import { Subject, takeUntil } from 'rxjs';
import { ToastService } from '../../../../service/toast/toast.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-create-category',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './create-category.component.html',
  styleUrl: './create-category.component.scss',
})
export class CreateCategoryComponent implements OnDestroy {
  private unsubscribe$ = new Subject<void>();
  categoryForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private productCategoryService: ProductCategoryService,
    private toastService: ToastService,
    private router: Router
  ) {
    this.categoryForm = this.fb.group({
      categoryName: ['', [Validators.required, Validators.minLength(3)]],
      propertyList: this.fb.array([]),
    });
  }

  get propertyList(): FormArray {
    return this.categoryForm.get('propertyList') as FormArray;
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  createPropertyGroup() {
    return this.fb.group({
      propertyName: ['', [Validators.required, Validators.minLength(2)]],
      isNumeric: [false],
    });
  }

  addProperty(): void {
    this.propertyList.push(this.createPropertyGroup());
  }

  removeProperty(index: number): void {
    this.propertyList.removeAt(index);
  }

  prepareRequest(): ProductCategoryCreateRequest {
    const properties: { [key: string]: boolean } = {};

    this.propertyList.controls.forEach((control) => {
      const propName = control.get('propertyName')?.value;
      const isNumeric = control.get('isNumeric')?.value;
      if (propName) {
        properties[propName] = isNumeric;
      }
    });

    return {
      name: this.categoryForm.get('categoryName')?.value,
      properties,
    };
  }

  onSubmit(): void {
    if (this.categoryForm.valid) {
      const payload = this.prepareRequest();
      this.productCategoryService
        .createCategory(payload)
        .pipe(takeUntil(this.unsubscribe$))
        .subscribe({
          next: () => {
            this.router.navigate(['/admin/categories']);
          },
          error: (error) => {
            this.toastService.showError(error.error.error, 'Ошибка');
          },
        });
    } else {
      this.categoryForm.markAllAsTouched();
    }
  }
}
