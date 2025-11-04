# Modelo de Datos — Sistema de Reservas


**Versión:** 1.0



## 1. Descripción General
El modelo de datos define las entidades principales y sus relaciones.


## 2. Entidades Principales


### Tabla: users
| Campo | Tipo | Descripción |
|--------|------|-------------|
| id | uuid | Identificador único |
| email | varchar(255) | Correo del usuario |
| password_hash | varchar | Contraseña cifrada |
| role | varchar(20) | user / admin |
| created_at | timestamp | Fecha de creación |


### Tabla: spaces ??
| Campo | Tipo | Descripción |
|--------|------|-------------|
| id | uuid | Identificador del espacio |
| name | varchar(100) | Nombre del espacio |
| capacity | int | Capacidad máxima |
| location | varchar(255) | Dirección o lugar |
| created_at | timestamp | Fecha de registro |


### Tabla: reservations
| Campo | Tipo | Descripción |
|--------|------|-------------|
| id | uuid | Identificador de reserva |
| user_id | uuid (FK→users) | Usuario que reservó |
| space_id | uuid (FK→spaces) | Espacio reservado |
| start_at | timestamp | Inicio de la reserva |
| end_at | timestamp | Fin de la reserva |
| status | varchar(20) | pendiente, confirmada, cancelada, finalizada |
| created_at | timestamp | Fecha creación |


## 3. Relaciones
- **users (1) — (N) reservations**
- **spaces (1) — (N) reservations**




## 4. Observaciones Técnicas
- Los identificadores serán UUID para evitar colisiones.
- El campo `metadata` (JSON) permite extensibilidad.