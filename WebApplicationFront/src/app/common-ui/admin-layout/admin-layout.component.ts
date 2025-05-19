import { Component, OnInit } from '@angular/core';
import { AdminSidebarComponent } from '../admin-sidebar/admin-sidebar.component';
import { Router, RouterModule } from '@angular/router';
import { AdminService } from '../../service/admin/admin.service';
import { CommonModule } from '@angular/common';
import { CookiesManagerService } from '../../service/cookies-manager/cookies-manager.service';

@Component({
  selector: 'app-admin-layout',
  imports: [AdminSidebarComponent, RouterModule, CommonModule],
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.scss',
})
export class AdminLayoutComponent {
  constructor(
    private adminService: AdminService,
    private cookiesManager: CookiesManagerService,
    private router: Router
  ) {}

  public isAdmin(): boolean {
    return this.adminService.isAdmin(this.cookiesManager.getAuthCookie());
  }

  public exitFromAdminMode(): void {
    this.cookiesManager.deleteAuthCookie();
    this.router.navigate(['/']);
  }
}
