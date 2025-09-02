import { Component } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';
import { RouterOutlet, RouterModule } from '@angular/router';

@Component({
  selector: 'app-home-view',
  imports: [MatButtonModule, MatMenuModule, MatIconModule, RouterOutlet, RouterModule],
  templateUrl: './home-view.html',
  styleUrl: './home-view.scss'
})
export class HomeView {
  logout() {
    localStorage.removeItem('auth-token');
    localStorage.removeItem('refresh-token');
    window.location.reload();
  }
}
