import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { ReservasService, Reserva } from '../../../core/services/reservas.service';

@Component({
  selector: 'app-reservas-listar',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule],
  templateUrl: './reservas-listar.component.html',
  styleUrl: './reservas-listar.component.scss'
})
export class ReservasListarComponent implements OnInit {

  reservas: Reserva[] = [];
  displayedColumns = ['id', 'inicio', 'fin', 'estado', 'acciones'];

  constructor(
    private reservasService: ReservasService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.cargar();
  }

  cargar() {
    this.reservasService.getAll().subscribe({
      next: (res) => this.reservas = res,
      error: err => console.error('Error cargando reservas', err)
    });
  }

  verDetalle(id: number) {
    this.router.navigate(['/reservas/detalle', id]);
  }

  crearReserva() {
    this.router.navigate(['/reservas/crear']);
  }
}
