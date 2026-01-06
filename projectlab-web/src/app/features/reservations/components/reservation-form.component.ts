import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ReservationService } from '../service/reservation.service';
import { ReservationRequest } from '../model/reservation.model';

@Component({
  selector: 'app-reservation-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './reservation-form.component.html',
  styleUrls: ['./reservation-form.component.scss'],
})
export class ReservationFormComponent {
  @Input() laboratoryId!: string;
  @Input() laboratoryName: string = '';
  @Output() saved = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private reservationService: ReservationService
  ) {
    this.form = this.fb.group({
      userId: ['', Validators.required],
      date: ['', Validators.required],
      startTime: ['', Validators.required],
      endTime: ['', Validators.required],
    });
  }

  onSubmit() {
    if (this.form.valid) {
      const formVal = this.form.value;
      const dateBase = formVal.date;

      const startDateTime = new Date(
        `${dateBase}T${formVal.startTime}:00`
      ).toISOString();
      const endDateTime = new Date(
        `${dateBase}T${formVal.endTime}:00`
      ).toISOString();
      const reservationDate = new Date(`${dateBase}T00:00:00`).toISOString();

      const request: ReservationRequest = {
        userId: formVal.userId,
        laboratoryId: this.laboratoryId,
        reservationDate: reservationDate,
        startTime: startDateTime,
        endTime: endDateTime,
      };

      this.reservationService.create(request).subscribe({
        next: () => {
          alert('Reserva realizada com sucesso!');
          this.saved.emit();
        },
        error: (err) => {
          console.error(err);
          alert('Erro ao reservar: ' + (err.error?.detail || err.message));
        },
      });
    } else {
      this.form.markAllAsTouched();
    }
  }
}
