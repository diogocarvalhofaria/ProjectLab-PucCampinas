import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LaboratoryResponse } from '../../../features/laboratories/models/laboratory.model'; // Ajuste o caminho se necessário

@Component({
  selector: 'app-lab-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './lab-card.component.html',
  styleUrls: ['./lab-card.component.scss'],
})
export class LabCardComponent {
  @Input() lab!: LaboratoryResponse;
  @Input() isAdmin: boolean = false;

  @Output() deleteRequest = new EventEmitter<string>();
  @Output() editRequest = new EventEmitter<LaboratoryResponse>();

  getCapacityStatus(): string {
    if (this.lab.capacity > 40) return 'Grande';
    if (this.lab.capacity > 20) return 'Médio';
    return 'Pequeno';
  }

  onDelete(event: Event) {
    event.stopPropagation();
    this.deleteRequest.emit(this.lab.id);
  }

  onEdit(event: Event) {
    event.stopPropagation();
    this.editRequest.emit(this.lab);
  }
}
