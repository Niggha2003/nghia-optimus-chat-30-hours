import { Component } from '@angular/core';
import { UserService } from '../../services/user.service';
@Component({
  selector: 'app-account-view',
  imports: [],
  templateUrl: './account-view.html',
  styleUrl: './account-view.scss'
})
export class AccountView {
  currentUser: any | null = null;

  constructor(private userService: UserService) {
    this.getCurrentUser();
  }

  getCurrentUser() {
    this.userService.getCurrentUser().subscribe({
      next: (data) => {
        this.currentUser = data;
      },
      error: (err) => {
        console.error('Failed to fetch current user', err);
      }
    });
  }
}
