import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CatalogManageComponent } from './catalog-manage.component';

describe('CatalogManageComponent', () => {
  let component: CatalogManageComponent;
  let fixture: ComponentFixture<CatalogManageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CatalogManageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CatalogManageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
