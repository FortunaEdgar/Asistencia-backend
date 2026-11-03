# Asistencia Backend

API REST para el sistema de asistencia de empleados, construida con ASP.NET Core 10 y PostgreSQL. Esta versión incluye el módulo de autenticación con JWT en cookie HttpOnly y contraseñas protegidas con BCrypt.

## Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/)
- Herramienta de EF Core: `dotnet tool install --global dotnet-ef`

## Instalación

```bash
# 1. Levantar PostgreSQL (puerto 5436)
docker compose up -d

# 2. Configurar la clave para firmar los JWT (mínimo 32 caracteres)
dotnet user-secrets set "Jwt:Key" "una-clave-secreta-de-al-menos-32-caracteres"

# 3. Aplicar las migraciones
dotnet ef database update

# 4. Ejecutar la API
dotnet run
```

La API queda disponible en `http://localhost:3000` y acepta peticiones con credenciales desde `http://localhost:5173` (frontend).

## Endpoints

Base: `/api/v1/auth`

| Método | Ruta        | Descripción                              | Autenticación |
| ------ | ----------- | ---------------------------------------- | ------------- |
| POST   | `/register` | Registra un usuario                      | No            |
| POST   | `/login`    | Inicia sesión y crea la cookie de acceso | No            |
| GET    | `/me`       | Devuelve el usuario autenticado          | Sí            |
| POST   | `/logout`   | Cierra sesión y elimina la cookie        | No            |

Ejemplo de cuerpo para `/register` (`/login` solo requiere `email` y `password`):

```json
{
  "name": "Ana Pérez",
  "email": "ana@example.com",
  "password": "contraseña123",
  "confirmPassword": "contraseña123"
}
```

## Estructura

```text
Data/         Contexto de base de datos (EF Core)
Migrations/   Migraciones de la base de datos
Modules/Auth/ Controlador, DTOs, entidad, repositorio y servicios de autenticación
Program.cs    Configuración y arranque de la aplicación
```
