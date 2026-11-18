# Expediente Electrónico para una Clínica Veterinaria

## Descripción

Proyecto programado del curso Desarrollo de Software IV: un sistema para que una clínica veterinaria mantenga el expediente electrónico de las mascotas que atiende.

**VeterinariaMVC** es la aplicación web en ASP.NET Core MVC (.NET 10). Contiene los módulos de administración y de veterinaria (autenticación por cookie) y la API REST (autenticación JWT). Usa SQL Server con Entity Framework Core (Code First con migraciones).

## Integrantes
Este proyecto se encuentra trabajado por los estudiantes:
- Anthony Salguera Robles - C4J684
- Dany Javier Muñoz Solano - C5H727
- Javier Villalta Perez - C4L022
- Julio Cesar Ortiz Castro - C4I158

## Estructura del proyecto
```
ProyectoVeterinaria/
└── VeterinariaMVC/
    ├── Controllers/
    ├── Data/              AppDbContext y Migrations
    ├── Models/            Entidades, enums y ViewModels
    ├── Services/          Servicio de archivos
    ├── Views/
    └── wwwroot/
```

## Inicio e Instalación

### Requisitos
- SDK de .NET 10
- SQL Server (Express o Developer)
- Herramienta de Entity Framework: `dotnet tool install --global dotnet-ef`

### Ejecutar la aplicación web
1. Revisar la cadena de conexión `Default` en `VeterinariaMVC/appsettings.json`.
2. Ejecutar desde la carpeta `VeterinariaMVC`:
   ```
   dotnet run
   ```
   Al iniciar se aplican las migraciones y se crean automáticamente los usuarios de prueba.

La clave JWT de desarrollo está en `appsettings.Development.json`. En el servidor publicado se configura una clave distinta en `Jwt:Key`.

## Usuarios de prueba

| Perfil        | Correo                | Contraseña |
|---------------|-----------------------|------------|
| Administrador | admin@veterinaria.com | Admin123!  |
| Veterinario   | vet@veterinaria.com   | Vet123!    |
| Dueño         | dueno@veterinaria.com | Dueno123!  |

## Aplicación publicada
- URL pública: pendiente

### Cómo volver a publicar
Pendiente.

## Estilo de pantallas
Pendiente.
