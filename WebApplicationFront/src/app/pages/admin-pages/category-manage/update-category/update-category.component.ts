import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { ProductCategoryService } from '../../../../service/product-category/product-category.service';
import { CommonModule } from '@angular/common';
import { ProductCategoryDTOUpdate } from '../../../../data/interfaces/product-category/product-category-update-request.interface';
import { ProductPropertyDTOUpdate } from '../../../../data/interfaces/product-category/product-property-update-request.interface';

@Component({
  imports: [CommonModule, ReactiveFormsModule],
  selector: 'app-update-category',
  templateUrl: './update-category.component.html',
  styleUrls: ['./update-category.component.scss'],
})
export class UpdateCategoryComponent implements OnInit, OnDestroy {
  public categoryId!: string;
  public category!: ProductCategoryDTOUpdate;
  private unsubscribe$ = new Subject<void>();
  public categoryForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private productCategoryService: ProductCategoryService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    this.categoryForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      properties: this.fb.array([]),
    });
  }

  public ngOnInit(): void {
    this.categoryId = this.route.snapshot.paramMap.get('id')!;

    this.productCategoryService
      .getProductCategoryForUpdate(this.categoryId)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (category) => {
          this.category = category;
          this.populateForm(category);
        },
        error: () => {
          this.router.navigate(['/admin/categories']);
        },
      });
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  private populateForm(category: ProductCategoryDTOUpdate): void {
    this.categoryForm.patchValue({
      name: category.name,
    });

    category.properties.forEach((property) => {
      this.addProperty(property);
    });
  }

  get properties(): FormArray {
    return this.categoryForm.get('properties') as FormArray;
  }

  public defaultValueValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      const isRanged = control.get('isRanged')?.value;
      const defaultValue = control.get('defaultValue')?.value;
      const isDefaultValueValid = /^-?\d*(\.\d+)?$/.test(defaultValue);

      if (isRanged && defaultValue && !isDefaultValueValid) {
        return { defaultValueInvalid: true };
      }
      return null;
    };
  }

  private createPropertyGroup(property?: ProductPropertyDTOUpdate): FormGroup {
    const group = this.fb.group(
      {
        productPropertyId: [property?.productPropertyId || null],
        name: [
          property?.name || '',
          [Validators.required, Validators.minLength(2)],
        ],
        isRanged: [property?.isRanged || false],
        defaultValue: [property?.defaultValue || ''],
      },
      { validators: this.defaultValueValidator() }
    );

    group.get('isRanged')?.valueChanges.subscribe((isRanged) => {
      const defaultValueControl = group.get('defaultValue');
      if (isRanged) {
        defaultValueControl?.setValidators([
          Validators.pattern(/^-?\d*(\.\d+)?$/),
        ]);
      } else {
        defaultValueControl?.clearValidators();
      }
      defaultValueControl?.updateValueAndValidity();
    });

    return group;
  }

  public addProperty(property?: ProductPropertyDTOUpdate): void {
    this.properties.push(this.createPropertyGroup(property));
  }

  public removeProperty(index: number): void {
    this.properties.removeAt(index);
  }

  public onSubmit(): void {
    if (this.categoryForm.valid) {
      const payload = this.prepareRequest();
      this.updateCategory(payload);
    } else {
      this.categoryForm.markAllAsTouched();
    }
  }

  private prepareRequest(): ProductCategoryDTOUpdate {
    return {
      name: this.formatString(this.categoryForm.value.name),
      properties: this.categoryForm.value.properties.map((prop: any) => ({
        productPropertyId: prop.productPropertyId,
        name: this.formatString(prop.name),
        isRanged: prop.isRanged,
        defaultValue: prop.defaultValue || undefined,
      })),
    };
  }

  private formatString(input: string): string {
    if (!input) return '';
    input = input.trim();
    return input.charAt(0).toUpperCase() + input.slice(1);
  }

  private updateCategory(payload: ProductCategoryDTOUpdate): void {
    this.productCategoryService
      .updateCategory(this.categoryId, payload)
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: () => {
          this.router.navigate(['/admin/categories']);
        },
        error: () => {
          alert('Ошибка при обновлении категории');
        },
      });
  }

  public exit(): void {
    this.router.navigate(['/admin/categories']);
  }
}
