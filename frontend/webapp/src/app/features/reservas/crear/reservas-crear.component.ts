import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { ReservasService, Reserva } from '../../../core/services/reservas.service';
import { HttpClientModule } from '@angular/common/http';

interface Espacio {
  id: number;
  nombre: string;
}

@Component({
  selector: 'app-reservas-crear',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    HttpClientModule
  ],
  templateUrl: './reservas-crear.component.html',
  styleUrl: './reservas-crear.component.scss'
})
export class ReservasCrearComponent implements OnInit {

  form!: FormGroup;
  espacios: Espacio[] = [];

  constructor(
    private fb: FormBuilder,
    private reservasService: ReservasService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      espacioId: [null, Validators.required],
      fechaInicio: [null, Validators.required],
      fechaFin: [null, Validators.required]
    });

    this.cargarEspacios();
  }

  cargarEspacios() {
    // llamar endpoint espacios
    // Por ahora ponemos ejemplo dummy
    this.espacios = [
      { id: 1, nombre: 'Sala A' },
      { id: 2, nombre: 'Sala B' },
      { id: 3, nombre: 'Sala C' }
    ];
  }

  submit() {
    if (this.form.invalid) return;

    const data: Partial<Reserva> = this.form.value;

    this.reservasService.create(data).subscribe({
      next: (res) => {
        alert('Reserva creada correctamente');
        this.router.navigate(['/reservas']);
      },
      error: (err) => {
        console.error(err);
        alert('Error creando reserva: ' + (err.error?.message || err.message));
      }
    });
  }
}
