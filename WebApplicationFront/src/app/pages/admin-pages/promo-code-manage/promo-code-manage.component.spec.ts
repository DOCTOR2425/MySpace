import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PromoCodeManageComponent } from './promo-code-manage.component';

describe('PromoCodeManageComponent', () => {
  let component: PromoCodeManageComponent;
  let fixture: ComponentFixture<PromoCodeManageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PromoCodeManageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PromoCodeManageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
