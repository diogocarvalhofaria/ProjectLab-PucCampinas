import { Component, EventEmitter, Output } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
} from '@angular/forms';
import { CommonModule } from '@angular/common';
import { LaboratoryService } from '../../services/laboratory.service';

@Component({
  selector: 'app-laboratory-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './laboratory-form.component.html',
})
export class LaboratoryFormComponent {
  labForm!: FormGroup;

  constructor(private fb: FormBuilder, private labService: LaboratoryService) {
    this.labForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      building: ['', Validators.required],
      capacity: [0, [Validators.required, Validators.min(1)]],
    });
  }

  @Output() saved = new EventEmitter<void>();

  onSubmit() {
    if (this.labForm.valid) {
      this.labService.create(this.labForm.value).subscribe({
        next: () => {
          this.saved.emit();
          this.labForm.reset();
        },
      });
    }
  }

  private initForm() {
    this.labForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      building: ['', Validators.required],
      capacity: [1, [Validators.required, Validators.min(1)]],
    });
  }
}
