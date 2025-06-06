import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductComparisonComponent } from './product-comparison.component';

describe('ProductComparisonComponent', () => {
  let component: ProductComparisonComponent;
  let fixture: ComponentFixture<ProductComparisonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductComparisonComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProductComparisonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
