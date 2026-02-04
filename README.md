# Sistema de gesion para asociaciones civiles

Este proyecto es una plataforma de gestión integral para asociaciones civiles, que incluye gestión de socios, cobranzas, alquileres y pagos online.

## 🛠️ Desarrollo Local (Hybrid Setup)

Para un desarrollo ágil, se recomienda correr la base de datos en Docker y las aplicaciones (API y Frontend) de forma nativa.

### Reqisitos Previos

- Docker Desktop
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js (v20+)](https://nodejs.org/)

### Pasos para levantar el entorno:

1. **Base de Datos (PostgreSQL):**
   Para desarrollo local, crea un archivo `docker-compose.override.yml` en la raíz (este archivo está en el `.gitignore`) con el siguiente contenido para configurar las credenciales y habilitar el puerto:

   ```yaml
   services:
     postgres:
       environment:
         POSTGRES_DB: casadeljubiladodb
         POSTGRES_USER: casadeljubiladouser
         POSTGRES_PASSWORD: casadeljubiladopass
       ports:
         - "5432:5432"
   ```

   Luego, levanta el servicio:

   ```bash
   docker compose up -d postgres
   ```

   _Nota: Se creará un volumen local llamado `postgres_data` en tu máquina, totalmente independiente de producción._

2. **Backend (ASP.NET Core API):**

   ```bash
   cd APIClub
   dotnet run --launch-profile http
   ```

   La API correrá en `http://localhost:5194` y se conectará a la DB de Docker usando `appsettings.Development.json`.

3. **Frontend (Vite + Vue):**
   ```bash
   cd Frontend
   npm install
   npm run dev
   ```
   El frontend correrá en `http://localhost:5173`. Asegúrate de que el archivo `Frontend/.env` apunte a `http://localhost:5194/api`.

---

## 🚀 Despliegue en VPS (Docker Full)

El sistema está preparado para ser desplegado como un conjunto de microservicios orquestados por Docker Compose, incluyendo Nginx con SSL (Certbot).

### 1. Preparar el servidor

- Instalar Docker y Docker Compose.
- Clonar el repositorio en el VPS.

### 2. Configurar Variables de Entorno

Crea un archivo `.env` en la raíz del proyecto basándote en `.env.example`. Asegúrate de configurar:

- Credenciales de PostgreSQL.
- Tokens de WhatsApp y Mercado Pago.
- URLs de producción para el Frontend y la API.

### 3. Ejecutar el despliegue

Ejecuta el siguiente comando para construir y levantar todos los servicios (Base de datos, API, Frontend, Nginx y Certbot):

```bash
docker-compose up -d --build
```

### 4. Configuración de Nginx y SSL

- El archivo `nginx/nginx.conf` debe estar configurado con los dominios correctos.
- El servicio `certbot` se encargará de generar y renovar los certificados SSL de Let's Encrypt automáticamente.

---

## 🗄️ Otros comandos útiles

- **Ejecutar Seeder (Datos de prueba):** `dotnet run --ExecuteSeeder` (dentro de APIClub).
- **Detener todo en local:** `docker compose down`.
- **Limpiar base de datos local (borrar volumen):** `docker compose down -v`.
