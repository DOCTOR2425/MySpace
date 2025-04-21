import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductsPendingReviewsComponent } from './products-pending-reviews.component';

describe('ProductsPendingReviewsComponent', () => {
  let component: ProductsPendingReviewsComponent;
  let fixture: ComponentFixture<ProductsPendingReviewsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductsPendingReviewsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProductsPendingReviewsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
