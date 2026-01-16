import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  Output,
  SimpleChanges,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { LaboratoryResponse } from '../../models/laboratory.model';
import { LaboratoryService } from '../../services/laboratory.service';
import { HttpErrorResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ToastService } from '../../../../shared/services/toast.service';

@Component({
  selector: 'app-laboratory-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './laboratory-form.component.html',
  styleUrl: './laboratory-form.component.scss',
})
export class LaboratoryFormComponent implements OnChanges {
  @Output() saved = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();
  @Input() labData: LaboratoryResponse | null = null;

  form: FormGroup;
  isSubmitting = false;

  constructor(
    private fb: FormBuilder,
    private labService: LaboratoryService,
    private toast: ToastService
  ) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      building: ['', Validators.required],
      room: ['', Validators.required],
      capacity: [null, [Validators.required, Validators.min(1)]],
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    this.isSubmitting = false;

    if (this.labData) {
      this.form.patchValue({
        name: this.labData.name,
        building: this.labData.building,
        room: this.labData.room,
        capacity: this.labData.capacity,
      });
    } else {
      this.form.reset();
    }
  }

  onSubmit() {
    if (this.form.valid) {
      this.isSubmitting = true;
      const formData = this.form.value;

      const request$: Observable<any> = this.labData
        ? this.labService.update(this.labData.id, formData)
        : this.labService.create(formData);

      request$.subscribe({
        next: () => {
          this.isSubmitting = false;

          const action = this.labData ? 'atualizado' : 'criado';
          this.toast.success(`Laboratório ${action} com sucesso!`);

          this.saved.emit();
        },
        error: (err: HttpErrorResponse) => {
          console.error('Erro:', err);
          this.isSubmitting = false;

          const msg = err.error?.message || err.message || 'Erro desconhecido';
          this.toast.error(`Erro ao salvar: ${msg}`);
        },
      });
    } else {
      this.form.markAllAsTouched();
      this.toast.warning('Por favor, preencha todos os campos obrigatórios.');
    }
  }

  onCancel() {
    this.cancelled.emit();
  }
}
