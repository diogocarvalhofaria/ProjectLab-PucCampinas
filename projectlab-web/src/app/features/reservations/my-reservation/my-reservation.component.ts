import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { ReservationResponse } from '../model/reservation.model';
import { ReservationService } from '../service/reservation.service';
import { ConfirmationModalComponent } from '../../../shared/components/confirmation-modal/confirmation-modal.component';
import { ToastService } from '../../../shared/services/toast.service';

@Component({
  selector: 'app-my-reservation',
  standalone: true,
  imports: [CommonModule, RouterModule, ConfirmationModalComponent],
  templateUrl: './my-reservation.component.html',
  styleUrls: ['./my-reservation.component.scss'],
})
export class MyReservationComponent implements OnInit {
  reservations: ReservationResponse[] = [];
  loading = true;

  showCancelModal = false;
  selectedReservation: ReservationResponse | null = null;

  constructor(
    private reservationService: ReservationService,
    private toast: ToastService
  ) {}

  ngOnInit(): void {
    this.fetchReservations();
  }

  fetchReservations() {
    this.loading = true;

    this.reservationService.getMyReservations().subscribe({
      next: (data) => {
        this.reservations = data || [];
        this.loading = false;
      },
      error: (err: HttpErrorResponse) => {
        console.error('Erro ao buscar reservas:', err);
        this.toast.error('Não foi possível carregar suas reservas.');
        this.reservations = [];
        this.loading = false;
      },
    });
  }

  openCancelModal(reservation: ReservationResponse) {
    this.selectedReservation = reservation;
    this.showCancelModal = true;
  }

  closeCancelModal() {
    this.showCancelModal = false;
    this.selectedReservation = null;
  }

  confirmCancel() {
    if (this.selectedReservation) {
      this.reservationService.cancel(this.selectedReservation.id).subscribe({
        next: () => {
          this.toast.success('Reserva cancelada com sucesso.');

          this.fetchReservations();
          this.closeCancelModal();
        },
        error: (err: HttpErrorResponse) => {
          console.error(err);

          let msg = 'Erro ao cancelar reserva.';

          if (err.status === 403) {
            msg = 'Você não tem permissão para cancelar esta reserva.';
          } else if (err.error && typeof err.error === 'string') {
            msg = err.error;
          } else if (err.error?.message) {
            msg = err.error.message;
          }

          this.toast.error(msg);
          this.closeCancelModal();
        },
      });
    }
  }

  isPast(dateStr: string): boolean {
    const reservationDate = new Date(dateStr);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    return reservationDate < today;
  }
}
