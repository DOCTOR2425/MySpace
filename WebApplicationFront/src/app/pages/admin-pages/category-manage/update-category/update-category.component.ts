import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { ProductCategoryCreateRequest } from '../../../../data/interfaces/product-category/product-category-create-request.interface';
import {
  FormArray,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ProductCategoryService } from '../../../../service/product-category/product-category.service';
import { ToastService } from '../../../../service/toast/toast.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-update-category',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './update-category.component.html',
  styleUrl: './update-category.component.scss',
})
export class UpdateCategoryComponent implements OnInit, OnDestroy {
  public categoryId!: string;
  private unsubscribe$ = new Subject<void>();
  public categoryForm: FormGroup;
  public isLoading = true;

  constructor(
    private fb: FormBuilder,
    private productCategoryService: ProductCategoryService,
    private toastService: ToastService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.categoryForm = this.fb.group({
      categoryName: ['', [Validators.required, Validators.minLength(3)]],
      propertyList: this.fb.array([]),
    });
  }

  public ngOnInit(): void {
    this.categoryId = this.route.snapshot.paramMap.get('id')!;

    // this.productCategoryService
    //   .getProductCategoryForAdmin(this.categoryId)
    //   .pipe(takeUntil(this.unsubscribe$))
    //   .subscribe({
    //     next: (category) => {
    //       this.populateForm(category);
    //       this.isLoading = false;
    //     },
    //     error: (error) => {
    //       this.toastService.showError(error.error.error, 'Ошибка');
    //       this.router.navigate(['/admin/categories']);
    //     },
    //   });
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  private populateForm(category: ProductCategoryCreateRequest): void {
    this.categoryForm.patchValue({
      categoryName: category.name,
    });

    if (this.propertyList.length) {
      this.propertyList.removeAt(0);
    }

    Object.entries(category.properties).forEach(([name, isNumeric]) => {
      this.addProperty(name, isNumeric);
    });
  }

  get propertyList(): FormArray {
    return this.categoryForm.get('propertyList') as FormArray;
  }

  private createPropertyGroup(name = '', isNumeric = false) {
    return this.fb.group({
      propertyName: [name, [Validators.required, Validators.minLength(2)]],
      isNumeric: [isNumeric],
    });
  }

  public addProperty(name = '', isNumeric = false): void {
    this.propertyList.push(this.createPropertyGroup(name, isNumeric));
  }

  public removeProperty(index: number): void {
    this.propertyList.removeAt(index);
  }

  public onSubmit(): void {
    if (this.categoryForm.valid) {
      const payload = this.prepareRequest();
      this.updateCategory(payload);
    } else {
      this.categoryForm.markAllAsTouched();
    }
  }

  public exit() {
    console.log('exit');

    this.router.navigate(['/admin/categories']);
  }

  private prepareRequest(): ProductCategoryCreateRequest {
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

  private updateCategory(payload: ProductCategoryCreateRequest): void {
    // this.productCategoryService
    //   .updateCategory(this.categoryId, payload)
    //   .pipe(takeUntil(this.unsubscribe$))
    //   .subscribe({
    //     next: () => {
    //       this.exit();
    //     },
    //     error: (error) => {
    //       this.toastService.showError(error.error.error, 'Ошибка обновления');
    //     },
    //   });
  }
}
