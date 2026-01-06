import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import {
  LaboratoryService,
  SearchParams,
} from '../../services/laboratory.service';
import { LaboratoryResponse } from '../../models/laboratory.model';
import { LabCardComponent } from '../../../../shared/components/lab-card/lab-card.component';
import { ModalWrapperComponent } from '../../../../shared/components/modal-wrapper/modal-wrapper.component';
import { ReservationFormComponent } from '../../../reservations/components/reservation-form.component';

@Component({
  selector: 'app-laboratory-list',
  standalone: true,
  imports: [
    CommonModule,
    LabCardComponent,
    RouterModule,
    ModalWrapperComponent,
    ReservationFormComponent,
  ],
  templateUrl: './laboratory-list.component.html',
  styleUrl: './laboratory-list.component.scss',
})
export class LaboratoryListComponent implements OnInit {
  laboratories: LaboratoryResponse[] = [];
  loading = true;
  errorMessage = '';

  showReservationModal = false;
  selectedLab: any = null;

  currentPage = 1;
  totalPages = 1;
  hasNext = false;
  hasPrevious = false;

  keyword = '';
  selectedBuilding = '';

  constructor(private readonly laboratoryService: LaboratoryService) {}

  ngOnInit(): void {
    this.fetchLaboratories();
  }

  fetchLaboratories(): void {
    this.loading = true;

    const params: SearchParams = {
      keyword: this.keyword,
      building: this.selectedBuilding,
      page: this.currentPage,
      size: 9,
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
        console.error(err);
        this.errorMessage = 'Erro ao carregar laboratórios.';
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

  openReservation(lab: any) {
    this.selectedLab = lab;
    this.showReservationModal = true;
  }

  closeReservation() {
    this.showReservationModal = false;
    this.selectedLab = null;
  }

  onReservationSaved() {
    this.closeReservation();
  }
}
