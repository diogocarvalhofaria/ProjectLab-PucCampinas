import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';
import { ReservationService } from '../../service/reservation.service';
import { ConfirmationModalComponent } from '../../../../shared/components/confirmation-modal/confirmation-modal.component';

@Component({
  selector: 'app-reservation-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, ConfirmationModalComponent],
  templateUrl: './reservation-admin.component.html',
  styleUrls: ['./reservation-admin.component.scss'],
})
export class ReservationAdminComponent implements OnInit {
  reservations: any[] = [];
  currentPage = 1;
  totalPages = 1;
  totalCount = 0;
  hasNext = false;
  hasPrevious = false;
  searchTerm: string = '';

  showCancelModal = false;
  selectedReservation: any = null;

  private searchSubject = new Subject<string>();

  constructor(private reservationService: ReservationService) {
    this.searchSubject
      .pipe(debounceTime(500), distinctUntilChanged())
      .subscribe((term) => {
        this.searchTerm = term;
        this.loadReservations(1);
      });
  }

  ngOnInit(): void {
    this.loadReservations();
  }

  loadReservations(page: number = 1) {
    this.reservationService
      .searchReservations(this.searchTerm, page, 10)
      .subscribe({
        next: (response) => {
          this.reservations = response.results;
          this.totalCount = response.totalCount;
          this.totalPages = response.totalPages;
          this.currentPage = response.currentPage;
          this.hasNext = response.nextPage;
          this.hasPrevious = response.previousPage;
        },
        error: (err) => console.error('Erro ao carregar relatório', err),
      });
  }

  onSearch(event: any) {
    this.searchSubject.next(event.target.value);
  }

  onDateChange(event: any) {
    const selectedDate = event.target.value;
    console.log('Filtrando por data:', selectedDate);
  }

  openCancelModal(res: any) {
    this.selectedReservation = res;
    this.showCancelModal = true;
  }

  confirmCancel() {
    if (this.selectedReservation) {
      this.reservationService.cancel(this.selectedReservation.id).subscribe({
        next: () => {
          this.loadReservations(this.currentPage);
          this.closeCancelModal();
        },
        error: (err) => console.error('Erro ao cancelar', err),
      });
    }
  }

  closeCancelModal() {
    this.showCancelModal = false;
    this.selectedReservation = null;
  }

  nextPage() {
    if (this.hasNext) this.loadReservations(this.currentPage + 1);
  }
  prevPage() {
    if (this.hasPrevious) this.loadReservations(this.currentPage - 1);
  }
}
