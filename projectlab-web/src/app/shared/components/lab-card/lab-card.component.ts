import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LaboratoryResponse } from '../../../features/laboratories/models/laboratory.model';

@Component({
  selector: 'app-lab-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './lab-card.component.html',
  styleUrls: ['./lab-card.component.scss'],
})
export class LabCardComponent {
  @Input() lab!: LaboratoryResponse;
  @Output() deleteRequest = new EventEmitter<string>();
  @Output() editRequest = new EventEmitter<LaboratoryResponse>();
  @Input() isAdmin: boolean = false;

  getCapacityStatus(): string {
    if (this.lab.capacity > 40) return 'Grande';
    if (this.lab.capacity > 20) return 'Médio';
    return 'Pequeno';
  }

  onDelete() {
    this.deleteRequest.emit(this.lab.id);
  }

  onEdit() {
    this.editRequest.emit(this.lab);
  }
}
