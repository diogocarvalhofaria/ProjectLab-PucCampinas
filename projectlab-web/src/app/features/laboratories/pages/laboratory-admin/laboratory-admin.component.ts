import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import {
  LaboratoryService,
  SearchParams,
} from '../../services/laboratory.service';
import { LaboratoryResponse } from '../../models/laboratory.model';
import { LaboratoryFormComponent } from '../../components/laboratory-form/laboratory-form.component';
import { ConfirmationModalComponent } from '../../../../shared/components/confirmation-modal/confirmation-modal.component';
import { ModalWrapperComponent } from '../../../../shared/components/modal-wrapper/modal-wrapper.component';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-laboratory-admin',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    LaboratoryFormComponent,
    ConfirmationModalComponent,
    ModalWrapperComponent,
  ],
  templateUrl: './laboratory-admin.component.html',
  styleUrl: './laboratory-admin.component.scss',
})
export class LaboratoryAdminComponent implements OnInit {
  laboratories: LaboratoryResponse[] = [];
  loading = false;

  showModal = false;
  showDeleteModal = false;
  selectedLab: LaboratoryResponse | null = null;
  idToDelete: string | null = null;
  isAdmin = false;

  currentPage = 1;
  totalPages = 1;
  hasNext = false;
  hasPrevious = false;

  keyword = '';
  selectedBuilding = '';

  constructor(
    private laboratoryService: LaboratoryService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.isAdmin = this.authService.isAdmin();

    this.fetchLaboratories();
  }

  get isEditing(): boolean {
    return !!this.selectedLab;
  }

  fetchLaboratories(): void {
    this.loading = true;

    const params: SearchParams = {
      page: this.currentPage,
      size: 9,
      keyword: this.keyword,
      building: this.selectedBuilding,
    };

    this.laboratoryService.findAll(params).subscribe({
      next: (response) => {
        this.laboratories = response.results;
        this.currentPage = response.currentPage;
        this.totalPages = response.totalPages;
        this.hasNext = response.nextPage;
        this.hasPrevious = response.previousPage;
        this.loading = false;
      },
      error: (err: HttpErrorResponse) => {
        console.error('Erro ao buscar laboratórios', err);
        this.loading = false;
      },
    });
  }

  onSearch(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.keyword = target.value;
    this.currentPage = 1;
    this.fetchLaboratories();
  }

  openModal() {
    this.selectedLab = null;
    this.showModal = true;
  }

  editLab(lab: LaboratoryResponse) {
    this.selectedLab = lab;
    this.showModal = true;
  }

  deleteLab(lab: LaboratoryResponse) {
    this.idToDelete = lab.id;
    this.showDeleteModal = true;
  }

  closeModal() {
    this.showModal = false;
    this.showDeleteModal = false;
    this.selectedLab = null;
    this.idToDelete = null;
  }

  onLabSaved() {
    this.closeModal();
    this.fetchLaboratories();
  }

  confirmDelete() {
    if (this.idToDelete) {
      this.laboratoryService.delete(this.idToDelete).subscribe({
        next: () => {
          this.fetchLaboratories();
          this.closeModal();
        },
        error: (err: HttpErrorResponse) =>
          console.error('Erro ao excluir', err),
      });
    }
  }

  nextPage() {
    if (this.hasNext) {
      this.currentPage++;
      this.fetchLaboratories();
    }
  }

  prevPage() {
    if (this.hasPrevious) {
      this.currentPage--;
      this.fetchLaboratories();
    }
  }
}
