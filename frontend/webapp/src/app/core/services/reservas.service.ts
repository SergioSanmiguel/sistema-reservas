import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Reserva {
  id: number;
  fechaInicio: string;
  fechaFin: string;
  estado: string;
  espacioId: number;
  usuarioId: number;
}

@Injectable({
  providedIn: 'root'
})
export class ReservasService {

  private base = '/api/reservas';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Reserva[]> {
    return this.http.get<Reserva[]>(this.base);
  }

  getById(id: number): Observable<Reserva> {
    return this.http.get<Reserva>(`${this.base}/${id}`);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`);
  }
  create(reserva: Partial<Reserva>): Observable<Reserva> {
  return this.http.post<Reserva>(this.base, reserva);
}
}

