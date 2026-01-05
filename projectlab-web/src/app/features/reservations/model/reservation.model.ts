export interface ReservationRequest {
  userId: string;
  laboratoryId: string;
  reservationDate: string;
  startTime: string;
  endTime: string;
}

export interface ReservationResponse {
  id: string;
  reservationDate: string;
  startTime: string;
  endTime: string;
  userId: string;
  userName: string;
  laboratoryId: string;
  laboratoryName: string;
}
