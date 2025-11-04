# Documento de Requisitos Funcionales


**Proyecto:** Sistema Integral de Reservas de Espacios



## 1. Introducción


### 1.1 Propósito
Explicar el objetivo general del sistema y su alcance dentro de la institución o contexto.


### 1.2 Alcance del Sistema
Registro y login de usuarios.


### 1.3 Actores del Sistema
| Actor | Descripción |
|--------|--------------|
| Usuario | Persona que realiza reservas. |
| Administrador | Gestiona usuarios, espacios y reservas. |


## 2. Requisitos Funcionales


| ID | Requisito | Descripción | Prioridad |
|----|------------|-------------|------------|
| RF-001 | Registro de usuarios | El sistema debe permitir registrar nuevos usuarios con email y contraseña. | Alta |
| RF-002 | Login | El sistema debe permitir autenticación mediante JWT. | Alta |
| RF-003 | CRUD de reservas | Crear, listar, modificar y cancelar reservas. | Alta |
| RF-004 | Control de disponibilidad | Validar solapamientos y disponibilidad de espacios. | Alta |
| RF-005 | Notificaciones | Enviar notificaciones al crear o finalizar reservas. | Media |


## 3. Requisitos No Funcionales


| ID | Requisito | Descripción |
|----|------------|-------------|
| RNF-001 | Seguridad | Cifrado de contraseñas y validación de roles. |
| RNF-002 | Rendimiento | Tiempo de respuesta < 2s. |
| RNF-003 | Usabilidad | Diseño responsive y accesible. |
| RNF-004 | Mantenibilidad | Código estructurado por capas y documentado. |


## 4. Criterios de Aceptación
- El usuario puede crear y cancelar reservas sin conflictos de horario.
- Una reserva finalizada genera un evento Kafka.
- Las contraseñas se almacenan cifradas.


## 5. Historias de Usuario (Ejemplo)
| ID | Como | Quiero | Para | Criterios de Aceptación |
|----|-------|--------|------|--------------------------|
| US-001 | Usuario | Crear una reserva | Agendar el uso de un espacio | La reserva debe validarse y confirmarse |
| US-002 | Admin | Ver todas las reservas | Gestionar disponibilidad | Lista completa de reservas |


## 6. Flujo General
registro → login → selección de espacio → reserva → confirmación → notificación.