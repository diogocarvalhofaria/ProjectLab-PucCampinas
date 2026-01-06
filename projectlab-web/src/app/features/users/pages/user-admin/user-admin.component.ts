import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http'; // <--- Importante para corrigir o erro 'err implicitly has any type'

// Imports dos Componentes
import { UserCardComponent } from '../../components/user-card/user-card.component';
import { UserFormComponent } from '../../components/user-form/user-form.component';
import { ModalWrapperComponent } from '../../../../shared/components/modal-wrapper/modal-wrapper.component';
import { ConfirmationModalComponent } from '../../../../shared/components/confirmation-modal/confirmation-modal.component';

// Service e Models
import { UserService, SearchParams } from '../../services/user.service';
import { UserResponse } from '../../models/user.model';

@Component({
  selector: 'app-user-admin',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    UserCardComponent,
    UserFormComponent,
    ModalWrapperComponent,
    ConfirmationModalComponent,
  ],
  templateUrl: './user-admin.component.html',
  styleUrl: './user-admin.component.scss',
})
export class UserAdminComponent implements OnInit {
  users: UserResponse[] = [];
  loading = false;

  showForm = false;
  showDeleteModal = false;
  selectedUser: UserResponse | null = null;
  idToDelete: string | null = null;

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
        // <--- Tipagem corrigida
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

  onRoleChange(event: Event) {
    const target = event.target as HTMLSelectElement;
    this.selectedRole = target.value;
    this.currentPage = 1;
    this.fetchUsers();
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

  openNewForm() {
    this.selectedUser = null;
    this.showForm = true;
  }

  openEditForm(user: UserResponse) {
    this.selectedUser = user;
    this.showForm = true;
  }

  closeForm() {
    this.showForm = false;
    this.selectedUser = null;
  }

  onUserSaved() {
    this.closeForm();
    this.fetchUsers();
  }

  openDeleteModal(id: string) {
    this.idToDelete = id;
    this.showDeleteModal = true;
  }

  confirmDelete() {
    if (this.idToDelete) {
      this.userService.delete(this.idToDelete).subscribe({
        next: () => {
          this.fetchUsers();
          this.closeModal();
        },
        error: (err: HttpErrorResponse) =>
          console.error('Erro ao deletar', err), // <--- Tipagem corrigida
      });
    }
  }

  closeModal() {
    this.showDeleteModal = false;
    this.idToDelete = null;
  }
}
