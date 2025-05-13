import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BrandManageComponent } from './brand-manage.component';

describe('BrandManageComponent', () => {
  let component: BrandManageComponent;
  let fixture: ComponentFixture<BrandManageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BrandManageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BrandManageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
