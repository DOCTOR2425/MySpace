import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../service/auth/auth.service';
import { AdminService } from '../../service/admin/admin.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-header',
  imports: [RouterModule, CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent implements OnInit {
  public userEmail?: string = undefined;
  constructor(
    public router: Router,
    public authService: AuthService,
    public adminService: AdminService
  ) {}

  ngOnInit(): void {
    this.userEmail = this.authService.userEMail?.slice(0, 3).toUpperCase();
  }
}
