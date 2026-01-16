import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { ConfirmationModalComponent } from '../../../shared/components/confirmation-modal/confirmation-modal.component';
import { ReservationService } from '../service/reservation.service';
import { AuthService } from '../../../core/services/auth.service';
import { ToastService } from '../../../shared/services/toast.service';

@Component({
  selector: 'app-reservation-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ConfirmationModalComponent],
  templateUrl: './reservation-form.component.html',
  styleUrls: ['./reservation-form.component.scss'],
})
export class ReservationFormComponent implements OnInit {
  @Input() laboratoryId: string = '';
  @Input() laboratoryName: string = '';
  @Output() saved = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  form: FormGroup;
  loading = false;

  occupiedSlots: any[] = [];

  timeOptions: string[] = [
    '08:00',
    '09:00',
    '10:00',
    '11:00',
    '12:00',
    '13:00',
    '14:00',
    '15:00',
    '16:00',
    '17:00',
    '18:00',
    '19:00',
    '20:00',
    '21:00',
    '22:00',
    '23:00',
  ];

  showConfirmationModal = false;
  confirmationMessage = '';

  constructor(
    private fb: FormBuilder,
    private reservationService: ReservationService,
    private authService: AuthService,
    private toast: ToastService
  ) {
    this.form = this.fb.group({
      reservationDate: ['', Validators.required],
      startTime: ['', Validators.required],
      endTime: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.form.get('reservationDate')?.valueChanges.subscribe((dateValue) => {
      if (dateValue && this.laboratoryId) {
        this.loadOccupiedSlots(dateValue);
      } else {
        this.occupiedSlots = [];
      }
    });
  }

  loadOccupiedSlots(dateVal: string | Date) {
    let dateStr = '';
    if (dateVal instanceof Date) {
      dateStr = dateVal.toISOString().split('T')[0];
    } else {
      dateStr = dateVal;
    }

    this.reservationService
      .getReservedTimes(this.laboratoryId, dateStr)
      .subscribe({
        next: (data) => {
          this.occupiedSlots = data.map((slot: any) => ({
            startTime: this.formatTime(slot.startTime || slot.StartTime),
            endTime: this.formatTime(slot.endTime || slot.EndTime),
          }));
        },
        error: (err) => {
          console.error('Erro ao buscar horários ocupados', err);
          this.toast.error('Não foi possível carregar a agenda.');
        },
      });
  }

  formatTime(value: string): string {
    if (!value) return '';
    if (value.includes(':') && !value.includes('-') && !value.includes('/')) {
      return value.substring(0, 5);
    }
    const date = new Date(value);
    if (!isNaN(date.getTime())) {
      return date.toLocaleTimeString('pt-BR', {
        hour: '2-digit',
        minute: '2-digit',
      });
    }
    if (value.includes(' ')) {
      const parts = value.split(' ');
      if (parts.length > 1) {
        return parts[1].substring(0, 5);
      }
    }
    return value.substring(0, 5);
  }

  onSubmit() {
    if (this.form.valid) {
      const date = this.form.get('reservationDate')?.value;
      const start = this.form.get('startTime')?.value;
      const end = this.form.get('endTime')?.value;

      if (start >= end) {
        // Alerta substituído por Toast Warning
        this.toast.warning(
          'O horário de início deve ser menor que o horário de fim.'
        );
        return;
      }

      const isConflict = this.occupiedSlots.some(
        (slot) =>
          (start >= slot.startTime && start < slot.endTime) ||
          (end > slot.startTime && end <= slot.endTime) ||
          (start <= slot.startTime && end >= slot.endTime)
      );

      if (isConflict) {
        this.toast.error('Este horário já está reservado. Verifique a lista.');
        return;
      }

      const dateObj = new Date(date);
      const dateStr = dateObj.toLocaleDateString('pt-BR', { timeZone: 'UTC' });

      this.confirmationMessage = `Confirma a reserva no <strong>${this.laboratoryName}</strong> para o dia <strong>${dateStr}</strong> das <strong>${start}</strong> às <strong>${end}</strong>?`;

      this.showConfirmationModal = true;
    } else {
      this.form.markAllAsTouched();
      this.toast.warning('Por favor, preencha todos os campos.');
    }
  }

  confirmSave() {
    this.showConfirmationModal = false;
    this.loading = true;

    const userId = this.authService.getUserIdFromToken();

    if (!userId) {
      this.toast.error('Sessão inválida. Faça login novamente.');
      this.loading = false;
      return;
    }

    const rawDate = this.form.get('reservationDate')?.value;
    let rawStart = this.form.get('startTime')?.value;
    let rawEnd = this.form.get('endTime')?.value;

    let dateStr = rawDate;
    if (rawDate instanceof Date) {
      dateStr = rawDate.toISOString().split('T')[0];
    }

    const fullStartTime = `${dateStr}T${rawStart}:00`;
    const fullEndTime = `${dateStr}T${rawEnd}:00`;

    const reservationData = {
      userId: userId,
      laboratoryId: this.laboratoryId,
      reservationDate: dateStr,
      startTime: fullStartTime,
      endTime: fullEndTime,
    };

    this.reservationService.create(reservationData).subscribe({
      next: () => {
        this.loading = false;
        // Sucesso!
        this.toast.success('Reserva realizada com sucesso!');
        this.saved.emit();
      },
      error: (err: HttpErrorResponse) => {
        this.loading = false;
        console.error('Erro Backend:', err);

        let msg = 'Erro ao reservar.';
        if (err.error?.errors) {
          const firstErrorKey = Object.keys(err.error.errors)[0];
          msg += ` (${err.error.errors[firstErrorKey][0]})`;
        } else if (err.error?.message) {
          msg = err.error.message;
        }

        this.toast.error(msg);
      },
    });
  }

  closeConfirmationModal() {
    this.showConfirmationModal = false;
  }

  onCancel() {
    this.cancelled.emit();
  }
}
