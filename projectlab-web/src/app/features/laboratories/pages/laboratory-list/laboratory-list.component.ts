import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LaboratoryService } from '../../services/laboratory.service';
import { LaboratoryResponse } from '../../models/laboratory.model';
import { LabCardComponent } from '../../../../shared/components/lab-card/lab-card.component';
import { LaboratoryFormComponent } from '../../components/laboratory-form/laboratory-form.component';

@Component({
  selector: 'app-laboratory-list',
  standalone: true,
  imports: [CommonModule, LabCardComponent, LaboratoryFormComponent],
  templateUrl: './laboratory-list.component.html',
  styleUrl: './laboratory-list.component.scss',
})
export class LaboratoryListComponent implements OnInit {
  laboratories: LaboratoryResponse[] = [];
  showForm = false;
  loading = true;
  errorMessage = '';

  constructor(private readonly laboratoryService: LaboratoryService) {}

  ngOnInit(): void {
    console.log('Componente de lista carregado!');
    this.fetchLaboratories();
  }

  toggleForm() {
    this.showForm = !this.showForm;
  }

  onLabSaved() {
    this.showForm = false; // Fecha o formulário
    this.fetchLaboratories(); // Recarrega a lista para mostrar o novo item
  }

  fetchLaboratories(): void {
    this.loading = true;
    this.laboratoryService.findAll().subscribe({
      next: (response) => {
        this.laboratories = response.results;
        this.loading = false;
      },
      error: (err) => {
        this.errorMessage = 'Erro ao carregar laboratórios.';
        this.loading = false;
      },
    });
  }
}
