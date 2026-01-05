import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserResponse } from '../../models/user.model';

@Component({
  selector: 'app-user-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './user-card.component.html',
})
export class UserCardComponent {
  @Input() user!: UserResponse;
  @Input() isAdmin: boolean = false;

  @Output() editRequest = new EventEmitter<UserResponse>();
  @Output() deleteRequest = new EventEmitter<string>();

  onEdit() {
    this.editRequest.emit(this.user);
  }

  onDelete() {
    this.deleteRequest.emit(this.user.id);
  }

  getRoleColor(role: string): string {
    switch (role) {
      case 'Admin':
        return '#c0392b';
      case 'Professor':
        return '#f39c12';
      default:
        return '#004a80';
    }
  }
}
