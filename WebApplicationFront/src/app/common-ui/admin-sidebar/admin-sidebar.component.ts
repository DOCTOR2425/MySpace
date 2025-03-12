import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AdminService } from '../../service/admin/admin.service';
import { CookiesManagerService } from '../../service/cookies-manager/cookies-manager.service';

@Component({
  selector: 'app-admin-sidebar',
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-sidebar.component.html',
  styleUrl: './admin-sidebar.component.scss',
})
export class AdminSidebarComponent {
  constructor(
    private adminService: AdminService,
    private router: Router,
    private cookiesManager: CookiesManagerService
  ) {}

  public exitFromAdminMode(): void {
    this.adminService.isAdmin = false;
    this.cookiesManager.deleteAuthCookie();
    this.router.navigate(['/']);
  }
}
