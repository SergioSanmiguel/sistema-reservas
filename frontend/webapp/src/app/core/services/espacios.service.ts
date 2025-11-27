import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Espacio {
  id: number;
  nombre: string;
  descripcion: string;
  disponible: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class EspaciosService {
  private apiUrl = 'http://localhost:8080/api/espacios'; 

  constructor(private http: HttpClient) {}

  getAll(): Observable<Espacio[]> {
    return this.http.get<Espacio[]>(this.apiUrl);
  }
}
