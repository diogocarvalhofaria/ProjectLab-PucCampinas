import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';

import {
  LaboratoryService,
  SearchParams, // <--- Importante
} from '../../services/laboratory.service';
import { LaboratoryResponse } from '../../models/laboratory.model';
import { LabCardComponent } from '../../../../shared/components/lab-card/lab-card.component';
import { LaboratoryFormComponent } from '../../components/laboratory-form/laboratory-form.component';
import { ConfirmationModalComponent } from '../../../../shared/components/confirmation-modal/confirmation-modal.component';
import { ModalWrapperComponent } from '../../../../shared/components/modal-wrapper/modal-wrapper.component';

@Component({
  selector: 'app-laboratory-admin',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    LabCardComponent,
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

  showForm = false;
  selectedLab: LaboratoryResponse | null = null;
  showDeleteModal = false;
  idToDelete: string | null = null;

  currentPage = 1;
  totalPages = 1;
  hasNext = false;
  hasPrevious = false;

  keyword = '';
  selectedBuilding = '';

  constructor(private laboratoryService: LaboratoryService) {}

  ngOnInit(): void {
    this.fetchLaboratories();
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

  onBuildingChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    this.selectedBuilding = target.value;
    this.currentPage = 1;
    this.fetchLaboratories();
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

  openNewForm() {
    this.selectedLab = null;
    this.showForm = true;
  }

  openEditForm(lab: LaboratoryResponse) {
    this.selectedLab = lab;
    this.showForm = true;
  }

  closeForm() {
    this.showForm = false;
    this.selectedLab = null;
  }

  toggleForm() {
    this.showForm = !this.showForm;
  }

  onLabSaved() {
    this.showForm = false;
    this.fetchLaboratories();
  }

  openDeleteModal(id: string) {
    this.idToDelete = id;
    this.showDeleteModal = true;
  }

  confirmDelete() {
    if (this.idToDelete) {
      this.laboratoryService.delete(this.idToDelete).subscribe({
        next: () => {
          this.fetchLaboratories(); // A lista atualiza mantendo filtros atuais
          this.closeModal();
        },
        error: (err: HttpErrorResponse) =>
          console.error('Erro ao excluir', err),
      });
    }
  }

  closeModal() {
    this.showDeleteModal = false;
    this.idToDelete = null;
  }
}
