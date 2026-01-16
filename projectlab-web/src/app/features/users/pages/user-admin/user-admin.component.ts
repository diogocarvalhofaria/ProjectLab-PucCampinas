import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { UserFormComponent } from '../../components/user-form/user-form.component';
import { ModalWrapperComponent } from '../../../../shared/components/modal-wrapper/modal-wrapper.component';
import { ConfirmationModalComponent } from '../../../../shared/components/confirmation-modal/confirmation-modal.component';
import { UserService, SearchParams } from '../../services/user.service';
import { UserResponse } from '../../models/user.model';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../shared/services/toast.service'; // 1. Importar

@Component({
  selector: 'app-user-admin',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
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
  isAdmin = false;

  showModal = false;
  selectedUser: UserResponse | null = null;

  showDeleteModal = false;
  userToDelete: UserResponse | null = null;

  get isEditing(): boolean {
    return !!this.selectedUser;
  }

  keyword = '';
  selectedRole = '';
  currentPage = 1;
  totalPages = 1;
  hasNext = false;
  hasPrevious = false;

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private toast: ToastService
  ) {}

  ngOnInit(): void {
    this.isAdmin = this.authService.isAdmin();
    this.fetchUsers();
  }

  onResendEmail(user: UserResponse) {
    if (
      confirm(
        `Deseja reenviar o e-mail de definição de senha para ${user.name}?`
      )
    ) {
      this.loading = true;

      this.userService.resendEmail(user.id).subscribe({
        next: () => {
          this.loading = false;
          this.toast.success('E-mail reenviado com sucesso!');
        },
        error: (err: HttpErrorResponse) => {
          this.loading = false;
          console.error(err);
          const msg = err.error?.message || err.message;
          this.toast.error(`Erro ao enviar: ${msg}`);
        },
      });
    }
  }

  fetchUsers(): void {
    this.loading = true;

    const params: SearchParams = {
      page: this.currentPage,
      size: 9,
      keyword: this.keyword,
      role: this.selectedRole,
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
        this.toast.error('Erro ao carregar lista de usuários.');
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
    this.currentPage = 1;
    this.fetchUsers();
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

  openDeleteModal(user: UserResponse) {
    this.userToDelete = user;
    this.showDeleteModal = true;
  }

  confirmDelete() {
    if (this.userToDelete) {
      this.userService.delete(this.userToDelete.id).subscribe({
        next: () => {
          this.toast.success('Usuário excluído com sucesso.');
          this.fetchUsers();
          this.closeDeleteModal();
        },
        error: (err: HttpErrorResponse) => {
          console.error('Erro ao deletar', err);

          if (err.status === 403) {
            this.toast.warning('Você não tem permissão para excluir usuários.');
          } else {
            this.toast.error('Não foi possível excluir o usuário.');
          }
        },
      });
    }
  }

  closeDeleteModal() {
    this.showDeleteModal = false;
    this.userToDelete = null;
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
