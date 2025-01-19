import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  public isAdmin: boolean = false;

  constructor() {}
}
