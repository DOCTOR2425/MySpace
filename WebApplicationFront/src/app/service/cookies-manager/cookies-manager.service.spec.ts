import { TestBed } from '@angular/core/testing';

import { CookiesManagerService } from './cookies-manager.service';

describe('CookiesManagerService', () => {
  let service: CookiesManagerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CookiesManagerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
