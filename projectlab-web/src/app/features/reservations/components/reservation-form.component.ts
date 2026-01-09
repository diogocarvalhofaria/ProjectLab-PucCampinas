import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ReservationService } from '../service/reservation.service';
import { ReservationRequest } from '../model/reservation.model';
import { AuthService } from '../../../core/services/auth.service';
import { ReservedTime } from '../model/reservation.model';

@Component({
  selector: 'app-reservation-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './reservation-form.component.html',
  styleUrls: ['./reservation-form.component.scss'],
})
export class ReservationFormComponent implements OnInit {
  @Input() laboratoryId!: string;
  @Input() laboratoryName: string = '';
  @Output() saved = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  form: FormGroup;
  loading = false;

  occupiedSlots: ReservedTime[] = [];

  timeOptions: string[] = [];

  constructor(
    private fb: FormBuilder,
    private reservationService: ReservationService,
    private authService: AuthService
  ) {
    this.form = this.fb.group({
      reservationDate: ['', Validators.required],
      startTime: ['', Validators.required],
      endTime: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.generateTimeOptions();

    this.form.get('reservationDate')?.valueChanges.subscribe((date) => {
      if (date && this.laboratoryId) {
        this.fetchOccupiedSlots(date);
      }
    });
  }

  generateTimeOptions() {
    const startHour = 7;
    const endHour = 22;

    for (let h = startHour; h <= endHour; h++) {
      const hourStr = h.toString().padStart(2, '0');
      this.timeOptions.push(`${hourStr}:00`);
      this.timeOptions.push(`${hourStr}:30`);
    }
  }

  fetchOccupiedSlots(date: string) {
    this.loading = true;
    this.reservationService
      .getReservedTimes(this.laboratoryId, date)
      .subscribe({
        next: (slots) => {
          this.occupiedSlots = slots;
          this.loading = false;
        },
        error: (err) => {
          console.error('Erro ao buscar horários', err);
          this.loading = false;
        },
      });
  }

  hasConflict(): boolean {
    const start = this.form.get('startTime')?.value;
    const end = this.form.get('endTime')?.value;

    if (!start || !end || this.occupiedSlots.length === 0) return false;

    return this.occupiedSlots.some((slot) => {
      return start < slot.endTime && end > slot.startTime;
    });
  }

  onCancel() {
    this.cancelled.emit();
  }

  onSubmit() {
    if (this.form.valid) {
      if (this.hasConflict()) {
        alert(
          'Este horário coincide com uma reserva já existente. Por favor, verifique a lista de horários indisponíveis.'
        );
        return;
      }

      const currentUser = this.authService.getCurrentUser();

      if (!currentUser) {
        alert('Sessão expirada. Por favor, faça login novamente.');
        return;
      }

      this.loading = true;
      const formVal = this.form.value;
      const dateBase = formVal.reservationDate;

      const startDateTime = new Date(
        `${dateBase}T${formVal.startTime}:00`
      ).toISOString();
      const endDateTime = new Date(
        `${dateBase}T${formVal.endTime}:00`
      ).toISOString();
      const reservationDate = new Date(`${dateBase}T00:00:00`).toISOString();

      const request: ReservationRequest = {
        userId: currentUser.id,
        laboratoryId: this.laboratoryId,
        reservationDate: reservationDate,
        startTime: startDateTime,
        endTime: endDateTime,
      };

      this.reservationService.create(request).subscribe({
        next: () => {
          this.loading = false;
          alert('Reserva realizada com sucesso!');
          this.saved.emit();
        },
        error: (err) => {
          this.loading = false;
          console.error(err);
          const msg =
            err.error?.detail ||
            err.error?.message ||
            'Erro desconhecido ao reservar.';
          alert('Erro ao reservar: ' + msg);
        },
      });
    } else {
      this.form.markAllAsTouched();
    }
  }
}
