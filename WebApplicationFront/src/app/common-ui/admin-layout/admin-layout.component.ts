import { Component } from '@angular/core';
import { AdminSidebarComponent } from "../admin-sidebar/admin-sidebar.component";
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-admin-layout',
  imports: [AdminSidebarComponent, RouterModule],
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.scss'
})
export class AdminLayoutComponent {

}
