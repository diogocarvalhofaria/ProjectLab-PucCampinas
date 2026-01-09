import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { ReservationResponse } from '../model/reservation.model';
import { ReservationService } from '../service/reservation.service';
import { ConfirmationModalComponent } from '../../../shared/components/confirmation-modal/confirmation-modal.component';

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

  constructor(private reservationService: ReservationService) {}

  ngOnInit(): void {
    this.fetchReservations();
  }

  fetchReservations() {
    this.loading = true;
    const userDataString = localStorage.getItem('user_data');

    if (!userDataString) {
      console.error('Usuário não identificado no LocalStorage.');
      this.loading = false;
      return;
    }

    const userData = JSON.parse(userDataString);
    const userId = userData.id;

    this.reservationService.getMyReservations(userId).subscribe({
      next: (data) => {
        this.reservations = data || [];
        this.loading = false;
      },
      error: (err: HttpErrorResponse) => {
        console.error('Erro ao buscar reservas:', err);
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
          this.fetchReservations();
          this.closeCancelModal();
        },
        error: (err: HttpErrorResponse) => {
          console.error(err);
          const msg = err.error || 'Erro ao cancelar reserva.';
          alert(msg);
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
