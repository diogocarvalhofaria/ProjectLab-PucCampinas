import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LaboratoryResponse } from '../../../features/laboratories/models/laboratory.model';

@Component({
  selector: 'app-lab-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './lab-card.component.html',
  styleUrl: './lab-card.component.scss'
})
export class LabCardComponent {
  @Input() lab!: LaboratoryResponse; 

  getCapacityStatus(): string {
    if (this.lab.capacity > 40) return 'Grande';
    if (this.lab.capacity > 20) return 'Médio';
    return 'Pequeno';
  }
}