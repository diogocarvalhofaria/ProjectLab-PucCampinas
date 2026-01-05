import {
  Component,
  EventEmitter,
  OnChanges,
  Output,
  Input,
  SimpleChanges,
} from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { LaboratoryService } from '../../services/laboratory.service';
import { LaboratoryResponse } from '../../models/laboratory.model';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-laboratory-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './laboratory-form.component.html',
})
export class LaboratoryFormComponent implements OnChanges {
  @Output() saved = new EventEmitter<void>();
  @Input() labData: LaboratoryResponse | null = null;

  form: FormGroup;

  constructor(private fb: FormBuilder, private labService: LaboratoryService) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      building: ['', Validators.required],
      capacity: [0, [Validators.required, Validators.min(1)]],
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (this.labData) {
      this.form.patchValue({
        name: this.labData.name,
        building: this.labData.building,
        capacity: this.labData.capacity,
      });
    } else {
      this.form.reset();
    }
  }

  onSubmit() {
    if (this.form.valid) {
      const formData = this.form.value;

      if (this.labData) {
        this.labService.update(this.labData.id, formData).subscribe({
          next: () => {
            alert('Atualizado com sucesso!');
            this.saved.emit();
          },
          error: (err: HttpErrorResponse) => {
            console.error('Erro ao atualizar:', err);
            console.log('Status do erro:', err.status);
            console.log('Mensagem:', err.message);

            alert('Erro ao salvar: ' + err.message);
          },
        });
      } else {
        this.labService.create(formData).subscribe({
          next: () => {
            alert('Criado com sucesso!');
            this.saved.emit();
          },
          error: (err: HttpErrorResponse) => {
            console.error('Erro ao criar:', err);
            alert('Erro ao criar laboratório.');
          },
        });
      }
    }
  }
}
