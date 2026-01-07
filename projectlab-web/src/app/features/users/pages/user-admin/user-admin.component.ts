import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { UserFormComponent } from '../../components/user-form/user-form.component';
import { ModalWrapperComponent } from '../../../../shared/components/modal-wrapper/modal-wrapper.component';
import { UserService, SearchParams } from '../../services/user.service';
import { UserResponse } from '../../models/user.model';

@Component({
  selector: 'app-user-admin',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    UserFormComponent,
    ModalWrapperComponent,
  ],
  templateUrl: './user-admin.component.html',
  styleUrl: './user-admin.component.scss',
})
export class UserAdminComponent implements OnInit {
  users: UserResponse[] = [];
  loading = false;

  showModal = false;
  selectedUser: UserResponse | null = null;

  get isEditing(): boolean {
    return !!this.selectedUser;
  }

  keyword = '';
  selectedRole = '';

  currentPage = 1;
  totalPages = 1;
  hasNext = false;
  hasPrevious = false;

  constructor(private userService: UserService) {}

  ngOnInit(): void {
    this.fetchUsers();
  }

  fetchUsers(): void {
    this.loading = true;

    const params: SearchParams = {
      page: this.currentPage,
      size: 9,
      keyword: this.keyword,
    };

    this.userService.findAll(params).subscribe({
      next: (res) => {
        this.users = res.results;
        this.currentPage = res.currentPage;
        this.totalPages = res.totalPages;
        this.hasNext = res.nextPage;
        this.hasPrevious = res.previousPage;
        this.loading = false;
      },
      error: (err: HttpErrorResponse) => {
        console.error('Erro ao buscar usuários', err);
        this.loading = false;
      },
    });
  }

  onSearch(event: Event) {
    const target = event.target as HTMLInputElement;
    this.keyword = target.value;
    this.currentPage = 1;
    this.fetchUsers();
  }

  onFilterRole(event: Event) {
    const target = event.target as HTMLSelectElement;
    this.selectedRole = target.value;
    if (this.selectedRole) {
    }
  }

  openModal() {
    this.selectedUser = null;
    this.showModal = true;
  }

  editUser(user: UserResponse) {
    this.selectedUser = user;
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
    this.selectedUser = null;
  }

  onUserSaved() {
    this.closeModal();
    this.fetchUsers();
  }

  deleteUser(user: UserResponse) {
    if (confirm(`Tem certeza que deseja excluir ${user.name}?`)) {
      this.userService.delete(user.id).subscribe({
        next: () => {
          this.fetchUsers();
        },
        error: (err) => console.error('Erro ao deletar', err),
      });
    }
  }

  nextPage() {
    if (this.hasNext) {
      this.currentPage++;
      this.fetchUsers();
    }
  }

  prevPage() {
    if (this.hasPrevious) {
      this.currentPage--;
      this.fetchUsers();
    }
  }
}
